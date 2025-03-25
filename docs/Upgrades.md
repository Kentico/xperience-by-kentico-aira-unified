# Upgrades

## 0.2.0.prerelease-2 -> 0.3.0-prerelease-3

The following core custom module table has been changed.

- `KenticoAiraUnified_AiraUnifiedChatPromptGroup`

Installing the v0.3.0.prerelease-3 NuGet package will automatically install the new custom module classes and create their tables.
However, it will not migrate the data associated with the generated quick prompts.
To ensure the module functions properly, the old prompt-related tables must be removed.

You can clean up the old tables by running the following SQL:

```sql
drop table KenticoAiraUnified_AiraUnifiedChatPromptGroup
drop table KenticoAiraUnified_AiraUnifiedChatPrompt

delete
FROM [dbo].[CMS_Class] where ClassName in ('KenticoAiraUnified.AiraUnifiedChatPromptGroup', 'KenticoAiraUnified.AiraUnifiedChatPrompt')
```

## Uninstall

This integration programmatically inserts custom module classes and their configuration into the Xperience solution on startup (see `AiraUnifiedModuleInstaller.cs`).

To completely uninstall, run the following SQL after removing the NuGet package from the solution:

```sql
drop table KenticoAiraUnified_AiraUnifiedChatMessage
drop table KenticoAiraUnified_AiraUnifiedChatPromptGroup
drop table KenticoAiraUnified_AiraUnifiedChatPrompt
drop table KenticoAiraUnified_AiraUnifiedChatSummary
drop table KenticoAiraUnified_AiraUnifiedChatThread
drop table KenticoAiraUnified_AiraUnifiedConfigurationItem

delete
FROM [dbo].[CMS_Class] where ClassName like 'kenticoairaunified%'

delete
from [CMS_Resource] where ResourceName = 'CMS.Integration.AiraUnified'
```

> Note: there is currently no way to migrate chat history and the configuration in the database between versions of this integration in the case that the database schema includes breaking changes. This feature could be added in a future update.
