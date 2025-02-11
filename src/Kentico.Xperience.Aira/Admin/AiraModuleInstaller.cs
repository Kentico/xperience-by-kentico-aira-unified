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
    {
        InstallAiraConfigurationClass(resourceInfo);
        InstallAiraChatPromptClass(resourceInfo);
        InstallAiraChatPromptGroupClass(resourceInfo);
        InstallAiraChatMessageClass(resourceInfo);
    }

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

        formInfo = AddFormItems(formInfo, typeof(AiraConfigurationItemInfo), nameof(AiraConfigurationItemInfo.AiraConfigurationItemId));

        SetFormDefinition(info, formInfo);

        if (info.HasChanged)
        {
            DataClassInfoProvider.SetDataClassInfo(info);
        }
    }

    private static void InstallAiraChatPromptGroupClass(ResourceInfo resourceInfo)
    {
        var info = DataClassInfoProvider.GetDataClassInfo(AiraChatPromptGroupInfo.OBJECT_TYPE) ??
            DataClassInfo.New(AiraChatPromptGroupInfo.OBJECT_TYPE);

        info.ClassName = AiraChatPromptGroupInfo.TYPEINFO.ObjectClassName;
        info.ClassTableName = AiraChatPromptGroupInfo.TYPEINFO.ObjectClassName.Replace(".", "_");
        info.ClassDisplayName = "Aira Chat Prompt Group";
        info.ClassResourceID = resourceInfo.ResourceID;
        info.ClassType = ClassType.OTHER;
        var formInfo = FormHelper.GetBasicFormDefinition(nameof(AiraChatPromptGroupInfo.AiraChatPromptGroupId));

        formInfo = AddFormItems(formInfo, typeof(AiraChatPromptGroupInfo), nameof(AiraChatPromptGroupInfo.AiraChatPromptGroupId));

        SetFormDefinition(info, formInfo);

        if (info.HasChanged)
        {
            DataClassInfoProvider.SetDataClassInfo(info);
        }
    }

    private static void InstallAiraChatPromptClass(ResourceInfo resourceInfo)
    {
        var info = DataClassInfoProvider.GetDataClassInfo(AiraChatPromptInfo.OBJECT_TYPE) ??
            DataClassInfo.New(AiraChatPromptInfo.OBJECT_TYPE);

        info.ClassName = AiraChatPromptInfo.TYPEINFO.ObjectClassName;
        info.ClassTableName = AiraChatPromptInfo.TYPEINFO.ObjectClassName.Replace(".", "_");
        info.ClassDisplayName = "Aira Chat Prompt";
        info.ClassResourceID = resourceInfo.ResourceID;
        info.ClassType = ClassType.OTHER;
        var formInfo = FormHelper.GetBasicFormDefinition(nameof(AiraChatPromptInfo.AiraChatPromptId));

        formInfo = AddFormItems(formInfo, typeof(AiraChatPromptInfo), nameof(AiraChatPromptInfo.AiraChatPromptId));

        SetFormDefinition(info, formInfo);

        if (info.HasChanged)
        {
            DataClassInfoProvider.SetDataClassInfo(info);
        }
    }

    private static void InstallAiraChatMessageClass(ResourceInfo resourceInfo)
    {
        var info = DataClassInfoProvider.GetDataClassInfo(AiraChatMessageInfo.OBJECT_TYPE) ??
            DataClassInfo.New(AiraChatMessageInfo.OBJECT_TYPE);

        info.ClassName = AiraChatMessageInfo.TYPEINFO.ObjectClassName;
        info.ClassTableName = AiraChatMessageInfo.TYPEINFO.ObjectClassName.Replace(".", "_");
        info.ClassDisplayName = "Aira Chat Message";
        info.ClassResourceID = resourceInfo.ResourceID;
        info.ClassType = ClassType.OTHER;
        var formInfo = FormHelper.GetBasicFormDefinition(nameof(AiraChatMessageInfo.AiraChatMessageId));

        formInfo = AddFormItems(formInfo, typeof(AiraChatMessageInfo), nameof(AiraChatMessageInfo.AiraChatMessageId));

        SetFormDefinition(info, formInfo);

        if (info.HasChanged)
        {
            DataClassInfoProvider.SetDataClassInfo(info);
        }
    }

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
    private static FormInfo AddFormItems(FormInfo formInfo, Type infoType, string idPropertyName)
    {
        var properties = infoType.GetProperties();

        foreach (var property in properties)
        {
            if (string.Equals(property.Name, idPropertyName))
            {
                continue; // Exclude Id from the loop
            }

            if (property.GetCustomAttributes(typeof(DatabaseFieldAttribute), true).FirstOrDefault() is DatabaseFieldAttribute)
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
