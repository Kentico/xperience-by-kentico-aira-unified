using System.Text.Json;

using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;

using Kentico.Xperience.Aira.Admin;
using Kentico.Xperience.Aira.Admin.InfoModels;

using Microsoft.AspNetCore.Http;

using File = CMS.IO.File;
using Path = CMS.IO.Path;

namespace Kentico.Xperience.Aira.Assets;

public interface IAiraAiraAssetService
{
    Task HandleFileUpload(IFormFileCollection files, int userId);
    Task<List<string>> GetUsersUploadedAssetUrls(int userId);
}

internal class AiraAssetService : IAiraAiraAssetService
{
    private readonly IInfoProvider<ContentLanguageInfo> contentLanguageProvider;
    private readonly IInfoProvider<SettingsKeyInfo> settingsKeyProvider;
    private readonly IInfoProvider<AiraChatContentItemAssetReferenceInfo> airaChatContentItemAssetReferenceProvider;
    private readonly IContentQueryExecutor contentQueryExecutor;
    private readonly IContentItemAssetRetriever contentItemAssetRetriever;

    public AiraAssetService(IInfoProvider<ContentLanguageInfo> contentLanguageProvider,
        IInfoProvider<SettingsKeyInfo> settingsKeyProvider,
        IInfoProvider<AiraChatContentItemAssetReferenceInfo> airaChatContentItemAssetReferenceProvider,
        IContentQueryExecutor contentQueryExecutor,
        IContentItemAssetRetriever contentItemAssetRetriever)
    {
        this.contentLanguageProvider = contentLanguageProvider;
        this.contentItemAssetRetriever = contentItemAssetRetriever;
        this.contentQueryExecutor = contentQueryExecutor;
        this.settingsKeyProvider = settingsKeyProvider;
        this.airaChatContentItemAssetReferenceProvider = airaChatContentItemAssetReferenceProvider;
    }

    public async Task<List<string>> GetUsersUploadedAssetUrls(int userId)
    {
        var contentItemAssetReferences = (await airaChatContentItemAssetReferenceProvider
            .Get()
            .Source(x => x.InnerJoin<DataClassInfo>(
                nameof(AiraChatContentItemAssetReferenceInfo.AiraChatContentItemAssetReferenceContentTypeDataClassInfoID),
                nameof(DataClassInfo.ClassID)
            ))
            .WhereEquals(nameof(AiraChatContentItemAssetReferenceInfo.AiraChatContentItemAssetReferenceUserID), userId)
            .Columns(nameof(AiraChatContentItemAssetReferenceInfo.AiraChatContentItemAssetReferenceContentItemID),
                nameof(DataClassInfo.ClassName),
                nameof(AiraChatContentItemAssetReferenceInfo.AiraChatContentItemAssetReferenceContentTypeAssetFieldName),
                nameof(AiraChatContentItemAssetReferenceInfo.AiraChatContentItemAssetReferenceUploadTime))
            .GetDataContainerResultAsync())
            .GroupBy(x =>
                new
                {
                    className = x[nameof(DataClassInfo.ClassName)] as string,
                    columnName = x[nameof(AiraChatContentItemAssetReferenceInfo.AiraChatContentItemAssetReferenceContentTypeAssetFieldName)] as string
                }
            );

        var resultingContentItemAssets = new List<AiraContentItemAsset>();

        foreach (var contentType in contentItemAssetReferences)
        {
            var builder = new ContentItemQueryBuilder();
            builder.ForContentType(contentType.Key.className, parameters => parameters
                .Where(where => where.WhereIn(nameof(ContentItemFields.ContentItemID), contentType
                        .Select(x => (int)x[nameof(AiraChatContentItemAssetReferenceInfo.AiraChatContentItemAssetReferenceContentItemID)])
                        .ToList()
                    )
                )
            );

            var contentItemAssets = await contentQueryExecutor.GetResult(builder, async x => new AiraContentItemAsset
            {
                Url = (await contentItemAssetRetriever.Retrieve(x, contentType.Key.columnName)).Url,
                ContentItemID = x.ContentItemID
            });

            resultingContentItemAssets.AddRange(contentItemAssets.Join(contentType.ToList(),
                x => x.ContentItemID,
                y => y[nameof(AiraChatContentItemAssetReferenceInfo.AiraChatContentItemAssetReferenceContentItemID)],
                (x, y) =>
                {
                    x.Created = (DateTime)y[nameof(AiraChatContentItemAssetReferenceInfo.AiraChatContentItemAssetReferenceUploadTime)];
                    return x;
                })
            );
        }

        return resultingContentItemAssets.OrderBy(x => x.Created).Select(x => x.Url).ToList();
    }

    public async Task HandleFileUpload(IFormFileCollection files, int userId)
    {
        var massAssetUploadConfiguration = (await settingsKeyProvider
            .Get()
            .WhereEquals(nameof(SettingsKeyInfo.KeyName), AiraConstants.MassAssetUploadConfigurationKey)
            .GetEnumerableTypedResultAsync())
            .First();

        var contentTypeInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(massAssetUploadConfiguration.KeyValue) ??
            throw new InvalidOperationException("No content type is configured for mass upload.");

        var contentTypeGuid = Guid.Parse(contentTypeInfo["ContentTypeGuid"]);

        var contentType = (await DataClassInfoProvider.ProviderObject
            .Get()
            .WhereEquals(nameof(DataClassInfo.ClassGUID), contentTypeGuid)
            .GetEnumerableTypedResultAsync())
            .Single();

        string contentItemAssetColumnCodeName = contentTypeInfo["AssetFieldName"];

        string languageName = (await contentLanguageProvider
            .Get()
            .WhereEquals(nameof(ContentLanguageInfo.ContentLanguageIsDefault), true)
            .GetEnumerableTypedResultAsync())
            .First()
            .ContentLanguageName;

        foreach (var file in files)
        {
            var createContentItemParameters = new CreateContentItemParameters(contentType.ClassName, null, file.FileName, languageName, "KenticoDefault");

            int contentItemId = await CreateContentAssetItem(createContentItemParameters, file, userId, contentItemAssetColumnCodeName);
            CreateContentAssetItemChatReference(userId, contentType.ClassID, contentItemAssetColumnCodeName, contentItemId);
        }
    }

    /// <summary>
    /// ID of newly created content item assset.
    /// </summary>
    /// <param name="createContentItemParameters"></param>
    /// <param name="file"></param>
    /// <param name="userId"></param>
    /// <param name="contentItemAssetColumnCodeName"></param>
    /// <returns></returns>
    private async Task<int> CreateContentAssetItem(CreateContentItemParameters createContentItemParameters, IFormFile file, int userId, string contentItemAssetColumnCodeName)
    {
        var contentItemManager = Service.Resolve<IContentItemManagerFactory>().Create(userId);

        var tempDirectory = Directory.CreateTempSubdirectory();

        string tempFilePath = Path.Combine(tempDirectory.FullName, file.FileName);
        using var fileStream = File.Create(tempFilePath);
        await file.CopyToAsync(fileStream);

        fileStream.Seek(0, SeekOrigin.Begin);

        var assetMetadata = new ContentItemAssetMetadata()
        {
            Extension = Path.GetExtension(tempFilePath),
            Identifier = Guid.NewGuid(),
            LastModified = DateTime.Now,
            Name = Path.GetFileName(tempFilePath),
            Size = fileStream.Length
        };

        var fileSource = new ContentItemAssetStreamSource((CancellationToken cancellationToken) => Task.FromResult<Stream>(fileStream));
        var assetMetadataWithSource = new ContentItemAssetMetadataWithSource(fileSource, assetMetadata);

        //assetMetadataWithSource.SetOptimizationParameters(new ImageOptimizationParameters
        //{
        //    Quality = 100
        //});

        var itemData = new ContentItemData(new Dictionary<string, object>{
            { contentItemAssetColumnCodeName, assetMetadataWithSource }
        });

        int contentItemId = await contentItemManager.Create(createContentItemParameters, itemData);
        _ = await contentItemManager.TryPublish(contentItemId, createContentItemParameters.LanguageName);

        File.Delete(tempFilePath);
        tempDirectory.Delete(true);

        return contentItemId;
    }

    private void CreateContentAssetItemChatReference(int userId, int classId, string fieldName, int contentItemId)
    {
        var referenceItem = new AiraChatContentItemAssetReferenceInfo
        {
            AiraChatContentItemAssetReferenceUserID = userId,
            AiraChatContentItemAssetReferenceContentTypeDataClassInfoID = classId,
            AiraChatContentItemAssetReferenceContentTypeAssetFieldName = fieldName,
            AiraChatContentItemAssetReferenceUploadTime = DateTime.Now,
            AiraChatContentItemAssetReferenceContentItemID = contentItemId
        };

        airaChatContentItemAssetReferenceProvider.Set(referenceItem);
    }
}

public class AiraContentItemAsset
{
    public string Url { get; set; } = string.Empty;
    public int ContentItemID { get; set; }
    public DateTime? Created { get; set; }
}
