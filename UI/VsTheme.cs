using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;

namespace NetForge.VsExtension.UI
{
    /// <summary>
    /// Themes a plain WPF window to match the current Visual Studio color theme at runtime. We can't
    /// reference VS theme types from XAML (they live in VS's runtime assemblies, not the SDK reference set),
    /// so the window ships a neutral fallback palette and we override the brush resources here in code.
    /// </summary>
    internal static class VsTheme
    {
        public static void Apply(Window window)
        {
            try
            {
                Set(window, "NfWindowBg", EnvironmentColors.ToolWindowBackgroundColorKey);
                Set(window, "NfText", EnvironmentColors.ToolWindowTextColorKey);
                Set(window, "NfBorder", EnvironmentColors.ToolWindowBorderColorKey);
                Set(window, "NfInputBg", EnvironmentColors.ComboBoxBackgroundColorKey);
            }
            catch
            {
                // Leave the XAML fallback palette in place if the theme service isn't available.
            }

            try
            {
                new WindowInteropHelper(window).Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            }
            catch
            {
            }
        }

        private static void Set(Window window, string key, ThemeResourceKey colorKey)
        {
            var c = VSColorTheme.GetThemedColor(colorKey);
            window.Resources[key] = new SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B));
        }
    }
}
