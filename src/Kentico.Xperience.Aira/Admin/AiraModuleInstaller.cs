using System.ComponentModel.DataAnnotations;

using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Membership;
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
        InstallAiraClass(
            resourceInfo,
            AiraConfigurationItemInfo.TYPEINFO.ObjectClassName,
            AiraConfigurationItemInfo.OBJECT_TYPE,
            classDisplayName: "Aira Configuration Item",
            typeof(AiraConfigurationItemInfo),
            nameof(AiraConfigurationItemInfo.AiraConfigurationItemId)
        );

        InstallAiraClass(
            resourceInfo,
            AiraChatPromptGroupInfo.TYPEINFO.ObjectClassName,
            AiraChatPromptGroupInfo.OBJECT_TYPE,
            classDisplayName: "Aira Chat Prompt Group",
            typeof(AiraChatPromptGroupInfo),
            nameof(AiraChatPromptGroupInfo.AiraChatPromptGroupId),
            [
                new FormFieldDependencyModel
                {
                    ReferenceType = ObjectDependencyEnum.Required,
                    ReferenceToObjectType = UserInfo.OBJECT_TYPE,
                    FormFieldName = nameof(AiraChatPromptGroupInfo.AiraChatPromptUserId)
                }
            ]
        );

        InstallAiraClass(
            resourceInfo,
            AiraChatPromptInfo.TYPEINFO.ObjectClassName,
            AiraChatPromptInfo.OBJECT_TYPE,
            classDisplayName: "Aira Chat Prompt",
            typeof(AiraChatPromptInfo),
            nameof(AiraChatPromptInfo.AiraChatPromptId),
            [
                new FormFieldDependencyModel
                {
                    ReferenceType = ObjectDependencyEnum.Required,
                    ReferenceToObjectType = AiraChatPromptGroupInfo.OBJECT_TYPE,
                    FormFieldName = nameof(AiraChatPromptInfo.AiraChatPromptChatPromptGroupId)
                }
            ]
        );

        InstallAiraClass(
            resourceInfo,
            AiraChatMessageInfo.TYPEINFO.ObjectClassName,
            AiraChatMessageInfo.OBJECT_TYPE,
            classDisplayName: "Aira Chat Message",
            typeof(AiraChatMessageInfo),
            nameof(AiraChatMessageInfo.AiraChatMessageId),
            [
                new FormFieldDependencyModel
                {
                    ReferenceType = ObjectDependencyEnum.Required,
                    ReferenceToObjectType = UserInfo.OBJECT_TYPE,
                    FormFieldName = nameof(AiraChatMessageInfo.AiraChatMessageUserId)
                }
            ]
        );

        InstallAiraClass(
            resourceInfo,
            AiraChatSummaryInfo.TYPEINFO.ObjectClassName,
            AiraChatSummaryInfo.OBJECT_TYPE,
            classDisplayName: "Aira Chat Summary",
            typeof(AiraChatSummaryInfo),
            nameof(AiraChatSummaryInfo.AiraChatSummaryId),
            [
                new FormFieldDependencyModel
                {
                    ReferenceType = ObjectDependencyEnum.Required,
                    ReferenceToObjectType = UserInfo.OBJECT_TYPE,
                    FormFieldName = nameof(AiraChatSummaryInfo.AiraChatSummaryUserId)
                }
            ]
        );
    }

    private static void InstallAiraClass(ResourceInfo resourceInfo,
        string objectClassName,
        string objectType,
        string classDisplayName,
        Type infoType,
        string idPropertyName,
        List<FormFieldDependencyModel>? dependencies = null)
    {
        var info = DataClassInfoProvider.GetDataClassInfo(objectType) ??
            DataClassInfo.New(objectType);

        info.ClassName = objectClassName;
        info.ClassTableName = objectClassName.Replace(".", "_");
        info.ClassDisplayName = classDisplayName;
        info.ClassResourceID = resourceInfo.ResourceID;
        info.ClassType = ClassType.OTHER;

        SetFormDefinition(info, infoType, idPropertyName, dependencies);
    }

    private sealed class FormFieldDependencyModel
    {
        public ObjectDependencyEnum ReferenceType { get; set; }
        public string FormFieldName { get; set; } = string.Empty;
        public string ReferenceToObjectType { get; set; } = string.Empty;
    }

    private static void SetFormDefinition(DataClassInfo info, Type infoType, string idPropertyName, List<FormFieldDependencyModel>? dependencies = null)
    {
        var formInfo = FormHelper.GetBasicFormDefinition(idPropertyName);

        formInfo = AddFormItems(formInfo, infoType, idPropertyName, dependencies);

        if (info.ClassID > 0)
        {
            var existingForm = new FormInfo(info.ClassFormDefinition);
            existingForm.CombineWithForm(formInfo, new());
            info.ClassFormDefinition = existingForm.GetXmlDefinition();
        }
        else
        {
            info.ClassFormDefinition = formInfo.GetXmlDefinition();
        }

        if (info.HasChanged)
        {
            DataClassInfoProvider.SetDataClassInfo(info);
        }
    }

    private static FormInfo AddFormItems(FormInfo formInfo, Type infoType, string idPropertyName, List<FormFieldDependencyModel>? dependencyProperties = null)
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

                if (dependencyProperties is not null)
                {
                    var dependency = dependencyProperties.FirstOrDefault(x => x.FormFieldName == property.Name);

                    if (dependency is not null)
                    {
                        formItem.ReferenceToObjectType = dependency.ReferenceToObjectType;
                        formItem.ReferenceType = dependency.ReferenceType;
                    }
                }

                formInfo.AddFormItem(formItem);
            }
        }

        return formInfo;
    }
}
