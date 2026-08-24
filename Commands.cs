using System;
using System.ComponentModel.Design;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using NetForge.VsExtension.Core;
using NetForge.VsExtension.UI;
using Task = System.Threading.Tasks.Task;

namespace NetForge.VsExtension
{
    /// <summary>Registers the Tools ▸ NetForge / File ▸ New commands against the menu command service.</summary>
    internal static class Commands
    {
        public static async Task RegisterAsync(AsyncPackage package)
        {
            await package.JoinableTaskFactory.SwitchToMainThreadAsync();
            if (!(await package.GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService mcs))
                return;

            mcs.AddCommand(Bind(PackageIds.NewProject, () => Fire(package, NewProjectAsync)));
            mcs.AddCommand(Bind(PackageIds.WhatsInPro, ShowPro));
            mcs.AddCommand(Bind(PackageIds.OpenConfigurator, () => NetForgeUrls.Open(NetForgeUrls.Configure)));
            mcs.AddCommand(Bind(PackageIds.OpenDemo, () => NetForgeUrls.Open(NetForgeUrls.Demo)));
            mcs.AddCommand(Bind(PackageIds.OpenDocs, () => NetForgeUrls.Open(NetForgeUrls.Docs)));
        }

        private static MenuCommand Bind(int id, Action exec) =>
            new MenuCommand((s, e) => exec(), new CommandID(PackageGuids.CmdSet, id));

        private static void Fire(AsyncPackage package, Func<Task> work)
        {
            _ = package.JoinableTaskFactory.RunAsync(async () =>
            {
                try
                {
                    await work();
                }
                catch (Exception ex)
                {
                    await VS.MessageBox.ShowErrorAsync("NetForge", ex.Message);
                }
            });
        }

        private static void ShowPro()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            new ProShowcaseDialog().ShowDialog();
        }

        private static async Task NewProjectAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dialog = new NewProjectDialog();
            if (dialog.ShowDialog() != true)
                return;

            if (dialog.SelectedEdition == NewProjectDialog.Edition.Pro)
            {
                var name = Uri.EscapeDataString(dialog.ProjectName ?? string.Empty);
                var url = NetForgeUrls.Configurator + "?edition=pro" + (string.IsNullOrEmpty(name) ? "" : "&name=" + name) + "#configure";
                NetForgeUrls.Open(url);
                return;
            }

            await ProjectCreator.CreateCommunityAsync(dialog.ProjectName, dialog.Location, dialog.Frontend, dialog.Database, dialog.BrandColor, dialog.BrandTheme);
        }
    }
}
