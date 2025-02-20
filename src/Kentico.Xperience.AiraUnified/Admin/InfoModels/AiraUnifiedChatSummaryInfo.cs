using System.ComponentModel.DataAnnotations;
using System.Data;

using CMS;
using CMS.DataEngine;

using CMS.Helpers;
using CMS.Membership;

using Kentico.Xperience.AiraUnified.Admin.InfoModels;

[assembly: RegisterObjectType(typeof(AiraUnifiedChatSummaryInfo), AiraUnifiedChatSummaryInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.AiraUnified.Admin.InfoModels;

public partial class AiraUnifiedChatSummaryInfo : AbstractInfo<AiraUnifiedChatSummaryInfo, IInfoProvider<AiraUnifiedChatSummaryInfo>>
{
    /// <summary>
    /// Object type.
    /// </summary>
    public const string OBJECT_TYPE = "kenticoairaunified.airaunifiedchatsummary";


    /// <summary>
    /// Type information.
    /// </summary>
    public static readonly ObjectTypeInfo TYPEINFO = new(typeof(IInfoProvider<AiraUnifiedChatSummaryInfo>), OBJECT_TYPE, "KenticoAiraUnified.AiraUnifiedChatSummary", nameof(AiraUnifiedChatSummaryId), null, nameof(AiraUnifiedChatSummaryGuid), null, null, null, null, null)
    {
        TouchCacheDependencies = true,
        DependsOn =
        [
            new(nameof(AiraUnifiedChatSummaryUserId), UserInfo.OBJECT_TYPE, ObjectDependencyEnum.Required)
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
    public virtual int AiraUnifiedChatSummaryId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraUnifiedChatSummaryId)), 0);
        set => SetValue(nameof(AiraUnifiedChatSummaryId), value);
    }


    /// <summary>
    /// Chat summary guid.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual Guid AiraUnifiedChatSummaryGuid
    {
        get => ValidationHelper.GetGuid(GetValue(nameof(AiraUnifiedChatSummaryGuid)), Guid.Empty);
        set => SetValue(nameof(AiraUnifiedChatSummaryGuid), value);
    }


    /// <summary>
    /// Chat summary content.
    /// </summary>
    [DatabaseField]
    public virtual string AiraUnifiedChatSummaryContent
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraUnifiedChatSummaryContent)), string.Empty);
        set => SetValue(nameof(AiraUnifiedChatSummaryContent), value);
    }


    /// <summary>
    /// Chat summary user id.
    /// </summary>
    [DatabaseField]
    public virtual int AiraUnifiedChatSummaryUserId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraUnifiedChatSummaryUserId)), 0);
        set => SetValue(nameof(AiraUnifiedChatSummaryUserId), value);
    }


    /// <summary>
    /// Deletes the object using appropriate provider.
    /// </summary>
    protected override void DeleteObject() => Provider.Delete(this);


    /// <summary>
    /// Updates the object using appropriate provider.
    /// </summary>
    protected override void SetObject() => Provider.Set(this);


    public AiraUnifiedChatSummaryInfo()
        : base(TYPEINFO)
    {
    }


    public AiraUnifiedChatSummaryInfo(DataRow dr)
        : base(TYPEINFO, dr)
    {
    }
}
