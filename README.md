# Xperience by Kentico Aira

[![Kentico Labs](https://img.shields.io/badge/Kentico_Labs-grey?labelColor=orange&logo=data:image/svg+xml;base64,PHN2ZyBjbGFzcz0ic3ZnLWljb24iIHN0eWxlPSJ3aWR0aDogMWVtOyBoZWlnaHQ6IDFlbTt2ZXJ0aWNhbC1hbGlnbjogbWlkZGxlO2ZpbGw6IGN1cnJlbnRDb2xvcjtvdmVyZmxvdzogaGlkZGVuOyIgdmlld0JveD0iMCAwIDEwMjQgMTAyNCIgdmVyc2lvbj0iMS4xIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPjxwYXRoIGQ9Ik05NTYuMjg4IDgwNC40OEw2NDAgMjc3LjQ0VjY0aDMyYzE3LjYgMCAzMi0xNC40IDMyLTMycy0xNC40LTMyLTMyLTMyaC0zMjBjLTE3LjYgMC0zMiAxNC40LTMyIDMyczE0LjQgMzIgMzIgMzJIMzg0djIxMy40NEw2Ny43MTIgODA0LjQ4Qy00LjczNiA5MjUuMTg0IDUxLjIgMTAyNCAxOTIgMTAyNGg2NDBjMTQwLjggMCAxOTYuNzM2LTk4Ljc1MiAxMjQuMjg4LTIxOS41MnpNMjQxLjAyNCA2NDBMNDQ4IDI5NS4wNFY2NGgxMjh2MjMxLjA0TDc4Mi45NzYgNjQwSDI0MS4wMjR6IiAgLz48L3N2Zz4=)](https://github.com/Kentico/.github/blob/main/SUPPORT.md#labs-limited-support) 
[![CI: Build and Test](https://github.com/Kentico/aira-unified/actions/workflows/ci.yml/badge.svg)](https://github.com/Kentico/aira-unified/actions/workflows/ci.yml) 
[![NuGet Package](https://img.shields.io/nuget/v/Kentico.Xperience.Aira.svg)](https://www.nuget.org/packages/Kentico.Xperience.Aira)

## Description

Aira integration enabling for alternative administration UI and chatbot which can easily be added to an Xperience By Kentico project.
## Screenshots

![UI Application](/images/screenshots/ui_application.png)

## Library Version Matrix

| Xperience Version |    Library Version   |
| ----------------- | -------------------- |
| >= 30.0.0         | >= 0.1.0-prerelase-1 |

### Dependencies

- [ASP.NET Core 8.0](https://dotnet.microsoft.com/en-us/download)
- [Xperience by Kentico](https://docs.xperience.io/xp/changelog)

## Package Installation

Add the package to your application using the .NET CLI

```powershell
dotnet add package Kentico.Xperience.Aira
```

## Quick Start

1. Include `Kentico.Xperience.Aira` project in the solution.

   ```powershell
   dotnet add package Kentico.Xperience.Aira
   ```

2. Register required services into DI container.

   ```csharp
   // Program.cs

   var builder = WebApplication.CreateBuilder(args);

   // ...

   builder.Services.AddKenticoAira();

   // ...

   var app = builder.Build();

   // ...
   
   app.UseAiraEndpoints();

   app.Run();

   ```

3. In the administration go to UI application 'Aira Companion App'.
4. Fill out the Aira configuration form, populating the fields with your custom values.
- Relative Path Base - the relative path where the Aira Companion App is available. The path is relative to the base url of your application.
- Logo - the asset selectable from Media Library shown in the top left corner of the Aira Companion App pwa.
- Chat Title - the title displayed on top of the screen of the chat page and the text of chat option in Aira Companion App menu.
- Chat Image - the asset selectable from Media library shown on top of the screen of the chat page and next to the chat option text in Aira Companion App menu.
- Smart Upload Title - the title displayed on top of the screen of the smart upload page and the text of smart upload option in Aira Companion App menu.
- Smart Upload Image - the asset selectable from Media library shown on top of the screen of the smart upload page and next to the smart upload option text in Aira Companion App menu.
5. Return to administration dashboard and select a `Content Type` used for [Mass asset upload](https://docs.kentico.com/developers-and-admins/development/content-types#mass-asset-upload-configuration). Uploaded assets in the Smart upload page of this integration will be saved under this `Content type` in the Content hub.
6. The users can now visit the Aira Companion App under the specified path.

## Full Instructions

View the [Usage Guide](./docs/Usage-Guide.md) for more detailed instructions.

You can view and start the DancingGoat example project.

## Contributing

To see the guidelines for Contributing to Kentico open source software, please see [Kentico's `CONTRIBUTING.md`](https://github.com/Kentico/.github/blob/main/CONTRIBUTING.md) for more information and follow the [Kentico's `CODE_OF_CONDUCT`](https://github.com/Kentico/.github/blob/main/CODE_OF_CONDUCT.md).

Instructions and technical details for contributing to **this** project can be found in [Contributing Setup](./docs/Contributing-Setup.md).

## License

Distributed under the MIT License. See [`LICENSE.md`](./LICENSE.md) for more information.

## Support

[![Kentico Labs](https://img.shields.io/badge/Kentico_Labs-grey?labelColor=orange&logo=data:image/svg+xml;base64,PHN2ZyBjbGFzcz0ic3ZnLWljb24iIHN0eWxlPSJ3aWR0aDogMWVtOyBoZWlnaHQ6IDFlbTt2ZXJ0aWNhbC1hbGlnbjogbWlkZGxlO2ZpbGw6IGN1cnJlbnRDb2xvcjtvdmVyZmxvdzogaGlkZGVuOyIgdmlld0JveD0iMCAwIDEwMjQgMTAyNCIgdmVyc2lvbj0iMS4xIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPjxwYXRoIGQ9Ik05NTYuMjg4IDgwNC40OEw2NDAgMjc3LjQ0VjY0aDMyYzE3LjYgMCAzMi0xNC40IDMyLTMycy0xNC40LTMyLTMyLTMyaC0zMjBjLTE3LjYgMC0zMiAxNC40LTMyIDMyczE0LjQgMzIgMzIgMzJIMzg0djIxMy40NEw2Ny43MTIgODA0LjQ4Qy00LjczNiA5MjUuMTg0IDUxLjIgMTAyNCAxOTIgMTAyNGg2NDBjMTQwLjggMCAxOTYuNzM2LTk4Ljc1MiAxMjQuMjg4LTIxOS41MnpNMjQxLjAyNCA2NDBMNDQ4IDI5NS4wNFY2NGgxMjh2MjMxLjA0TDc4Mi45NzYgNjQwSDI0MS4wMjR6IiAgLz48L3N2Zz4=)](https://github.com/Kentico/.github/blob/main/SUPPORT.md#labs-limited-support) 

This project has **Kentico Labs limited support**.

See [`SUPPORT.md`](https://github.com/Kentico/.github/blob/main/SUPPORT.md#full-support) for more information.

For any security issues see [`SECURITY.md`](https://github.com/Kentico/.github/blob/main/SECURITY.md).
