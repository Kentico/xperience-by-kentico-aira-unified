using System.ComponentModel.DataAnnotations;
using System.Data;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;

using Kentico.Xperience.AiraUnified.Admin.InfoModels;

[assembly: RegisterObjectType(typeof(AiraUnifiedChatPromptInfo), AiraUnifiedChatPromptInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.AiraUnified.Admin.InfoModels;

/// <summary>
/// Data conainer class for <see cref="AiraUnifiedChatPromptInfo"/>.
/// </summary>
[Serializable]
public partial class AiraUnifiedChatPromptInfo : AbstractInfo<AiraUnifiedChatPromptInfo, IInfoProvider<AiraUnifiedChatPromptInfo>>
{
    /// <summary>
    /// Object type.
    /// </summary>
    public const string OBJECT_TYPE = "kenticoaira.airaunifiedchatprompt";


    /// <summary>
    /// Type information.
    /// </summary>
    public static readonly ObjectTypeInfo TYPEINFO = new(typeof(IInfoProvider<AiraUnifiedChatPromptInfo>), OBJECT_TYPE, "KenticoAira.AiraUnifiedChatPrompt", nameof(AiraUnifiedChatPromptId), null, nameof(AiraUnifiedChatPromptGuid), null, null, null, null, null)
    {
        TouchCacheDependencies = true,
        DependsOn =
        [
            new(nameof(AiraUnifiedChatPromptChatPromptGroupId), AiraUnifiedChatPromptGroupInfo.OBJECT_TYPE, ObjectDependencyEnum.Required)
        ],
        ContinuousIntegrationSettings =
        {
            Enabled = false
        }
    };


    /// <summary>
    /// Chat prompt id.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual int AiraUnifiedChatPromptId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraUnifiedChatPromptId)), 0);
        set => SetValue(nameof(AiraUnifiedChatPromptId), value);
    }


    /// <summary>
    /// Chat prompt guid.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual Guid AiraUnifiedChatPromptGuid
    {
        get => ValidationHelper.GetGuid(GetValue(nameof(AiraUnifiedChatPromptGuid)), Guid.Empty);
        set => SetValue(nameof(AiraUnifiedChatPromptGuid), value);
    }


    /// <summary>
    /// Chat prompt group id.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual int AiraUnifiedChatPromptChatPromptGroupId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraUnifiedChatPromptChatPromptGroupId)), 0);
        set => SetValue(nameof(AiraUnifiedChatPromptChatPromptGroupId), value);
    }


    /// <summary>
    /// Chat prompt text.
    /// </summary>
    [DatabaseField]
    public virtual string AiraUnifiedChatPromptText
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraUnifiedChatPromptText)), string.Empty);
        set => SetValue(nameof(AiraUnifiedChatPromptText), value);
    }


    /// <summary>
    /// Deletes the object using appropriate provider.
    /// </summary>
    protected override void DeleteObject() => Provider.Delete(this);


    /// <summary>
    /// Updates the object using appropriate provider.
    /// </summary>
    protected override void SetObject() => Provider.Set(this);


    public AiraUnifiedChatPromptInfo()
        : base(TYPEINFO)
    {
    }


    public AiraUnifiedChatPromptInfo(DataRow dr)
        : base(TYPEINFO, dr)
    {
    }
}
