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
