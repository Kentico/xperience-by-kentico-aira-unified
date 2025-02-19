using System.Text.Json;

using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using CMS.DataEngine.Query;
using CMS.FormEngine;
using CMS.Membership;

using Kentico.Xperience.AiraUnified.Admin;
using Kentico.Xperience.AiraUnified.Admin.UIPages;

using Microsoft.AspNetCore.Http;

using File = CMS.IO.File;
using Path = CMS.IO.Path;

namespace Kentico.Xperience.AiraUnified.Assets;

internal class AiraUnifiedAssetService : IAiraUnifiedAssetService
{
    private readonly IInfoProvider<ContentLanguageInfo> contentLanguageProvider;
    private readonly IInfoProvider<SettingsKeyInfo> settingsKeyProvider;
    private readonly IInfoProvider<RoleInfo> roleProvider;
    private readonly IEventLogService eventLogService;
    private readonly ISettingsService settingsService;

    public AiraUnifiedAssetService(IInfoProvider<ContentLanguageInfo> contentLanguageProvider,
        IInfoProvider<SettingsKeyInfo> settingsKeyProvider,
        IEventLogService eventLogService,
        IInfoProvider<RoleInfo> roleProvider,
        ISettingsService settingsService
        )
    {
        this.contentLanguageProvider = contentLanguageProvider;
        this.roleProvider = roleProvider;
        this.eventLogService = eventLogService;
        this.settingsService = settingsService;
        this.settingsKeyProvider = settingsKeyProvider;
    }

    public async Task<bool> DoesUserHaveAiraUnifiedPermission(string permission, int userId)
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
            .WhereEquals(nameof(ApplicationPermissionInfo.ApplicationName), AiraUnifiedApplicationPage.IDENTIFIER)
            .WhereEquals(nameof(ApplicationPermissionInfo.PermissionName), permission)
            .GetCountAsync();

        return countOfRolesWithTheRightWhereUserIsContained > 0;
    }

    private async Task<Dictionary<string, string>> GetMassAssetUploadConfiguration()
    {
        var massAssetUploadConfiguration = (await settingsKeyProvider
           .Get()
           .WhereEquals(nameof(SettingsKeyInfo.KeyName), AiraUnifiedConstants.MassAssetUploadConfigurationKey)
           .GetEnumerableTypedResultAsync())
           .First();

        var contentTypeInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(massAssetUploadConfiguration.KeyValue) ??
            throw new InvalidOperationException("No content type is configured for mass upload.");

        return contentTypeInfo;
    }

    public async Task<string> GetAllowedFileExtensions()
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

        var allowedExtensions = fields.Settings["AllowedExtensions"];

        if (allowedExtensions is not string)
        {
            eventLogService.LogWarning(nameof(IAiraUnifiedAssetService), nameof(GetAllowedFileExtensions), "No file format is configured for Smart Upload.");

            return string.Empty;
        }

        var settings = (string)allowedExtensions;

        if (string.Equals(settings, "_INHERITED_"))
        {
            return GetGlobalAllowedFileExtensions();
        }

        return settings;
    }

    public async Task<bool> HandleFileUpload(IFormFileCollection files, int userId)
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

            var fileCreated = await CreateContentAssetItem(createContentItemParameters, file, userId, contentItemAssetColumnCodeName);

            if (!fileCreated)
            {
                return false;
            }
        }

        return true;
    }

    private async Task<bool> IsFileExtensionAllowed(string fileExtension)
    {
        fileExtension = fileExtension.ToLowerInvariant().TrimStart('.');

        var allowedExtensions = (await GetAllowedFileExtensions()).ToLowerInvariant();

        if (allowedExtensions.Trim() == string.Empty)
        {
            return true;
        }

        if (fileExtension == string.Empty)
        {
            // Handle empty extension
            return allowedExtensions.Contains(";;") || allowedExtensions.StartsWith(";", StringComparison.Ordinal) || allowedExtensions.EndsWith(";", StringComparison.Ordinal);
        }

        allowedExtensions = string.Format(";{0};", allowedExtensions);
        return allowedExtensions.Contains(string.Format(";{0};", fileExtension)) || allowedExtensions.Contains(";." + fileExtension + ";");
    }

    private async Task<bool> CreateContentAssetItem(CreateContentItemParameters createContentItemParameters, IFormFile file, int userId, string contentItemAssetColumnCodeName)
    {
        var contentItemManager = Service.Resolve<IContentItemManagerFactory>().Create(userId);

        var tempDirectory = Directory.CreateTempSubdirectory();

        var tempFilePath = Path.Combine(tempDirectory.FullName, file.FileName);

        var extension = Path.GetExtension(tempFilePath);

        if (!await IsFileExtensionAllowed(extension))
        {
            eventLogService.LogError(nameof(IAiraUnifiedAssetService), nameof(CreateContentAssetItem), $"Smart uploader attempted to upload a file in {extension} format, which is not configured for mass upload.");

            return false;
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

        await contentItemManager.Create(createContentItemParameters, itemData);

        File.Delete(tempFilePath);
        tempDirectory.Delete(true);

        return true;
    }

    public string GetGlobalAllowedFileExtensions() => settingsService["CMSMediaFileAllowedExtensions"];
}
