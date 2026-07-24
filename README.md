<div align="center">
  <img src="https://raw.githubusercontent.com/emonney/netforge-vs/HEAD/Resources/icon.png" height="80" alt="NetForge" />
  <img src="https://raw.githubusercontent.com/emonney/netforge-vs/HEAD/Resources/sep-x.png" height="80" alt="×" />
  <img src="https://raw.githubusercontent.com/emonney/netforge-vs/HEAD/Resources/visual-studio.png" height="80" alt="Visual Studio" />
  <h1>NetForge for Visual Studio</h1>
  <p><strong>Scaffold a production-shaped ASP.NET Core 10 app — with a React 19 or Angular 22 frontend — in seconds, right inside Visual Studio.</strong></p>
</div>

NetForge is an opinionated, **AI-ready**, beautiful-out-of-the-box starter for line-of-business apps — built to
be extended by AI or by you. Its vertical-slice architecture and clear conventions let an AI assistant add a
whole feature from a single prompt, and keep the result readable enough for **you** to review at a glance.
This extension creates one without leaving the IDE.

## Create a project: **Extensions ▸ NetForge**

**Extensions ▸ NetForge ▸ New NetForge Project…** is how you start a NetForge project in Visual Studio. It opens
a guided dialog — pick an **edition**, **frontend** (React or Angular), **database**, **theme**, and **accent
colour**, then name it. NetForge scaffolds locally and opens the ready-to-run solution. Press **F5** and the
ASP.NET Core server builds and runs, starting the client for you.

![NetForge dashboard](https://raw.githubusercontent.com/emonney/netforge-vs/HEAD/Resources/screenshots/dashboard-light.png)

<p align="center">
  <img src="https://raw.githubusercontent.com/emonney/netforge-vs/HEAD/Resources/screenshots/command-palette.png" width="49%" alt="Global ⌘K search" />
  <img src="https://raw.githubusercontent.com/emonney/netforge-vs/HEAD/Resources/screenshots/products-desktop.png" width="49%" alt="DataGrid with filtering, sorting, export" />
</p>

## Features

- **Extensions ▸ NetForge ▸ New NetForge Project…** — the guided creation dialog. Scaffolds locally and opens
  the solution; **F5** runs the whole stack (the ASP.NET Core server starts the client).
- **Pick your setup right in the dialog** — edition, frontend (**React 19 / Angular 22**), database (**SQLite /
  PostgreSQL / SQL Server**), theme, and brand accent colour.
- **Community scaffolds locally** with `dotnet new` — offline, instant, no sign-in. The extension detects your
  .NET SDK and installs the `NetForge.Templates` package the first time.
- **A themed Pro showcase** — see exactly what Pro unlocks (screenshots + the full feature grid) inside the IDE,
  via **Extensions ▸ NetForge ▸ What's in Pro?**
- Quick links to the **configurator**, **live demo**, and **docs**.

> **Tip:** NetForge also appears in Visual Studio's *Create a new project* dialog, but **Extensions ▸ NetForge**
> is the recommended path — it always includes your chosen client (React or Angular) and opens the solution ready to run.

## Requirements

- Visual Studio **2022 (17.x)** or **2026 (18.x)**.
- The [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) for Community scaffolding & running.

## Community vs Pro

| | **Community** (free, MIT) | **Pro** |
|---|---|---|
| Auth, RBAC, admin, theming | ✅ | ✅ |
| Vertical-slice architecture, DataGrid, forms | ✅ | ✅ |
| Multi-tenancy, audit, dashboards | — | ✅ |
| Global search, notifications, webhooks | — | ✅ |
| 2FA/OAuth/sessions, uploads, export/import, jobs | — | ✅ |
| PWA, tour, changelog, Sales demo domain | — | ✅ |

The **Community** edition is scaffolded locally and is free forever under the MIT license. **Pro** is generated
from the [configurator](https://netforge.ebenmonney.com) after signing in, and is unlocked by any
[GitHub Sponsors](https://github.com/sponsors/emonney) tier.

## Getting started

1. **Extensions ▸ NetForge ▸ New NetForge Project…**
2. Choose **Community**, pick your frontend / database / theme / accent colour, name your app, and choose a folder.
3. Press **F5** — the ASP.NET Core server builds and runs, starting the client for you. Sign in with the
   seeded dev admin (credentials are in the project **README.md**).

## Links

🌐 [Configurator](https://netforge.ebenmonney.com) · ▶ [Live demo](https://demo.netforge.ebenmonney.com) · 📘 [Docs](https://docs.netforge.ebenmonney.com) · 💜 [Sponsor & unlock Pro](https://github.com/sponsors/emonney)

## Build from source

Requires Visual Studio with the **Visual Studio extension development** workload (the VSSDK build tooling);
`dotnet build` alone won't produce a `.vsix`. From a Developer prompt:

```
msbuild NetForge.VsExtension.csproj /t:Restore;Build /p:Configuration=Release
```

The `.vsix` lands in `bin\Release\net48\`.

## License

The extension is MIT-licensed. The Community template it scaffolds is MIT; the Pro edition is sponsor-licensed.
