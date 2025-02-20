using System.ComponentModel.DataAnnotations;
using System.Data;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;

using Kentico.Xperience.AiraUnified.Admin.InfoModels;

[assembly: RegisterObjectType(typeof(AiraUnifiedChatMessageInfo), AiraUnifiedChatMessageInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.AiraUnified.Admin.InfoModels;

public class AiraUnifiedChatMessageInfo : AbstractInfo<AiraUnifiedChatMessageInfo, IInfoProvider<AiraUnifiedChatMessageInfo>>
{
    /// <summary>
    /// Object type.
    /// </summary>
    public const string OBJECT_TYPE = "kenticoairaunified.airaunifiedchatmessage";


    /// <summary>
    /// Type information.
    /// </summary>
    public static readonly ObjectTypeInfo TYPEINFO = new(typeof(IInfoProvider<AiraUnifiedChatMessageInfo>), OBJECT_TYPE, "KenticoAiraUnified.AiraUnifiedChatMessage", nameof(AiraUnifiedChatMessageId), null, nameof(AiraUnifiedChatMessageGuid), null, null, null, null, null)
    {
        TouchCacheDependencies = true,
        DependsOn =
        [
            new(nameof(AiraUnifiedChatMessageUserId), UserInfo.OBJECT_TYPE, ObjectDependencyEnum.Required)
        ],
        ContinuousIntegrationSettings =
        {
            Enabled = false,
        }
    };


    /// <summary>
    /// Chat message id.
    /// </summary>
    [DatabaseField]
    public virtual int AiraUnifiedChatMessageId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraUnifiedChatMessageId)), 0);
        set => SetValue(nameof(AiraUnifiedChatMessageId), value);
    }


    /// <summary>
    /// Chat message guid.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual Guid AiraUnifiedChatMessageGuid
    {
        get => ValidationHelper.GetGuid(GetValue(nameof(AiraUnifiedChatMessageGuid)), Guid.Empty);
        set => SetValue(nameof(AiraUnifiedChatMessageGuid), value);
    }


    /// <summary>
    /// Chat message creation time.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual DateTime AiraUnifiedChatMessageCreatedWhen
    {
        get => ValidationHelper.GetDateTime(GetValue(nameof(AiraUnifiedChatMessageCreatedWhen)), DateTimeHelper.ZERO_TIME);
        set => SetValue(nameof(AiraUnifiedChatMessageCreatedWhen), value);
    }


    /// <summary>
    /// Chat message user id.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual int AiraUnifiedChatMessageUserId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraUnifiedChatMessageUserId)), 0);
        set => SetValue(nameof(AiraUnifiedChatMessageUserId), value);
    }


    /// <summary>
    /// Chat message text.
    /// </summary>
    [DatabaseField]
    public virtual string AiraUnifiedChatMessageText
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraUnifiedChatMessageText)), string.Empty);
        set => SetValue(nameof(AiraUnifiedChatMessageText), value);
    }


    /// <summary>
    /// Chat message role.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual int AiraUnifiedChatMessageRole
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraUnifiedChatMessageRole)), 0);
        set => SetValue(nameof(AiraUnifiedChatMessageRole), value);
    }


    /// <summary>
    /// Deletes the object using appropriate provider.
    /// </summary>
    protected override void DeleteObject() => Provider.Delete(this);


    /// <summary>
    /// Updates the object using appropriate provider.
    /// </summary>
    protected override void SetObject() => Provider.Set(this);


    public AiraUnifiedChatMessageInfo()
        : base(TYPEINFO)
    {
    }


    public AiraUnifiedChatMessageInfo(DataRow dr)
        : base(TYPEINFO, dr)
    {
    }
}
