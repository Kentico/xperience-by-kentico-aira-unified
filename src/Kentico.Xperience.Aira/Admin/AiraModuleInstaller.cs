using System.ComponentModel.DataAnnotations;

using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Modules;

using Kentico.Xperience.Aira.Admin.InfoModels;

namespace Kentico.Xperience.Aira.Admin;

internal class AiraModuleInstaller : IAiraModuleInstaller
{
    private readonly IInfoProvider<ResourceInfo> resourceInfoProvider;

    public AiraModuleInstaller(IInfoProvider<ResourceInfo> resourceInfoProvider)
        => this.resourceInfoProvider = resourceInfoProvider;

    public void Install()
    {
        var resourceInfo = InstallModule();
        InstallModuleClasses(resourceInfo);
    }

    private ResourceInfo InstallModule()
    {
        var resourceInfo = resourceInfoProvider.Get(AiraCompanionAppConstants.ResourceName)
            ?? new ResourceInfo();

        resourceInfo.ResourceDisplayName = AiraCompanionAppConstants.ResourceDisplayName;
        resourceInfo.ResourceName = AiraCompanionAppConstants.ResourceName;
        resourceInfo.ResourceDescription = AiraCompanionAppConstants.ResourceDescription;
        resourceInfo.ResourceIsInDevelopment = AiraCompanionAppConstants.ResourceIsInDevelopment;
        if (resourceInfo.HasChanged)
        {
            resourceInfoProvider.Set(resourceInfo);
        }

        return resourceInfo;
    }

    private static void InstallModuleClasses(ResourceInfo resourceInfo)
        => InstallAiraConfigurationClass(resourceInfo);

    private static void InstallAiraConfigurationClass(ResourceInfo resourceInfo)
    {
        var info = DataClassInfoProvider.GetDataClassInfo(AiraConfigurationItemInfo.OBJECT_TYPE) ??
            DataClassInfo.New(AiraConfigurationItemInfo.OBJECT_TYPE);

        info.ClassName = AiraConfigurationItemInfo.TYPEINFO.ObjectClassName;
        info.ClassTableName = AiraConfigurationItemInfo.TYPEINFO.ObjectClassName.Replace(".", "_");
        info.ClassDisplayName = "Aira Configuration Item";
        info.ClassResourceID = resourceInfo.ResourceID;
        info.ClassType = ClassType.OTHER;
        var formInfo = FormHelper.GetBasicFormDefinition(nameof(AiraConfigurationItemInfo.AiraConfigurationItemId));

        formInfo = AddFormItems(formInfo);

        SetFormDefinition(info, formInfo);

        if (info.HasChanged)
        {
            DataClassInfoProvider.SetDataClassInfo(info);
        }
    }

    /// <summary>
    /// Ensure that the form is not upserted with any existing form
    /// </summary>
    /// <param name="info"></param>
    /// <param name="form"></param>
    private static void SetFormDefinition(DataClassInfo info, FormInfo form)
    {
        if (info.ClassID > 0)
        {
            var existingForm = new FormInfo(info.ClassFormDefinition);
            existingForm.CombineWithForm(form, new());
            info.ClassFormDefinition = existingForm.GetXmlDefinition();
        }
        else
        {
            info.ClassFormDefinition = form.GetXmlDefinition();
        }
    }

    /// <summary>
    /// Loop through all AiraConfigurationItemInfo properties and add them as Form Items
    /// </summary>
    private static FormInfo AddFormItems(FormInfo formInfo)
    {
        var airaConfigurationItemInfoType = typeof(AiraConfigurationItemInfo);
        var properties = airaConfigurationItemInfoType.GetProperties();

        foreach (var property in properties)
        {
            if (property.Name == nameof(AiraConfigurationItemInfo.AiraConfigurationItemId))
            {
                continue; // Exclude AiraConfigurationItemId from the loop
            }

            if (property.GetCustomAttributes(typeof(DatabaseFieldAttribute), true).FirstOrDefault() is DatabaseFieldAttribute databaseFieldAttribute)
            {
                var formItem = new FormFieldInfo()
                {
                    Name = property.Name,
                    Visible = true,
                    DataType = FieldDataType.Text,
                    Enabled = true,
                    AllowEmpty = !property.IsDefined(typeof(RequiredAttribute), true) // Set AllowEmpty to true if the property has the Required attribute
                };

                // Map the property type to the appropriate FieldDataType
                formItem.DataType = property.PropertyType switch
                {
                    Type t when t == typeof(string) => FieldDataType.Text,
                    Type t when t == typeof(int) => FieldDataType.Integer,
                    Type t when t == typeof(DateTime) => FieldDataType.DateTime,
                    _ => formItem.DataType // Default case if no match is found
                };

                formInfo.AddFormItem(formItem);
            }
        }

        return formInfo;
    }
}
