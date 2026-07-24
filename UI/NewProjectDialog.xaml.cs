using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace NetForge.VsExtension.UI
{
    /// <summary>The guided "New NetForge Project" dialog. Collects edition, name, and location.</summary>
    public partial class NewProjectDialog : Window
    {
        public enum Edition { Community, Pro }

        private static readonly Regex NameRe = new Regex("^[A-Za-z][A-Za-z0-9_]{0,49}$", RegexOptions.Compiled);

        public Edition SelectedEdition { get; private set; } = Edition.Community;
        public string ProjectName { get; private set; }
        public string Location { get; private set; }
        public string Frontend { get; private set; } = "react";
        public string Database { get; private set; } = "sqlite";
        public string BrandTheme { get; private set; } = string.Empty;
        public string BrandColor { get; private set; } = string.Empty;

        public NewProjectDialog()
        {
            InitializeComponent();
            VsTheme.Apply(this);
            TxtLocation.Text = DefaultLocation();
            Loaded += (s, e) =>
            {
                TxtName.Focus();
                TxtName.SelectAll();
                Revalidate();
            };
        }

        private static string DefaultLocation()
        {
            try
            {
                var repos = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source", "repos");
                return Directory.Exists(repos) ? repos : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            catch
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

        private void Edition_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            SelectedEdition = RdoPro.IsChecked == true ? Edition.Pro : Edition.Community;
            bool community = SelectedEdition == Edition.Community;
            LocationPanel.Visibility = community ? Visibility.Visible : Visibility.Collapsed;
            BtnPrimary.Content = community ? "Create" : "Open configurator…";
            Revalidate();
        }

        // Wired to TextBox.TextChanged in XAML (must match the TextChangedEventHandler signature).
        private void Validate(object sender, TextChangedEventArgs e) => Revalidate();

        private void Revalidate()
        {
            if (!IsInitialized) return;

            string error = null;
            if (SelectedEdition == Edition.Community)
            {
                var name = (TxtName.Text ?? string.Empty).Trim();
                var loc = (TxtLocation.Text ?? string.Empty).Trim();
                if (!NameRe.IsMatch(name))
                    error = "Start with a letter; letters, digits, and underscores only (max 50).";
                else if (string.IsNullOrEmpty(loc))
                    error = "Choose a location.";
                else if (!Directory.Exists(loc))
                    error = "That location doesn't exist.";
                else
                {
                    var target = Path.Combine(loc, name);
                    if (Directory.Exists(target) && Directory.GetFileSystemEntries(target).Length > 0)
                        error = "\"" + target + "\" already exists and isn't empty.";
                }
            }

            ValidationText.Text = error ?? string.Empty;
            ValidationText.Visibility = error == null ? Visibility.Collapsed : Visibility.Visible;
            BtnPrimary.IsEnabled = error == null;
        }

        private void BrandColor_Changed(object sender, TextChangedEventArgs e) => UpdateSwatch();

        private void UpdateSwatch()
        {
            if (ColorSwatch == null) return;
            var c = ParseColor(TxtBrandColor.Text);
            ColorSwatch.Background = c.HasValue
                ? new System.Windows.Media.SolidColorBrush(c.Value)
                : System.Windows.Media.Brushes.Transparent;
        }

        private static System.Windows.Media.Color? ParseColor(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            try { return (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(text.Trim()); }
            catch { return null; }
        }

        private void PickColor_Click(object sender, RoutedEventArgs e)
        {
            using (var dlg = new System.Windows.Forms.ColorDialog { FullOpen = true, AnyColor = true })
            {
                var current = ParseColor(TxtBrandColor.Text);
                if (current.HasValue)
                    dlg.Color = System.Drawing.Color.FromArgb(current.Value.R, current.Value.G, current.Value.B);
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    TxtBrandColor.Text = string.Format("#{0:X2}{1:X2}{2:X2}", dlg.Color.R, dlg.Color.G, dlg.Color.B);
            }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            using (var dlg = new System.Windows.Forms.FolderBrowserDialog())
            {
                dlg.Description = "Choose where to create the project";
                if (Directory.Exists(TxtLocation.Text)) dlg.SelectedPath = TxtLocation.Text;
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TxtLocation.Text = dlg.SelectedPath;
                    Revalidate();
                }
            }
        }

        private void Primary_Click(object sender, RoutedEventArgs e)
        {
            ProjectName = (TxtName.Text ?? string.Empty).Trim();
            Location = (TxtLocation.Text ?? string.Empty).Trim();
            Frontend = ((CboFrontend.SelectedItem as ComboBoxItem)?.Tag as string) ?? "react";
            Database = ((CboDatabase.SelectedItem as ComboBoxItem)?.Tag as string) ?? "sqlite";
            BrandTheme = ((CboTheme.SelectedItem as ComboBoxItem)?.Tag as string) ?? string.Empty;
            BrandColor = (TxtBrandColor.Text ?? string.Empty).Trim();
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
