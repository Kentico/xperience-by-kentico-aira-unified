using System.ComponentModel.DataAnnotations;
using System.Data;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;

using Kentico.Xperience.Aira.Admin.InfoModels;

[assembly: RegisterObjectType(typeof(AiraChatPromptInfo), AiraChatPromptInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.Aira.Admin.InfoModels;

/// <summary>
/// Data conainer class for <see cref="AiraChatPromptInfo"/>.
/// </summary>
public partial class AiraChatPromptInfo : AbstractInfo<AiraChatPromptInfo, IInfoProvider<AiraChatPromptInfo>>
{
    /// <summary>
    /// Object type.
    /// </summary>
    public const string OBJECT_TYPE = "kenticoaira.airachatprompt";


    /// <summary>
    /// Type information.
    /// </summary>
    public static readonly ObjectTypeInfo TYPEINFO = new(typeof(IInfoProvider<AiraChatPromptInfo>), OBJECT_TYPE, "KenticoAira.AiraChatPrompt", nameof(AiraChatPromptId), null, nameof(AiraChatPromptGuid), null, null, null, null, null)
    {
        TouchCacheDependencies = true,
        DependsOn =
        [
            new(nameof(AiraChatPromptChatPromptGroupId), AiraChatPromptGroupInfo.OBJECT_TYPE, ObjectDependencyEnum.Required)
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
    public virtual int AiraChatPromptId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatPromptId)), 0);
        set => SetValue(nameof(AiraChatPromptId), value);
    }


    /// <summary>
    /// Chat prompt guid.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual Guid AiraChatPromptGuid
    {
        get => ValidationHelper.GetGuid(GetValue(nameof(AiraChatPromptGuid)), Guid.Empty);
        set => SetValue(nameof(AiraChatPromptGuid), value);
    }


    /// <summary>
    /// Chat prompt group id.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual int AiraChatPromptChatPromptGroupId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatPromptChatPromptGroupId)), 0);
        set => SetValue(nameof(AiraChatPromptChatPromptGroupId), value);
    }


    /// <summary>
    /// Chat prompt text.
    /// </summary>
    [DatabaseField]
    public virtual string AiraChatPromptText
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraChatPromptText)), string.Empty);
        set => SetValue(nameof(AiraChatPromptText), value);
    }


    /// <summary>
    /// Deletes the object using appropriate provider.
    /// </summary>
    protected override void DeleteObject() => Provider.Delete(this);


    /// <summary>
    /// Updates the object using appropriate provider.
    /// </summary>
    protected override void SetObject() => Provider.Set(this);


    public AiraChatPromptInfo()
        : base(TYPEINFO)
    {
    }


    public AiraChatPromptInfo(DataRow dr)
        : base(TYPEINFO, dr)
    {
    }
}
