# Xperience by Kentico Aira Unified

[![Kentico Labs](https://img.shields.io/badge/Kentico_Labs-grey?labelColor=orange&logo=data:image/svg+xml;base64,PHN2ZyBjbGFzcz0ic3ZnLWljb24iIHN0eWxlPSJ3aWR0aDogMWVtOyBoZWlnaHQ6IDFlbTt2ZXJ0aWNhbC1hbGlnbjogbWlkZGxlO2ZpbGw6IGN1cnJlbnRDb2xvcjtvdmVyZmxvdzogaGlkZGVuOyIgdmlld0JveD0iMCAwIDEwMjQgMTAyNCIgdmVyc2lvbj0iMS4xIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPjxwYXRoIGQ9Ik05NTYuMjg4IDgwNC40OEw2NDAgMjc3LjQ0VjY0aDMyYzE3LjYgMCAzMi0xNC40IDMyLTMycy0xNC40LTMyLTMyLTMyaC0zMjBjLTE3LjYgMC0zMiAxNC40LTMyIDMyczE0LjQgMzIgMzIgMzJIMzg0djIxMy40NEw2Ny43MTIgODA0LjQ4Qy00LjczNiA5MjUuMTg0IDUxLjIgMTAyNCAxOTIgMTAyNGg2NDBjMTQwLjggMCAxOTYuNzM2LTk4Ljc1MiAxMjQuMjg4LTIxOS41MnpNMjQxLjAyNCA2NDBMNDQ4IDI5NS4wNFY2NGgxMjh2MjMxLjA0TDc4Mi45NzYgNjQwSDI0MS4wMjR6IiAgLz48L3N2Zz4=)](https://github.com/Kentico/.github/blob/main/SUPPORT.md#labs-limited-support) 
[![CI: Build and Test](https://github.com/Kentico/aira-unified/actions/workflows/ci.yml/badge.svg)](https://github.com/Kentico/aira-unified/actions/workflows/ci.yml) 
[![NuGet Package](https://img.shields.io/nuget/v/Kentico.Xperience.AiraUnified.svg)](https://www.nuget.org/packages/Kentico.Xperience.AiraUnified)

## Description

Aira Unified integration provides an alternative administration UI and chatbot that can be easily added to an Xperience by Kentico (XbyK) project. It is designed for content editors and marketers who need essential functionality on mobile devices.

## Screenshots

![UI Application](/images/screenshots/ui_application.png)

## Library Version Matrix

| Xperience Version |    Library Version   |
| ----------------- | -------------------- |
| >= 30.0.0         | >= 0.1.0-prerelase-1 |

## Dependencies

- [ASP.NET Core 8.0](https://dotnet.microsoft.com/en-us/download)
- [Xperience by Kentico](https://docs.xperience.io/xp/changelog)

## Package Installation

Add the package to your application using the .NET CLI

```powershell
dotnet add package Kentico.Xperience.AiraUnified
```

## Quick Start

1. Add the Aira Unified API subscription key to your `appsettings.json`:

```json
"AiraUnifiedOptions": {
  "AiraUnifiedApiSubscriptionKey": "<your aira unified API key>"
}
```
2. Configure your project for [HTTPS](https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl).

3. Include the `Kentico.Xperience.AiraUnified` package in your project:

   ```powershell
   dotnet add package Kentico.Xperience.AiraUnified
   ```

4. Register the required services in `Program.cs`:

   ```csharp
   var builder = WebApplication.CreateBuilder(args);

   // ...
   
   builder.Services.AddKenticoAiraUnified(builder.Configuration);
   
   // ...

   var app = builder.Build();

   // ...
   
   app.UseAiraUnifiedEndpoints();
   
   app.Run();
   ```

5. Configure the Aira Unified settings in the administration UI:
   - **Relative Path Base**: Defines where Aira Unified is available.
   - **Logo**: Select an asset from the Media Library.
   - **Chat Title**: Title for the chat page.
   - **Smart Upload Title**: Title for the smart upload page.
   
   ![Admin Configuration](/images/AiraUnifiedAdminConfiguration.png)

6. Set up a `Content Type` for [Mass Asset Upload](https://docs.kentico.com/developers-and-admins/development/content-types#mass-asset-upload-configuration).
7. Configure role-based permissions in the `Role Management` application:
   - **View**: Access the Aira Unified chat.
   - **Create/Update**: Access and upload content via Smart Upload.

   ![Role Configuration](/images/ConfigureAiraUnifiedPermissions.png)

8. In case of using this library in a project with XbyK versions > 30.2.0 the Aira Unified expects a workspace named "Kentico Default" (code name 'KenticoDefault'). In that case, add a workspace with a code name `KenticoDefault` workspace.

9. Users can now sign in to the Aira Unified app - `<your-path-base>/signin`.

## Full Instructions

You can view and start the DancingGoat example project.

To activate the communication of AIRA Unified app with the AIRA service, you need to request an activation key. Please contact us at productmanagement@xperience.io for more information.

## Contributing

To see the guidelines for Contributing to Kentico open source software, please see [Kentico's `CONTRIBUTING.md`](https://github.com/Kentico/.github/blob/main/CONTRIBUTING.md) for more information and follow the [Kentico's `CODE_OF_CONDUCT`](https://github.com/Kentico/.github/blob/main/CODE_OF_CONDUCT.md).

Find project-specific contribution details in [Contributing Setup](./docs/Contributing-Setup.md).

## License

Distributed under the MIT License. See [`LICENSE.md`](./LICENSE.md) for more information.

## Support

[![Kentico Labs](https://img.shields.io/badge/Kentico_Labs-grey?labelColor=orange&logo=data:image/svg+xml;base64,PHN2ZyBjbGFzcz0ic3ZnLWljb24iIHN0eWxlPSJ3aWR0aDogMWVtOyBoZWlnaHQ6IDFlbTt2ZXJ0aWNhbC1hbGlnbjogbWlkZGxlO2ZpbGw6IGN1cnJlbnRDb2xvcjtvdmVyZmxvdzogaGlkZGVuOyIgdmlld0JveD0iMCAwIDEwMjQgMTAyNCIgdmVyc2lvbj0iMS4xIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPjxwYXRoIGQ9Ik05NTYuMjg4IDgwNC40OEw2NDAgMjc3LjQ0VjY0aDMyYzE3LjYgMCAzMi0xNC40IDMyLTMycy0xNC40LTMyLTMyLTMyaC0zMjBjLTE3LjYgMC0zMiAxNC40LTMyIDMyczE0LjQgMzIgMzIgMzJIMzg0djIxMy40NEw2Ny43MTIgODA0LjQ4Qy00LjczNiA5MjUuMTg0IDUxLjIgMTAyNCAxOTIgMTAyNGg2NDBjMTQwLjggMCAxOTYuNzM2LTk4Ljc1MiAxMjQuMjg4LTIxOS41MnpNMjQxLjAyNCA2NDBMNDQ4IDI5NS4wNFY2NGgxMjh2MjMxLjA0TDc4Mi45NzYgNjQwSDI0MS4wMjR6IiAgLz48L3N2Zz4=)](https://github.com/Kentico/.github/blob/main/SUPPORT.md#labs-limited-support) 

This project has **Kentico Labs limited support**.

See [`SUPPORT.md`](https://github.com/Kentico/.github/blob/main/SUPPORT.md#full-support) for more information.

This feature is currently in a Preview mode, do not use for production instances.

For any security issues see [`SECURITY.md`](https://github.com/Kentico/.github/blob/main/SECURITY.md).
