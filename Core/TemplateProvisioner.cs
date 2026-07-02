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

                var sdk = await DotNetCli.ProbeAsync();
                if (!sdk.Installed)
                    return; // no SDK yet; don't nag at startup — the New Project flow handles it on demand

                // Install the bundled, version-matched .nupkg (offline) so NetForge shows in the native
                // "Create a new project" dialog after this extension loads — not only after the first use of
                // our own wizard. The marker means "this extension version has provisioned once" and triggers a
                // reinstall on a fresh/updated extension; but we DON'T early-return on it — the user may have
                // uninstalled the template since, so EnsureCommunityTemplateAsync always verifies the real
                // install count and self-heals (and never duplicates — see its note).
                bool firstForThisVersion = !File.Exists(marker);
                bool ok = await DotNetCli.EnsureCommunityTemplateAsync(DotNetCli.FindBundledTemplate(), forceReinstall: firstForThisVersion);
                if (ok && firstForThisVersion)
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
