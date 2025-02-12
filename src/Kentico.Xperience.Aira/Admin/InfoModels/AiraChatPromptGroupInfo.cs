using System.ComponentModel.DataAnnotations;
using System.Data;

using CMS;
using CMS.DataEngine;

using CMS.Helpers;
using CMS.Membership;

using Kentico.Xperience.Aira.Admin.InfoModels;

using Newtonsoft.Json;

[assembly: RegisterObjectType(typeof(AiraChatPromptGroupInfo), AiraChatPromptGroupInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.Aira.Admin.InfoModels;

/// <summary>
/// Data container class for <see cref="AiraChatPromptGroupInfo"/>.
/// </summary>
[Serializable]
internal class AiraChatPromptGroupInfo : AbstractInfo<AiraChatPromptGroupInfo, IInfoProvider<AiraChatPromptGroupInfo>>
{
    /// <summary>
    /// Object type.
    /// </summary>
    public const string OBJECT_TYPE = "kenticoaira.airachatpromptgroup";


    /// <summary>
    /// Type information.
    /// </summary>
    public static readonly ObjectTypeInfo TYPEINFO = new(typeof(IInfoProvider<AiraChatPromptGroupInfo>), OBJECT_TYPE, "KenticoAira.AiraChatPromptGroup", nameof(AiraChatPromptGroupId), null, nameof(AiraChatPromptGroupGuid), null, null, null, null, null)
    {
        TouchCacheDependencies = true,
        DependsOn =
        [
            new(nameof(AiraChatPromptGroupUserId), UserInfo.OBJECT_TYPE, ObjectDependencyEnum.Required),
            new(nameof(AiraChatPromptGroupThreadId), UserInfo.OBJECT_TYPE, ObjectDependencyEnum.Required)
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
    public virtual int AiraChatPromptGroupId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatPromptGroupId)), 0);
        set => SetValue(nameof(AiraChatPromptGroupId), value);
    }


    /// <summary>
    /// Chat prompt group guid.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual Guid AiraChatPromptGroupGuid
    {
        get => ValidationHelper.GetGuid(GetValue(nameof(AiraChatPromptGroupGuid)), Guid.Empty);
        set => SetValue(nameof(AiraChatPromptGroupGuid), value);
    }


    /// <summary>
    /// Chat prompt group creation time.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual DateTime AiraChatPromptGroupCreatedWhen
    {
        get => ValidationHelper.GetDateTime(GetValue(nameof(AiraChatPromptGroupCreatedWhen)), DateTimeHelper.ZERO_TIME);
        set => SetValue(nameof(AiraChatPromptGroupCreatedWhen), value);
    }


    /// <summary>
    /// Chat prompt user id.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual int AiraChatPromptGroupUserId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatPromptGroupUserId)), 0);
        set => SetValue(nameof(AiraChatPromptGroupUserId), value);
    }


    /// <summary>
    /// The chat thread id.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual int AiraChatPromptGroupThreadId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatPromptGroupThreadId)), 0);
        set => SetValue(nameof(AiraChatPromptGroupThreadId), value);
    }


    /// <summary>
    /// Deletes the object using appropriate provider.
    /// </summary>
    protected override void DeleteObject() => Provider.Delete(this);


    /// <summary>
    /// Updates the object using appropriate provider.
    /// </summary>
    protected override void SetObject() => Provider.Set(this);


    public AiraChatPromptGroupInfo()
        : base(TYPEINFO)
    {
    }


    public AiraChatPromptGroupInfo(DataRow dr)
        : base(TYPEINFO, dr)
    {
    }
}
