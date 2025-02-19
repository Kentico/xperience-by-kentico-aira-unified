using System.ComponentModel.DataAnnotations;

using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Membership;
using CMS.Modules;

using Kentico.Xperience.AiraUnified.Admin.InfoModels;

namespace Kentico.Xperience.AiraUnified.Admin;

internal class AiraUnifiedModuleInstaller : IAiraUnifiedModuleInstaller
{
    private readonly IInfoProvider<ResourceInfo> resourceInfoProvider;

    public AiraUnifiedModuleInstaller(IInfoProvider<ResourceInfo> resourceInfoProvider)
        => this.resourceInfoProvider = resourceInfoProvider;

    public void Install()
    {
        var resourceInfo = InstallModule();
        InstallModuleClasses(resourceInfo);
    }

    private ResourceInfo InstallModule()
    {
        var resourceInfo = resourceInfoProvider.Get(AiraUnifiedConstants.ResourceName)
            ?? new ResourceInfo();

        resourceInfo.ResourceDisplayName = AiraUnifiedConstants.ResourceDisplayName;
        resourceInfo.ResourceName = AiraUnifiedConstants.ResourceName;
        resourceInfo.ResourceDescription = AiraUnifiedConstants.ResourceDescription;
        resourceInfo.ResourceIsInDevelopment = AiraUnifiedConstants.ResourceIsInDevelopment;
        if (resourceInfo.HasChanged)
        {
            resourceInfoProvider.Set(resourceInfo);
        }

        return resourceInfo;
    }

    private static void InstallModuleClasses(ResourceInfo resourceInfo)
    {
        InstallAiraUnifiedClass(
            resourceInfo,
            AiraUnifiedConfigurationItemInfo.TYPEINFO.ObjectClassName,
            AiraUnifiedConfigurationItemInfo.OBJECT_TYPE,
            classDisplayName: "Aira Unified Configuration Item",
            typeof(AiraUnifiedConfigurationItemInfo),
            nameof(AiraUnifiedConfigurationItemInfo.AiraUnifiedConfigurationItemId)
        );

        InstallAiraUnifiedClass(
            resourceInfo,
            AiraUnifiedChatPromptGroupInfo.TYPEINFO.ObjectClassName,
            AiraUnifiedChatPromptGroupInfo.OBJECT_TYPE,
            classDisplayName: "Aira Unified Chat Prompt Group",
            typeof(AiraUnifiedChatPromptGroupInfo),
            nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupId),
            [
                new FormFieldDependencyModel
                {
                    ReferenceType = ObjectDependencyEnum.Required,
                    ReferenceToObjectType = UserInfo.OBJECT_TYPE,
                    FormFieldName = nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptUserId)
                }
            ]
        );

        InstallAiraUnifiedClass(
            resourceInfo,
            AiraUnifiedChatPromptInfo.TYPEINFO.ObjectClassName,
            AiraUnifiedChatPromptInfo.OBJECT_TYPE,
            classDisplayName: "Aira Unified Chat Prompt",
            typeof(AiraUnifiedChatPromptInfo),
            nameof(AiraUnifiedChatPromptInfo.AiraUnifiedChatPromptId),
            [
                new FormFieldDependencyModel
                {
                    ReferenceType = ObjectDependencyEnum.Required,
                    ReferenceToObjectType = AiraUnifiedChatPromptGroupInfo.OBJECT_TYPE,
                    FormFieldName = nameof(AiraUnifiedChatPromptInfo.AiraUnifiedChatPromptChatPromptGroupId)
                }
            ]
        );

        InstallAiraUnifiedClass(
            resourceInfo,
            AiraUnifiedChatMessageInfo.TYPEINFO.ObjectClassName,
            AiraUnifiedChatMessageInfo.OBJECT_TYPE,
            classDisplayName: "Aira Unified Chat Message",
            typeof(AiraUnifiedChatMessageInfo),
            nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageId),
            [
                new FormFieldDependencyModel
                {
                    ReferenceType = ObjectDependencyEnum.Required,
                    ReferenceToObjectType = UserInfo.OBJECT_TYPE,
                    FormFieldName = nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageUserId)
                }
            ]
        );

        InstallAiraUnifiedClass(
            resourceInfo,
            AiraUnifiedChatSummaryInfo.TYPEINFO.ObjectClassName,
            AiraUnifiedChatSummaryInfo.OBJECT_TYPE,
            classDisplayName: "Aira Unified Chat Summary",
            typeof(AiraUnifiedChatSummaryInfo),
            nameof(AiraUnifiedChatSummaryInfo.AiraUnifiedChatSummaryId),
            [
                new FormFieldDependencyModel
                {
                    ReferenceType = ObjectDependencyEnum.Required,
                    ReferenceToObjectType = UserInfo.OBJECT_TYPE,
                    FormFieldName = nameof(AiraUnifiedChatSummaryInfo.AiraUnifiedChatSummaryUserId)
                }
            ]
        );
    }

    private static void InstallAiraUnifiedClass(ResourceInfo resourceInfo,
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
