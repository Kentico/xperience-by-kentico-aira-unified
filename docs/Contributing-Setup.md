# Contributing Setup

## Required Software

The requirements to setup, develop, and build this project are listed below.

### .NET Runtime

.NET SDK 8.0 or newer

- <https://dotnet.microsoft.com/en-us/download/dotnet/8.0>
- See `global.json` file for specific SDK requirements

### Node.js Runtime

- [Node.js](https://nodejs.org/en/download) 20.10.0 or newer
- [NVM for Windows](https://github.com/coreybutler/nvm-windows) to manage multiple installed versions of Node.js
- See `engines` in the [`package.json`](/src/Kentico.Xperience.AiraUnified/FrontEnd/package.json) for specific version requirements

### C# Editor

- VS Code
- Visual Studio
- Rider

### Database

SQL Server 2019 or newer compatible database

- [SQL Server Linux](https://learn.microsoft.com/en-us/sql/linux/sql-server-linux-setup?view=sql-server-ver15)
- [Azure SQL Edge](https://learn.microsoft.com/en-us/azure/azure-sql-edge/disconnected-deployment)

### SQL Editor

- MS SQL Server Management Studio
- Azure Data Studio

## Sample Project

### Database Setup

Running the sample project requires creating a new Xperience by Kentico database using the included template.

Change directory in your console to `./examples/DancingGoat` and follow the instructions in the Xperience
documentation on [creating a new database](https://docs.kentico.com/developers-and-admins/installation#Installation-CreateProjectDatabase).

## FrontEnd Customization

The Aira Unified application uses frontend assets located in:

- `src/Kentico.Xperience.AiraUnified/wwwroot/js/index.js`
- `src/Kentico.Xperience.AiraUnified/wwwroot/css/`

These files are **generated** from source files located in the `src/Kentico.Xperience.AiraUnified/FrontEnd` folder.

Changes made to the client-side code in the `FrontEnd` folder will **not** be reflected immediately. To apply the changes, you must rebuild the assets using Webpack.

### Rebuilding Client Assets

After making changes, navigate to the `FrontEnd` folder and run the following commands:

```bash
npm run build
npm run build-css
```

This will recompile the JavaScript and CSS, and update the files in the wwwroot directory accordingly.

## Development Workflow

1. Create a new branch with one of the following prefixes

   - `feat/` - for new functionality
   - `refactor/` - for restructuring of existing features
   - `fix/` - for bugfixes

1. Run `dotnet format` against the `Kentico.Xperience.AiraUnified` solution

   > use `dotnet: format` VS Code task.

1. Commit changes, with a commit message preferably following the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/#summary) convention.

1. Once ready, create a PR on GitHub. The PR will need to have all comments resolved and all tests passing before it will be merged.

   - The PR should have a helpful description of the scope of changes being contributed.
   - Include screenshots or video to reflect UX or UI updates
   - Indicate if new settings need to be applied when the changes are merged - locally or in other environments
