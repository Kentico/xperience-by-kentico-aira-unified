using System.Text.RegularExpressions;

using CMS.DataEngine;
using CMS.Membership;

using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.AiraUnified.Admin.InfoModels;
using Kentico.Xperience.AiraUnified.Admin.UIPages;

[assembly: UIPage(
    parentType: typeof(AiraUnifiedApplicationPage),
    slug: "configuration",
    uiPageType: typeof(AiraUnifiedConfigurationEditPage),
    name: "Edit configuration",
    templateName: TemplateNames.EDIT,
    order: UIPageOrder.NoOrder)]

namespace Kentico.Xperience.AiraUnified.Admin.UIPages;

[UIEvaluatePermission(SystemPermissions.UPDATE)]
internal class AiraUnifiedConfigurationEditPage : ModelEditPage<AiraUnifiedConfigurationModel>
{
    private AiraUnifiedConfigurationModel? model = null;
    private readonly IAiraUnifiedConfigurationService airaUnifiedConfigurationService;
    private readonly IInfoProvider<AiraUnifiedConfigurationItemInfo> airaUnifiedConfigurationProvider;

    public AiraUnifiedConfigurationEditPage(Xperience.Admin.Base.Forms.Internal.IFormItemCollectionProvider formItemCollectionProvider,
        IFormDataBinder formDataBinder,
        IAiraUnifiedConfigurationService airaUnifiedConfigurationService,
        IInfoProvider<AiraUnifiedConfigurationItemInfo> airaUnifiedConfigurationProvider)
        : base(formItemCollectionProvider, formDataBinder)
    {
        this.airaUnifiedConfigurationService = airaUnifiedConfigurationService;
        this.airaUnifiedConfigurationProvider = airaUnifiedConfigurationProvider;
    }

    protected override AiraUnifiedConfigurationModel Model
    {
        get
        {
            var configurationInfo = airaUnifiedConfigurationProvider.Get().TopN(2).GetEnumerableTypedResult().SingleOrDefault();

            model ??= new AiraUnifiedConfigurationModel(configurationInfo ?? new());

            return model;
        }
    }

    protected override async Task<ICommandResponse> ProcessFormData(AiraUnifiedConfigurationModel model, ICollection<IFormItem> formItems)
    {
        if (!IsValidSubpath(model.RelativePathBase))
        {
            return ResponseFrom(new FormSubmissionResult(
                FormSubmissionStatus.ValidationFailure
            )).AddErrorMessage($"{model.RelativePathBase} is not a valid path.");
        }

        var result = await airaUnifiedConfigurationService.TrySaveOrUpdateConfiguration(model);

        var response = ResponseFrom(new FormSubmissionResult(
            result
            ? FormSubmissionStatus.ValidationSuccess
            : FormSubmissionStatus.ValidationFailure
        ));

        if (result)
        {
            response.AddSuccessMessage("Aira unified configuration updated.");
        }
        else
        {
            response.AddErrorMessage("Could not update aira unified configuration.");
        }

        return response;
    }

    private static bool IsValidSubpath(string path)
    {
        if (string.IsNullOrEmpty(path) || path[0] != '/' || (path.Length > 1 && path[1] == '/'))
        {
            return false;
        }

        var pattern = @"^\/[a-zA-Z0-9-_\/]+$";

        if (!Regex.IsMatch(path, pattern))
        {
            return false;
        }

        if (path.EndsWith('/'))
        {
            return false;
        }

        return true;
    }
}
