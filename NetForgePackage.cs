using System;
using System.Runtime.InteropServices;
using System.Threading;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace NetForge.VsExtension
{
    /// <summary>
    /// The NetForge VS 2022 package. Registers the Tools-menu commands. Build + package this inside Visual
    /// Studio 2022 (Extension Development workload) — the VSSDK build tooling produces the .vsix.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("NetForge", "ASP.NET Core 10 + React 19 starter", "0.1.0")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid("6f7a1b2c-3d4e-5f60-7182-93a4b5c6d7e8")]
    public sealed class NetForgePackage : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.RegisterCommandsAsync();
        }
    }
}
