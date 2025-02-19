using System.Text.RegularExpressions;

using CMS.DataEngine;
using CMS.Membership;

using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.Aira.Admin.InfoModels;
using Kentico.Xperience.Aira.Admin.UIPages;

[assembly: UIPage(
    parentType: typeof(AiraApplicationPage),
    slug: "configuration",
    uiPageType: typeof(AiraConfigurationEditPage),
    name: "Edit configuration",
    templateName: TemplateNames.EDIT,
    order: UIPageOrder.NoOrder)]

namespace Kentico.Xperience.Aira.Admin.UIPages;

[UIEvaluatePermission(SystemPermissions.UPDATE)]
internal class AiraConfigurationEditPage : ModelEditPage<AiraConfigurationModel>
{
    private AiraConfigurationModel? model = null;
    private readonly IAiraConfigurationService airaConfigurationService;
    private readonly IInfoProvider<AiraConfigurationItemInfo> airaConfigurationProvider;

    public AiraConfigurationEditPage(Xperience.Admin.Base.Forms.Internal.IFormItemCollectionProvider formItemCollectionProvider,
        IFormDataBinder formDataBinder,
        IAiraConfigurationService airaConfigurationService,
        IInfoProvider<AiraConfigurationItemInfo> airaConfigurationProvider)
        : base(formItemCollectionProvider, formDataBinder)
    {
        this.airaConfigurationService = airaConfigurationService;
        this.airaConfigurationProvider = airaConfigurationProvider;
    }

    protected override AiraConfigurationModel Model
    {
        get
        {
            var configurationInfo = airaConfigurationProvider.Get().TopN(2).GetEnumerableTypedResult().SingleOrDefault();

            model ??= new AiraConfigurationModel(configurationInfo ?? new());

            return model;
        }
    }

    protected override async Task<ICommandResponse> ProcessFormData(AiraConfigurationModel model, ICollection<IFormItem> formItems)
    {
        if (!IsValidSubpath(model.RelativePathBase))
        {
            return ResponseFrom(new FormSubmissionResult(
                FormSubmissionStatus.ValidationFailure
            )).AddErrorMessage($"{model.RelativePathBase} is not a valid path.");
        }

        var result = await airaConfigurationService.TrySaveOrUpdateConfiguration(model);

        var response = ResponseFrom(new FormSubmissionResult(
            result
            ? FormSubmissionStatus.ValidationSuccess
            : FormSubmissionStatus.ValidationFailure
        ));

        if (result)
        {
            response.AddSuccessMessage("Aira configuration updated.");
        }
        else
        {
            response.AddErrorMessage("Could not update aira configuration.");
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
