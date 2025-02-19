using System.ComponentModel.DataAnnotations;
using System.Data;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;

using Kentico.Xperience.AiraUnified.Admin.InfoModels;

[assembly: RegisterObjectType(typeof(AiraUnifiedConfigurationItemInfo), AiraUnifiedConfigurationItemInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.AiraUnified.Admin.InfoModels;

/// <summary>
/// Data container class for <see cref="AiraUnifiedConfigurationItemInfo"/>.
/// </summary>
[Serializable]
public partial class AiraUnifiedConfigurationItemInfo : AbstractInfo<AiraUnifiedConfigurationItemInfo, IInfoProvider<AiraUnifiedConfigurationItemInfo>>
{
    /// <summary>
    /// Object type.
    /// </summary>
    public const string OBJECT_TYPE = "kenticoaira.airaunifiedconfigurationitem";


    /// <summary>
    /// Type information.
    /// </summary>
    public static readonly ObjectTypeInfo TYPEINFO = new(typeof(IInfoProvider<AiraUnifiedConfigurationItemInfo>), OBJECT_TYPE, "KenticoAira.AiraUnifiedConfigurationItem", nameof(AiraUnifiedConfigurationItemId), null, nameof(AiraUnifiedConfigurationItemGuid), null, null, null, null, null)
    {
        TouchCacheDependencies = true,
        ContinuousIntegrationSettings =
        {
            Enabled = true
        }
    };


    /// <summary>
    /// Aira unified configuration item id.
    /// </summary>
    [DatabaseField]
    public virtual int AiraUnifiedConfigurationItemId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraUnifiedConfigurationItemId)), 0);
        set => SetValue(nameof(AiraUnifiedConfigurationItemId), value);
    }


    /// <summary>
    /// Aira unified configuration item guid.
    /// </summary>
    [DatabaseField]
    public virtual Guid AiraUnifiedConfigurationItemGuid
    {
        get => ValidationHelper.GetGuid(GetValue(nameof(AiraUnifiedConfigurationItemGuid)), Guid.Empty);
        set => SetValue(nameof(AiraUnifiedConfigurationItemGuid), value);
    }


    /// <summary>
    /// Aira unified path base.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual string AiraUnifiedConfigurationItemAiraPathBase
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraUnifiedConfigurationItemAiraPathBase)), string.Empty);
        set => SetValue(nameof(AiraUnifiedConfigurationItemAiraPathBase), value);
    }


    /// <summary>
    /// Logo asset id.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual string AiraUnifiedConfigurationItemAiraRelativeLogoId
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraUnifiedConfigurationItemAiraRelativeLogoId)), string.Empty);
        set => SetValue(nameof(AiraUnifiedConfigurationItemAiraRelativeLogoId), value);
    }


    /// <summary>
    /// Aira unified chat page title.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual string AiraUnifiedConfigurationItemAiraUnifiedChatTitle
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraUnifiedConfigurationItemAiraUnifiedChatTitle)), string.Empty);
        set => SetValue(nameof(AiraUnifiedConfigurationItemAiraUnifiedChatTitle), value);
    }


    /// <summary>
    /// Smart upload page title.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual string AiraUnifiedConfigurationItemAiraSmartUploadTitle
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraUnifiedConfigurationItemAiraSmartUploadTitle)), string.Empty);
        set => SetValue(nameof(AiraUnifiedConfigurationItemAiraSmartUploadTitle), value);
    }

    /// <summary>
    /// Deletes the object using appropriate provider.
    /// </summary>
    protected override void DeleteObject() => Provider.Delete(this);


    /// <summary>
    /// Updates the object using appropriate provider.
    /// </summary>
    protected override void SetObject() => Provider.Set(this);


    public AiraUnifiedConfigurationItemInfo()
        : base(TYPEINFO)
    {
    }


    public AiraUnifiedConfigurationItemInfo(DataRow dr)
        : base(TYPEINFO, dr)
    {
    }
}
