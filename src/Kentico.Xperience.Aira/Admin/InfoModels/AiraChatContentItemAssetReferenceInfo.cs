using System.Data;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;

using Kentico.Xperience.Aira.Admin.InfoModels;

[assembly: RegisterObjectType(typeof(AiraChatContentItemAssetReferenceInfo), AiraChatContentItemAssetReferenceInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.Aira.Admin.InfoModels;

/// <summary>
/// Data container class for <see cref="AiraChatContentItemAssetReferenceInfo"/>.
/// </summary>
[Serializable]
public partial class AiraChatContentItemAssetReferenceInfo : AbstractInfo<AiraChatContentItemAssetReferenceInfo, IInfoProvider<AiraChatContentItemAssetReferenceInfo>>
{
    /// <summary>
    /// Object type.
    /// </summary>
    public const string OBJECT_TYPE = "kenticoaira.airachatcontentitemassetreference";


    public static readonly ObjectTypeInfo TYPEINFO = new(typeof(IInfoProvider<AiraChatContentItemAssetReferenceInfo>), OBJECT_TYPE, "KenticoAira.AiraChatContentItemAssetReference", nameof(AiraChatContentItemAssetReferenceId), null, nameof(AiraChatContentItemAssetReferenceGuid), null, null, null, null, null)
    {
        TouchCacheDependencies = true,
        DependsOn =
        [
            new(nameof(AiraChatContentItemAssetReferenceUserID), UserInfo.OBJECT_TYPE, ObjectDependencyEnum.Required),
            new(nameof(AiraChatContentItemAssetReferenceContentTypeDataClassInfoID), DataClassInfo.OBJECT_TYPE, ObjectDependencyEnum.Required)
        ],
        ContinuousIntegrationSettings =
        {
            Enabled = true
        }
    };


    [DatabaseField]
    public virtual int AiraChatContentItemAssetReferenceId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatContentItemAssetReferenceId)), 0);
        set => SetValue(nameof(AiraChatContentItemAssetReferenceId), value);
    }


    [DatabaseField]
    public virtual Guid AiraChatContentItemAssetReferenceGuid
    {
        get => ValidationHelper.GetGuid(GetValue(nameof(AiraChatContentItemAssetReferenceGuid)), Guid.Empty);
        set => SetValue(nameof(AiraChatContentItemAssetReferenceGuid), value);
    }


    [DatabaseField]
    public virtual int AiraChatContentItemAssetReferenceUserID
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatContentItemAssetReferenceUserID)), 0);
        set => SetValue(nameof(AiraChatContentItemAssetReferenceUserID), value);
    }


    [DatabaseField]
    public virtual int AiraChatContentItemAssetReferenceContentTypeDataClassInfoID
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatContentItemAssetReferenceContentTypeDataClassInfoID)), 0);
        set => SetValue(nameof(AiraChatContentItemAssetReferenceContentTypeDataClassInfoID), value);
    }


    [DatabaseField]
    public virtual string AiraChatContentItemAssetReferenceContentTypeAssetFieldName
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraChatContentItemAssetReferenceContentTypeAssetFieldName)), string.Empty);
        set => SetValue(nameof(AiraChatContentItemAssetReferenceContentTypeAssetFieldName), value);
    }


    [DatabaseField]
    public virtual DateTime AiraChatContentItemAssetReferenceUploadTime
    {
        get => ValidationHelper.GetDateTime(GetValue(nameof(AiraChatContentItemAssetReferenceUploadTime)), DateTimeHelper.ZERO_TIME);
        set => SetValue(nameof(AiraChatContentItemAssetReferenceUploadTime), value);
    }


    [DatabaseField]
    public virtual int AiraChatContentItemAssetReferenceContentItemID
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatContentItemAssetReferenceContentItemID)), 0);
        set => SetValue(nameof(AiraChatContentItemAssetReferenceContentItemID), value);
    }


    /// <summary>
    /// Deletes the object using appropriate provider.
    /// </summary>
    protected override void DeleteObject() => Provider.Delete(this);


    /// <summary>
    /// Updates the object using appropriate provider.
    /// </summary>
    protected override void SetObject() => Provider.Set(this);


    public AiraChatContentItemAssetReferenceInfo()
        : base(TYPEINFO)
    {
    }


    public AiraChatContentItemAssetReferenceInfo(DataRow dr)
        : base(TYPEINFO, dr)
    {
    }
}
