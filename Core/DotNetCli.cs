using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetForge.VsExtension.Core
{
    /// <summary>Thin async wrappers over the <c>dotnet</c> CLI — SDK probe, template install, scaffolding.</summary>
    internal static class DotNetCli
    {
        public const string TemplatePackageId = "NetForge.Templates";
        public const string TemplateShortName = "netforge";
        public const int MinSdkMajor = 10;

        public sealed class CliResult
        {
            public int Code { get; set; }
            public string StdOut { get; set; } = string.Empty;
            public string StdErr { get; set; } = string.Empty;
            /// <summary>True when <c>dotnet</c> itself couldn't be launched (not on PATH).</summary>
            public bool NotFound => Code == -1;
        }

        public sealed class SdkInfo
        {
            public bool Installed { get; set; }
            public int Major { get; set; }
            public string Version { get; set; } = string.Empty;
            public bool MeetsMinimum { get { return Installed && Major >= MinSdkMajor; } }
        }

        public static async Task<CliResult> RunAsync(string arguments, string workingDirectory = null)
        {
            var psi = new ProcessStartInfo("dotnet", arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = string.IsNullOrEmpty(workingDirectory) ? Environment.CurrentDirectory : workingDirectory,
            };

            try
            {
                using (var process = new Process { StartInfo = psi })
                {
                    var stdout = new StringBuilder();
                    var stderr = new StringBuilder();
                    process.OutputDataReceived += (s, e) => { if (e.Data != null) stdout.AppendLine(e.Data); };
                    process.ErrorDataReceived += (s, e) => { if (e.Data != null) stderr.AppendLine(e.Data); };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    await Task.Run(() => process.WaitForExit());

                    return new CliResult { Code = process.ExitCode, StdOut = stdout.ToString(), StdErr = stderr.ToString() };
                }
            }
            catch (System.ComponentModel.Win32Exception)
            {
                return new CliResult { Code = -1 }; // dotnet not on PATH
            }
        }

        public static async Task<SdkInfo> ProbeAsync()
        {
            var result = await RunAsync("--version");
            if (result.NotFound)
                return new SdkInfo { Installed = false };

            var version = result.StdOut.Trim();
            int major;
            int.TryParse(version.Split('.')[0], out major);
            return new SdkInfo { Installed = result.Code == 0, Major = major, Version = version };
        }

        /// <summary>The version-matched NetForge.Templates.*.nupkg bundled beside the extension, or null.</summary>
        public static string FindBundledTemplate()
        {
            try
            {
                var dir = Path.GetDirectoryName(typeof(DotNetCli).Assembly.Location);
                var templates = Path.Combine(dir, "Templates");
                return Directory.Exists(templates)
                    ? Directory.GetFiles(templates, "NetForge.Templates.*.nupkg").FirstOrDefault()
                    : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>How many NetForge.Templates packages are installed. Each `dotnet new install` of a local
        /// nupkg adds its own entry, so this catches the duplicates that a `--force` reinstall would create.</summary>
        public static async Task<int> CountCommunityInstallsAsync()
        {
            var result = await RunAsync("new uninstall");
            if (result.Code != 0)
                return 0;
            var hint = "dotnet new uninstall " + TemplatePackageId;
            int n = 0;
            foreach (var line in result.StdOut.Split('\n'))
                if (line.Trim() == hint) n++;
            return n;
        }

        /// <summary>
        /// Ensures exactly one NetForge.Templates is installed — from the bundled nupkg when present, else nuget.
        /// NEVER uses <c>--force</c>: forcing a local-path install ADDS a duplicate entry every time, which
        /// eventually breaks scaffolding ("Sequence contains more than one matching element"). Any extra/old
        /// copies are uninstalled first. <paramref name="forceReinstall"/> reinstalls even when one copy is
        /// already present (used on a new extension version to pick up a refreshed bundled template).
        /// Returns true if a NetForge.Templates ends up installed.
        /// </summary>
        public static async Task<bool> EnsureCommunityTemplateAsync(string bundledPath, bool forceReinstall)
        {
            var count = await CountCommunityInstallsAsync();
            if (!forceReinstall && count == 1)
                return true; // steady state: exactly one install

            // Clean slate — remove all NetForge.Templates entries (duplicates and/or an old version).
            for (int i = 0; i < 10 && (await CountCommunityInstallsAsync()) > 0; i++)
                await RunAsync("new uninstall " + TemplatePackageId);

            var install = string.IsNullOrEmpty(bundledPath)
                ? await RunAsync("new install " + TemplatePackageId)
                : await RunAsync("new install \"" + bundledPath + "\"");
            return install.Code == 0;
        }

        public static Task<CliResult> ScaffoldAsync(string name, string outputDir, string database = null, string brandColor = null, string brandTheme = null)
        {
            var args = new StringBuilder();
            args.AppendFormat("new {0} --name \"{1}\" --output \"{2}\"", TemplateShortName, name, outputDir);
            if (!string.IsNullOrWhiteSpace(database)) args.Append(" --database ").Append(database);
            if (!string.IsNullOrWhiteSpace(brandTheme)) args.AppendFormat(" --brandTheme \"{0}\"", brandTheme.Trim());
            if (!string.IsNullOrWhiteSpace(brandColor)) args.AppendFormat(" --brandColor \"{0}\"", brandColor.Trim());
            return RunAsync(args.ToString());
        }
    }
}
