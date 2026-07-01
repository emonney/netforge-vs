using System;
using System.IO;
using System.Threading.Tasks;

namespace NetForge.VsExtension.Core
{
    /// <summary>
    /// Silently installs/updates the free <c>NetForge.Templates</c> dotnet template so NetForge appears in VS's
    /// native "Create a new project" dialog. Runs once per extension version (a marker file gates it), entirely
    /// best-effort — if the SDK is absent or offline it does nothing and the scaffold wizard guides the user later.
    /// </summary>
    internal static class TemplateProvisioner
    {
        public static async Task EnsureAsync(string extensionVersion)
        {
            try
            {
                var marker = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "NetForge", "vs-template-" + extensionVersion + ".ok");
                if (File.Exists(marker))
                    return;

                var sdk = await DotNetCli.ProbeAsync();
                if (!sdk.Installed)
                    return; // no SDK yet; don't nag at startup — the New Project flow handles it on demand

                // Install the bundled, version-matched .nupkg (offline) so NetForge shows in the native
                // "Create a new project" dialog immediately after this extension loads — not only after the
                // first use of our own wizard. Falls back to nuget.org if no nupkg is bundled.
                var result = await DotNetCli.InstallTemplateAsync(DotNetCli.FindBundledTemplate());
                if (result.Code == 0)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(marker));
                    File.WriteAllText(marker, DateTime.UtcNow.ToString("o"));
                }
            }
            catch
            {
                // Never disrupt VS startup.
            }
        }
    }
}
