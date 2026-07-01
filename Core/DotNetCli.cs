using System;
using System.Diagnostics;
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

        public static async Task<bool> IsTemplateInstalledAsync()
        {
            var result = await RunAsync("new list " + TemplateShortName);
            if (result.Code != 0)
                return false;
            var text = result.StdOut;
            return text.IndexOf(TemplateShortName, StringComparison.OrdinalIgnoreCase) >= 0
                && text.IndexOf("no templates", StringComparison.OrdinalIgnoreCase) < 0;
        }

        public static Task<CliResult> InstallTemplateAsync()
        {
            return RunAsync("new install " + TemplatePackageId);
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
