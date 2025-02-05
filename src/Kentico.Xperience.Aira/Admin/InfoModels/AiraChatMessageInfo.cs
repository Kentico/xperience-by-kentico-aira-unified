using System.ComponentModel.DataAnnotations;
using System.Data;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;

using Kentico.Xperience.Aira.Admin.InfoModels;

[assembly: RegisterObjectType(typeof(AiraChatMessageInfo), AiraChatMessageInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.Aira.Admin.InfoModels;

public class AiraChatMessageInfo : AbstractInfo<AiraChatMessageInfo, IInfoProvider<AiraChatMessageInfo>>
{
    /// <summary>
    /// Object type.
    /// </summary>
    public const string OBJECT_TYPE = "kenticoaira.airachatmessage";


    /// <summary>
    /// Type information.
    /// </summary>
    public static readonly ObjectTypeInfo TYPEINFO = new(typeof(IInfoProvider<AiraChatMessageInfo>), OBJECT_TYPE, "KenticoAira.AiraChatMessage", nameof(AiraChatMessageId), null, nameof(AiraChatMessageGuid), null, null, null, null, null)
    {
        TouchCacheDependencies = true,
        ContinuousIntegrationSettings =
        {
            Enabled = false,
        }
    };


    /// <summary>
    /// Chat message id.
    /// </summary>
    [DatabaseField]
    public virtual int AiraChatMessageId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatMessageId)), 0);
        set => SetValue(nameof(AiraChatMessageId), value);
    }


    /// <summary>
    /// Chat message guid.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual Guid AiraChatMessageGuid
    {
        get => ValidationHelper.GetGuid(GetValue(nameof(AiraChatMessageGuid)), Guid.Empty);
        set => SetValue(nameof(AiraChatMessageGuid), value);
    }


    /// <summary>
    /// Chat message cration time.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual DateTime AiraChatMessageCreatedWhen
    {
        get => ValidationHelper.GetDateTime(GetValue(nameof(AiraChatMessageCreatedWhen)), DateTimeHelper.ZERO_TIME);
        set => SetValue(nameof(AiraChatMessageCreatedWhen), value);
    }


    /// <summary>
    /// Chat message user id.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual int AiraChatMessageUserId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatMessageUserId)), 0);
        set => SetValue(nameof(AiraChatMessageUserId), value);
    }


    /// <summary>
    /// Chat message text.
    /// </summary>
    [DatabaseField]
    public virtual string AiraChatMessageText
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraChatMessageText)), string.Empty);
        set => SetValue(nameof(AiraChatMessageText), value);
    }


    /// <summary>
    /// Chat message role.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual int AiraChatMessageRole
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatMessageRole)), 0);
        set => SetValue(nameof(AiraChatMessageRole), value);
    }


    /// <summary>
    /// Deletes the object using appropriate provider.
    /// </summary>
    protected override void DeleteObject() => Provider.Delete(this);


    /// <summary>
    /// Updates the object using appropriate provider.
    /// </summary>
    protected override void SetObject() => Provider.Set(this);


    public AiraChatMessageInfo()
        : base(TYPEINFO)
    {
    }


    public AiraChatMessageInfo(DataRow dr)
        : base(TYPEINFO, dr)
    {
    }
}
