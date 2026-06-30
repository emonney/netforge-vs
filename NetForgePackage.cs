using System;
using System.Runtime.InteropServices;
using System.Threading;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using NetForge.VsExtension.Core;
using Task = System.Threading.Tasks.Task;

namespace NetForge.VsExtension
{
    /// <summary>
    /// The NetForge Visual Studio package. Registers the Tools ▸ NetForge commands and, on first load per
    /// version, silently ensures the `NetForge.Templates` dotnet template is installed so NetForge appears in
    /// VS's native "Create a new project" dialog. Auto-loads in the background once the shell is initialized.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("NetForge", "ASP.NET Core 10 + React 19 starter", "1.0.0")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    [Guid("6f7a1b2c-3d4e-5f60-7182-93a4b5c6d7e8")]
    public sealed class NetForgePackage : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await Commands.RegisterAsync(this);

            // Best-effort, off the UI thread: make NetForge show up in "Create a new project" without the user
            // ever opening our wizard. Never blocks or fails startup.
            var version = GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0";
            _ = System.Threading.Tasks.Task.Run(() => TemplateProvisioner.EnsureAsync(version));
        }
    }

    /// <summary>Outbound links — kept in one place so commands and dialogs agree.</summary>
    internal static class NetForgeUrls
    {
        public const string Configurator = "https://netforge.ebenmonney.com";
        public const string Demo = "https://demo.netforge.ebenmonney.com";
        public const string Docs = "https://docs.netforge.ebenmonney.com";
        public const string Sponsor = "https://github.com/sponsors/emonney";
        public const string DotnetDownload = "https://dotnet.microsoft.com/download/dotnet/10.0";

        public static void Open(string url) =>
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
    }
}
