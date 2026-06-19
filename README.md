# NetForge for Visual Studio 2022

A Visual Studio 2022 extension that surfaces NetForge — the AI-ready ASP.NET Core 10 + React 19 starter —
from the **Tools** menu: open the configurator to scaffold a project, and jump to Pro.

> **Status:** scaffold built with the modern [Community.VisualStudio.Toolkit](https://github.com/VsixCommunity/Community.VisualStudio.Toolkit).
> It **builds and packages inside Visual Studio 2022** with the *"Visual Studio extension development"*
> workload installed (which provides the VSSDK build tooling). `dotnet build` on its own will not produce a
> `.vsix`. This is the counterpart to the [VS Code extension](../netforge-vscode), which is fully built.

## What it does

- **Tools → NetForge: New Project (Configurator)** — opens [the configurator](https://netforge.ebenmonney.com)
  to pick features + edition and download a starter.
- **Tools → NetForge: Upgrade to Pro** — opens [GitHub Sponsors](https://github.com/sponsors/emonney);
  any amount unlocks the full Pro feature set and the offline CLI.

## Build & package (in Visual Studio 2022)

1. Install the **"Visual Studio extension development"** workload (VS Installer).
2. Open this folder in VS 2022 (it creates a solution around `NetForge.VsExtension.csproj`).
3. **Build → Build Solution** (Release) — the `.vsix` lands in `bin\Release\`.
4. **F5** launches the VS Experimental Instance to debug the commands.
5. Publish to the [Visual Studio Marketplace](https://marketplace.visualstudio.com/manage) via the
   *Manage Publishers & Extensions* portal (or `VsixPublisher.exe publish`).

## Files

- `NetForge.VsExtension.csproj` — SDK-style VSIX project (net472 + VSSDK build tools).
- `source.extension.vsixmanifest` — marketplace metadata + install targets.
- `VSCommandTable.vsct` — the Tools-menu group + the two command buttons.
- `NetForgePackage.cs` — the async package (registers commands).
- `Commands.cs` — the two command handlers.

## Next steps (roadmap)

- Replace the external-browser "Upgrade to Pro" with an in-VS WebView2 tool window showing the Pro pitch
  (mirrors the VS Code extension's webview).
- In-VS Basic scaffolding (download from the configurator API + add to the open solution), matching the
  VS Code extension's New Project flow.

## License

MIT.
