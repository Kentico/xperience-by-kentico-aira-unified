using System.ComponentModel.DataAnnotations;
using System.Data;

using CMS;
using CMS.DataEngine;

using CMS.Helpers;
using CMS.Membership;

using Kentico.Xperience.Aira.Admin.InfoModels;

[assembly: RegisterObjectType(typeof(AiraChatSummaryInfo), AiraChatSummaryInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.Aira.Admin.InfoModels;

[Serializable]
public partial class AiraChatSummaryInfo : AbstractInfo<AiraChatSummaryInfo, IInfoProvider<AiraChatSummaryInfo>>
{
    /// <summary>
    /// Object type.
    /// </summary>
    public const string OBJECT_TYPE = "kenticoaira.airachatsummary";


    /// <summary>
    /// Type information.
    /// </summary>
    public static readonly ObjectTypeInfo TYPEINFO = new(typeof(IInfoProvider<AiraChatSummaryInfo>), OBJECT_TYPE, "KenticoAira.AiraChatSummary", nameof(AiraChatSummaryId), null, nameof(AiraChatSummaryGuid), null, null, null, null, null)
    {
        TouchCacheDependencies = true,
        DependsOn =
        [
            new(nameof(AiraChatSummaryUserId), UserInfo.OBJECT_TYPE, ObjectDependencyEnum.Required)
        ],
        ContinuousIntegrationSettings =
        {
            Enabled = false
        }
    };


    /// <summary>
    /// Chat summary id.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual int AiraChatSummaryId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatSummaryId)), 0);
        set => SetValue(nameof(AiraChatSummaryId), value);
    }


    /// <summary>
    /// Chat summary guid.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual Guid AiraChatSummaryGuid
    {
        get => ValidationHelper.GetGuid(GetValue(nameof(AiraChatSummaryGuid)), Guid.Empty);
        set => SetValue(nameof(AiraChatSummaryGuid), value);
    }


    /// <summary>
    /// Chat summary content.
    /// </summary>
    [DatabaseField]
    public virtual string AiraChatSummaryContent
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraChatSummaryContent)), string.Empty);
        set => SetValue(nameof(AiraChatSummaryContent), value);
    }


    /// <summary>
    /// Chat summary user id.
    /// </summary>
    [DatabaseField]
    public virtual int AiraChatSummaryUserId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraChatSummaryUserId)), 0);
        set => SetValue(nameof(AiraChatSummaryUserId), value);
    }


    /// <summary>
    /// Deletes the object using appropriate provider.
    /// </summary>
    protected override void DeleteObject() => Provider.Delete(this);


    /// <summary>
    /// Updates the object using appropriate provider.
    /// </summary>
    protected override void SetObject() => Provider.Set(this);


    public AiraChatSummaryInfo()
        : base(TYPEINFO)
    {
    }


    public AiraChatSummaryInfo(DataRow dr)
        : base(TYPEINFO, dr)
    {
    }
}
