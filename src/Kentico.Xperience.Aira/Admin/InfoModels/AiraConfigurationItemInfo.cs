using System.ComponentModel.DataAnnotations;
using System.Data;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;

using Kentico.Xperience.Aira.Admin.InfoModels;

[assembly: RegisterObjectType(typeof(AiraConfigurationItemInfo), AiraConfigurationItemInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.Aira.Admin.InfoModels;

/// <summary>
/// Data container class for <see cref="AiraConfigurationItemInfo"/>.
/// </summary>
[Serializable]
public partial class AiraConfigurationItemInfo : AbstractInfo<AiraConfigurationItemInfo, IInfoProvider<AiraConfigurationItemInfo>>
{
    /// <summary>
    /// Object type.
    /// </summary>
    public const string OBJECT_TYPE = "kenticoaira.airaconfigurationitem";


    /// <summary>
    /// Type information.
    /// </summary>
    public static readonly ObjectTypeInfo TYPEINFO = new(typeof(IInfoProvider<AiraConfigurationItemInfo>), OBJECT_TYPE, "KenticoAira.AiraConfigurationItem", nameof(AiraConfigurationItemId), null, nameof(AiraConfigurationItemGuid), null, null, null, null, null)
    {
        TouchCacheDependencies = true,
        ContinuousIntegrationSettings =
        {
            Enabled = true
        }
    };


    /// <summary>
    /// Aira configuration item id.
    /// </summary>
    [DatabaseField]
    public virtual int AiraConfigurationItemId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraConfigurationItemId)), 0);
        set => SetValue(nameof(AiraConfigurationItemId), value);
    }


    /// <summary>
    /// Aira configuration item guid.
    /// </summary>
    [DatabaseField]
    public virtual Guid AiraConfigurationItemGuid
    {
        get => ValidationHelper.GetGuid(GetValue(nameof(AiraConfigurationItemGuid)), Guid.Empty);
        set => SetValue(nameof(AiraConfigurationItemGuid), value);
    }


    /// <summary>
    /// Aira path base.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual string AiraConfigurationItemAiraPathBase
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraConfigurationItemAiraPathBase)), string.Empty);
        set => SetValue(nameof(AiraConfigurationItemAiraPathBase), value);
    }


    /// <summary>
    /// Logo asset id.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual string AiraConfigurationItemAiraRelativeLogoId
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraConfigurationItemAiraRelativeLogoId)), string.Empty);
        set => SetValue(nameof(AiraConfigurationItemAiraRelativeLogoId), value);
    }


    /// <summary>
    /// Chat image asset id.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual string AiraConfigurationItemAiraRelativeChatImgId
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraConfigurationItemAiraRelativeChatImgId)), string.Empty);
        set => SetValue(nameof(AiraConfigurationItemAiraRelativeChatImgId), value);
    }


    /// <summary>
    /// Chat title.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual string AiraConfigurationItemAiraChatTitle
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraConfigurationItemAiraChatTitle)), string.Empty);
        set => SetValue(nameof(AiraConfigurationItemAiraChatTitle), value);
    }


    /// <summary>
    /// Smart upload title.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual string AiraConfigurationItemAiraSmartUploadTitle
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraConfigurationItemAiraSmartUploadTitle)), string.Empty);
        set => SetValue(nameof(AiraConfigurationItemAiraSmartUploadTitle), value);
    }


    /// <summary>
    /// Smart upload image asset id.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual string AiraConfigurationItemAiraSmartUploadImgId
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraConfigurationItemAiraSmartUploadImgId)), string.Empty);
        set => SetValue(nameof(AiraConfigurationItemAiraSmartUploadImgId), value);
    }


    /// <summary>
    /// Deletes the object using appropriate provider.
    /// </summary>
    protected override void DeleteObject() => Provider.Delete(this);


    /// <summary>
    /// Updates the object using appropriate provider.
    /// </summary>
    protected override void SetObject() => Provider.Set(this);


    public AiraConfigurationItemInfo()
        : base(TYPEINFO)
    {
    }


    public AiraConfigurationItemInfo(DataRow dr)
        : base(TYPEINFO, dr)
    {
    }
}
