using System;
using System.IO;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace NetForge.VsExtension.Core
{
    /// <summary>Drives a Community scaffold end-to-end with in-IDE feedback (output pane, status bar, dialogs).</summary>
    internal static class ProjectCreator
    {
        private static OutputWindowPane _pane;

        public static async Task CreateCommunityAsync(string name, string location)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var pane = await GetPaneAsync();
            await pane.ActivateAsync();
            await pane.WriteLineAsync("── NetForge: creating \"" + name + "\" ──");

            var sdk = await DotNetCli.ProbeAsync();
            if (!sdk.Installed)
            {
                var open = await VS.MessageBox.ShowConfirmAsync(
                    "NetForge",
                    "The .NET SDK isn't on your PATH. NetForge needs the .NET 10 SDK to scaffold and run. Open the download page?");
                if (open) NetForgeUrls.Open(NetForgeUrls.DotnetDownload);
                return;
            }

            if (!sdk.MeetsMinimum)
            {
                var proceed = await VS.MessageBox.ShowConfirmAsync(
                    "NetForge",
                    "NetForge targets the .NET 10 SDK, but " + (string.IsNullOrEmpty(sdk.Version) ? "an older version" : sdk.Version) + " is active. Continue anyway?");
                if (!proceed) return;
            }

            var target = Path.Combine(location, name);

            if (!await DotNetCli.IsTemplateInstalledAsync())
            {
                await VS.StatusBar.ShowMessageAsync("Installing the NetForge template…");
                await pane.WriteLineAsync("Installing the NetForge template (NetForge.Templates)…");
                var install = await DotNetCli.InstallTemplateAsync();
                await pane.WriteLineAsync(install.StdOut + install.StdErr);
                if (install.Code != 0)
                {
                    await FailAsync(pane, "Couldn't install the NetForge template.");
                    return;
                }
            }

            await VS.StatusBar.ShowMessageAsync("Scaffolding " + name + "…");
            await pane.WriteLineAsync("Scaffolding into " + target + "…");
            var result = await DotNetCli.ScaffoldAsync(name, target);
            await pane.WriteLineAsync(result.StdOut + result.StdErr);
            if (result.Code != 0)
            {
                await FailAsync(pane, "Couldn't scaffold the project.");
                return;
            }

            await pane.WriteLineAsync("Done. Opening the solution…");
            await VS.StatusBar.ShowMessageAsync("NetForge project \"" + name + "\" created.");

            var solution = FindSolution(target);
            if (solution != null)
            {
                try
                {
                    var solutionService = await VS.GetRequiredServiceAsync<SVsSolution, IVsSolution>();
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    solutionService.OpenSolutionFile(0, solution);
                }
                catch (Exception ex)
                {
                    await pane.WriteLineAsync("Created. Open it manually: " + solution + " (" + ex.Message + ")");
                }
            }
            else
            {
                await VS.MessageBox.ShowAsync("NetForge", "Project created at " + target + ".");
            }
        }

        private static async Task FailAsync(OutputWindowPane pane, string summary)
        {
            await pane.ActivateAsync();
            await VS.StatusBar.ShowMessageAsync(summary);
            await VS.MessageBox.ShowErrorAsync("NetForge", summary + "\n\nSee the NetForge output window for details.");
        }

        private static async Task<OutputWindowPane> GetPaneAsync()
        {
            if (_pane == null)
                _pane = await VS.Windows.CreateOutputWindowPaneAsync("NetForge");
            return _pane;
        }

        private static string FindSolution(string dir)
        {
            if (!Directory.Exists(dir)) return null;
            var slnx = Directory.GetFiles(dir, "*.slnx");
            if (slnx.Length > 0) return slnx[0];
            var sln = Directory.GetFiles(dir, "*.sln");
            return sln.Length > 0 ? sln[0] : null;
        }
    }
}
