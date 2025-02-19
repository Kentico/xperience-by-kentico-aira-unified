using System.ComponentModel.DataAnnotations;
using System.Data;

using CMS;
using CMS.DataEngine;

using CMS.Helpers;
using CMS.Membership;

using Kentico.Xperience.AiraUnified.Admin.InfoModels;

[assembly: RegisterObjectType(typeof(AiraUnifiedChatPromptGroupInfo), AiraUnifiedChatPromptGroupInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.AiraUnified.Admin.InfoModels;

/// <summary>
/// Data conainer class for <see cref="AiraUnifiedChatPromptGroupInfo"/>.
/// </summary>
[Serializable]
internal class AiraUnifiedChatPromptGroupInfo : AbstractInfo<AiraUnifiedChatPromptGroupInfo, IInfoProvider<AiraUnifiedChatPromptGroupInfo>>
{
    /// <summary>
    /// Object type.
    /// </summary>
    public const string OBJECT_TYPE = "kenticoaira.airaunifiedchatpromptgroup";


    /// <summary>
    /// Type information.
    /// </summary>
    public static readonly ObjectTypeInfo TYPEINFO = new(typeof(IInfoProvider<AiraUnifiedChatPromptGroupInfo>), OBJECT_TYPE, "KenticoAira.AiraUnifiedChatPromptGroup", nameof(AiraUnifiedChatPromptGroupId), null, nameof(AiraUnifiedChatPromptGroupGuid), null, null, null, null, null)
    {
        TouchCacheDependencies = true,
        DependsOn =
        [
            new(nameof(AiraUnifiedChatPromptUserId), UserInfo.OBJECT_TYPE, ObjectDependencyEnum.Required)
        ],
        ContinuousIntegrationSettings =
        {
            Enabled = false
        }
    };


    /// <summary>
    /// Chat prompt group id.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual int AiraUnifiedChatPromptGroupId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraUnifiedChatPromptGroupId)), 0);
        set => SetValue(nameof(AiraUnifiedChatPromptGroupId), value);
    }


    /// <summary>
    /// Chat prompt group guid.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual Guid AiraUnifiedChatPromptGroupGuid
    {
        get => ValidationHelper.GetGuid(GetValue(nameof(AiraUnifiedChatPromptGroupGuid)), Guid.Empty);
        set => SetValue(nameof(AiraUnifiedChatPromptGroupGuid), value);
    }


    /// <summary>
    /// Chat prompt group creation time.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual DateTime AiraUnifiedChatPromptGroupCreatedWhen
    {
        get => ValidationHelper.GetDateTime(GetValue(nameof(AiraUnifiedChatPromptGroupCreatedWhen)), DateTimeHelper.ZERO_TIME);
        set => SetValue(nameof(AiraUnifiedChatPromptGroupCreatedWhen), value);
    }


    /// <summary>
    /// Chat prompt id.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual int AiraUnifiedChatPromptUserId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraUnifiedChatPromptUserId)), 0);
        set => SetValue(nameof(AiraUnifiedChatPromptUserId), value);
    }


    /// <summary>
    /// Deletes the object using appropriate provider.
    /// </summary>
    protected override void DeleteObject() => Provider.Delete(this);


    /// <summary>
    /// Updates the object using appropriate provider.
    /// </summary>
    protected override void SetObject() => Provider.Set(this);


    public AiraUnifiedChatPromptGroupInfo()
        : base(TYPEINFO)
    {
    }


    public AiraUnifiedChatPromptGroupInfo(DataRow dr)
        : base(TYPEINFO, dr)
    {
    }
}
