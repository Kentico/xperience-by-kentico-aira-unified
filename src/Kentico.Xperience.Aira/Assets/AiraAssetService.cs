using System.Text.Json;

using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using CMS.DataEngine.Query;
using CMS.Membership;

using Kentico.Xperience.Aira.Admin;
using Kentico.Xperience.Aira.Admin.UIPages;

using Microsoft.AspNetCore.Http;

using File = CMS.IO.File;
using Path = CMS.IO.Path;

namespace Kentico.Xperience.Aira.Assets;

internal class AiraAssetService : IAiraAssetService
{
    private readonly IInfoProvider<ContentLanguageInfo> contentLanguageProvider;
    private readonly IInfoProvider<SettingsKeyInfo> settingsKeyProvider;
    private readonly IInfoProvider<RoleInfo> roleProvider;

    public AiraAssetService(IInfoProvider<ContentLanguageInfo> contentLanguageProvider,
        IInfoProvider<SettingsKeyInfo> settingsKeyProvider,
        IInfoProvider<RoleInfo> roleProvider
        )
    {
        this.contentLanguageProvider = contentLanguageProvider;
        this.roleProvider = roleProvider;
        this.settingsKeyProvider = settingsKeyProvider;
    }

    public async Task<bool> DoesUserHaveAiraCompanionAppPermission(string permission, int userId)
    {
        int countOfRolesWithTheRightWhereUserIsContained = await roleProvider
            .Get()
            .Source(x =>
            {
                x.InnerJoin<ApplicationPermissionInfo>(
                    nameof(RoleInfo.RoleID),
                    nameof(ApplicationPermissionInfo.RoleID)
                );

                x.InnerJoin<UserRoleInfo>(
                    nameof(RoleInfo.RoleID),
                    nameof(UserRoleInfo.RoleID)
                );
            })
            .WhereEquals(nameof(UserRoleInfo.UserID), userId)
            .WhereEquals(nameof(ApplicationPermissionInfo.ApplicationName), AiraApplicationPage.IDENTIFIER)
            .WhereEquals(nameof(ApplicationPermissionInfo.PermissionName), permission)
            .GetCountAsync();

        return countOfRolesWithTheRightWhereUserIsContained > 0;
    }

    public async Task HandleFileUpload(IFormFileCollection files, int userId)
    {
        var massAssetUploadConfiguration = (await settingsKeyProvider
            .Get()
            .WhereEquals(nameof(SettingsKeyInfo.KeyName), AiraCompanionAppConstants.MassAssetUploadConfigurationKey)
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

            await CreateContentAssetItem(createContentItemParameters, file, userId, contentItemAssetColumnCodeName);
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

        var itemData = new ContentItemData(new Dictionary<string, object>{
            { contentItemAssetColumnCodeName, assetMetadataWithSource }
        });

        int contentItemId = await contentItemManager.Create(createContentItemParameters, itemData);

        File.Delete(tempFilePath);
        tempDirectory.Delete(true);

        return contentItemId;
    }
}
