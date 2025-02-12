using System.ComponentModel.DataAnnotations;
using System.Data;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;

using Kentico.Xperience.Aira.Admin.InfoModels;


[assembly: RegisterObjectType(typeof(AiraChatThreadInfo), AiraChatThreadInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.Aira.Admin.InfoModels;

/// <summary>
/// Data container class for <see cref="AiraChatThreadInfo"/>.
/// </summary>
public class AiraChatThreadInfo : AbstractInfo<AiraChatThreadInfo, IInfoProvider<AiraChatThreadInfo>>
{
    /// <summary>
    /// Object type.
    /// </summary>
    public const string OBJECT_TYPE = "kenticoaira.airachatthread";


    /// <summary>
    /// Type information.
    /// </summary>
    public static readonly ObjectTypeInfo TYPEINFO = new(typeof(IInfoProvider<AiraChatThreadInfo>), OBJECT_TYPE, "KenticoAira.AiraChatThread", nameof(AiraChatThreadId), null, nameof(AiraChatThreadGuid), null, null, null, null, null)
    {
        TouchCacheDependencies = true,
        DependsOn =
        [
            new(nameof(AiraChatThreadUserId), UserInfo.OBJECT_TYPE, ObjectDependencyEnum.Required)
        ],
        ContinuousIntegrationSettings =
        {
            Enabled = false
        }
    };


    /// <summary>
    /// Chat thread id.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual int AiraChatThreadId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatThreadId)), 0);
        set => SetValue(nameof(AiraChatThreadId), value);
    }


    /// <summary>
    /// Admin application user id.
    /// </summary>
    public virtual int AiraChatThreadUserId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatThreadUserId)), 0);
        set => SetValue(nameof(AiraChatThreadUserId), value);
    }


    /// <summary>
    /// Chat thread guid.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual Guid AiraChatThreadGuid
    {
        get => ValidationHelper.GetGuid(GetValue(nameof(AiraChatThreadGuid)), Guid.Empty);
        set => SetValue(nameof(AiraChatThreadGuid), value);
    }


    /// <summary>
    /// Chat thread name.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual string AiraChatThreadName
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraChatThreadName)), string.Empty);
        set => SetValue(nameof(AiraChatThreadName), value);
    }


    /// <summary>
    /// Is chat thread the latest used.
    /// </summary>
    [DatabaseField]
    public bool AiraChatThreadIsLatest
    {
        get => ValidationHelper.GetBoolean(GetValue(nameof(AiraChatThreadIsLatest)), false);
        set => SetValue(nameof(AiraChatThreadIsLatest), value);
    }


    /// <summary>
    /// Deletes the object using appropriate provider.
    /// </summary>
    protected override void DeleteObject() => Provider.Delete(this);


    /// <summary>
    /// Updates the object using appropriate provider.
    /// </summary>
    protected override void SetObject() => Provider.Set(this);


    public AiraChatThreadInfo()
        : base(TYPEINFO)
    {
    }


    public AiraChatThreadInfo(DataRow dr)
        : base(TYPEINFO, dr)
    {
    }
}
