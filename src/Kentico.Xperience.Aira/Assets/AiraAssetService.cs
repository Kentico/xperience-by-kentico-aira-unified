using System.Text.Json;

using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using CMS.DataEngine.Query;
using CMS.FormEngine;
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
        var countOfRolesWithTheRightWhereUserIsContained = await roleProvider
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

    private async Task<Dictionary<string, string>> GetMassAssetUploadConfiguration()
    {
        var massAssetUploadConfiguration = (await settingsKeyProvider
           .Get()
           .WhereEquals(nameof(SettingsKeyInfo.KeyName), AiraCompanionAppConstants.MassAssetUploadConfigurationKey)
           .GetEnumerableTypedResultAsync())
           .First();

        var contentTypeInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(massAssetUploadConfiguration.KeyValue) ??
            throw new InvalidOperationException("No content type is configured for mass upload.");

        return contentTypeInfo;
    }

    public async Task<List<string>> GetAllowedFileExtensions()
    {
        var massAssetConfigurationInfo = await GetMassAssetUploadConfiguration();
        var contentItemAssetColumnCodeName = massAssetConfigurationInfo["AssetFieldName"];
        var contentTypeGuid = Guid.Parse(massAssetConfigurationInfo["ContentTypeGuid"]);

        var contentType = (await DataClassInfoProvider.ProviderObject
           .Get()
           .WhereEquals(nameof(DataClassInfo.ClassGUID), contentTypeGuid)
           .GetEnumerableTypedResultAsync())
           .Single();

        var contentTypeFormInfo = new FormInfo(contentType.ClassFormDefinition);
        var fields = contentTypeFormInfo.GetFormField(contentItemAssetColumnCodeName);

        var settings = JsonSerializer.Deserialize<List<string>>((string)(fields.Settings["InputImageExtensions"]
           ?? throw new InvalidOperationException("No file format is configured for Smart Upload.")));

        return settings
            ?? throw new InvalidOperationException("No file format is configured for Smart Upload.");
    }

    public async Task HandleFileUpload(IFormFileCollection files, int userId)
    {
        var massAssetConfigurationInfo = await GetMassAssetUploadConfiguration();

        var contentTypeGuid = Guid.Parse(massAssetConfigurationInfo["ContentTypeGuid"]);

        var contentType = (await DataClassInfoProvider.ProviderObject
            .Get()
            .WhereEquals(nameof(DataClassInfo.ClassGUID), contentTypeGuid)
            .GetEnumerableTypedResultAsync())
            .Single();

        var languageName = (await contentLanguageProvider
            .Get()
            .WhereEquals(nameof(ContentLanguageInfo.ContentLanguageIsDefault), true)
            .GetEnumerableTypedResultAsync())
            .First()
            .ContentLanguageName;

        var contentItemAssetColumnCodeName = massAssetConfigurationInfo["AssetFieldName"];

        foreach (var file in files)
        {
            var createContentItemParameters = new CreateContentItemParameters(contentType.ClassName, null, file.FileName, languageName, "KenticoDefault");

            await CreateContentAssetItem(createContentItemParameters, file, userId, contentItemAssetColumnCodeName);
        }
    }

    /// <summary>
    /// ID of newly created content item asset.
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

        var tempFilePath = Path.Combine(tempDirectory.FullName, file.FileName);

        var allowedExtensions = await GetAllowedFileExtensions();

        var extension = Path.GetExtension(tempFilePath);

        var extensionWithoutLeadingDot = extension[1..];

        if (extension[0] != '.' || !allowedExtensions.Contains(extensionWithoutLeadingDot))
        {
            throw new InvalidOperationException($"File type {extension} is not configured for smart asset upload.");
        }

        using var fileStream = File.Create(tempFilePath);
        await file.CopyToAsync(fileStream);

        fileStream.Seek(0, SeekOrigin.Begin);

        var assetMetadata = new ContentItemAssetMetadata()
        {
            Extension = extension,
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

        var contentItemId = await contentItemManager.Create(createContentItemParameters, itemData);

        File.Delete(tempFilePath);
        tempDirectory.Delete(true);

        return contentItemId;
    }
}
