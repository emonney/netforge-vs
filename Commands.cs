using System.Diagnostics;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace NetForge.VsExtension
{
    // PackageIds is generated from VSCommandTable.vsct by the toolkit at build time
    // (NetForge.VsExtension.VSCommandTable.PackageIds).

    /// <summary>Opens the NetForge configurator — pick features + edition, then download a starter.</summary>
    [Command(PackageIds.OpenConfigurator)]
    internal sealed class OpenConfiguratorCommand : BaseCommand<OpenConfiguratorCommand>
    {
        protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            Browse("https://netforge.ebenmonney.com");
            return Task.CompletedTask;
        }

        internal static void Browse(string url) =>
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }

    /// <summary>Opens GitHub Sponsors — any amount unlocks the full Pro feature set + the offline CLI.</summary>
    [Command(PackageIds.UpgradeToPro)]
    internal sealed class UpgradeToProCommand : BaseCommand<UpgradeToProCommand>
    {
        protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            OpenConfiguratorCommand.Browse("https://github.com/sponsors/emonney");
            return Task.CompletedTask;
        }
    }
}
