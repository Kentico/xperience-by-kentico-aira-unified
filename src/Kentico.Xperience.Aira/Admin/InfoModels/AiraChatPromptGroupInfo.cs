using System.ComponentModel.DataAnnotations;
using System.Data;

using CMS;
using CMS.DataEngine;

using CMS.Helpers;
using CMS.Membership;

using Kentico.Xperience.Aira.Admin.InfoModels;

[assembly: RegisterObjectType(typeof(AiraChatPromptGroupInfo), AiraChatPromptGroupInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.Aira.Admin.InfoModels;

/// <summary>
/// Data conainer class for <see cref="AiraChatPromptGroupInfo"/>.
/// </summary>
[Serializable]
internal class AiraChatPromptGroupInfo : AbstractInfo<AiraChatPromptGroupInfo, IInfoProvider<AiraChatPromptGroupInfo>>
{
    /// <summary>
    /// Object type.
    /// </summary>
    public const string OBJECT_TYPE = "kenticoaira.airachatpromptgroup";


    public static readonly ObjectTypeInfo TYPEINFO = new(typeof(IInfoProvider<AiraChatPromptGroupInfo>), OBJECT_TYPE, "KenticoAira.AiraChatPromptGroup", nameof(AiraChatPromptGroupId), null, nameof(AiraChatPromptGroupGuid), null, null, null, null, null)
    {
        TouchCacheDependencies = true,
        DependsOn =
        [
            new(nameof(AiraChatPromptUserId), UserInfo.OBJECT_TYPE, ObjectDependencyEnum.Required)
        ],
        ContinuousIntegrationSettings =
        {
            Enabled = true
        }
    };


    [DatabaseField]
    public virtual int AiraChatPromptGroupId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatPromptGroupId)), 0);
        set => SetValue(nameof(AiraChatPromptGroupId), value);
    }


    [DatabaseField]
    [Required]
    public virtual Guid AiraChatPromptGroupGuid
    {
        get => ValidationHelper.GetGuid(GetValue(nameof(AiraChatPromptGroupGuid)), Guid.Empty);
        set => SetValue(nameof(AiraChatPromptGroupGuid), value);
    }


    [DatabaseField]
    [Required]
    public virtual DateTime AiraChatPromptGroupCreatedWhen
    {
        get => ValidationHelper.GetDateTime(GetValue(nameof(AiraChatPromptGroupCreatedWhen)), DateTimeHelper.ZERO_TIME);
        set => SetValue(nameof(AiraChatPromptGroupCreatedWhen), value);
    }


    [DatabaseField]
    [Required]
    public virtual int AiraChatPromptUserId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatPromptUserId)), 0);
        set => SetValue(nameof(AiraChatPromptUserId), value);
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
