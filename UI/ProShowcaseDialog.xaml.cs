using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace NetForge.VsExtension.UI
{
    /// <summary>The themed "What's in Pro?" showcase — feature grid, screenshots, and CTAs.</summary>
    public partial class ProShowcaseDialog : Window
    {
        public sealed class FeatureItem
        {
            public string Title { get; set; }
            public string Blurb { get; set; }
        }

        private static readonly FeatureItem[] Features =
        {
            new FeatureItem { Title = "Multi-tenancy", Blurb = "Tenants, members, invitations, per-tenant branding & roles." },
            new FeatureItem { Title = "Audit trail", Blurb = "Change-tracking interceptor, admin log, per-entity timeline." },
            new FeatureItem { Title = "Widget dashboard", Blurb = "Drag/resize grid, saved per-user layouts, charts." },
            new FeatureItem { Title = "Global ⌘K search", Blurb = "Command palette with provider fan-out across your data." },
            new FeatureItem { Title = "Notifications & real-time", Blurb = "In-app SignalR bell, retention, email delivery." },
            new FeatureItem { Title = "Outgoing webhooks", Blurb = "HMAC-signed delivery, retries, subscriptions UI." },
            new FeatureItem { Title = "2FA · OAuth · sessions", Blurb = "Advanced auth, account linking, per-device sessions." },
            new FeatureItem { Title = "File uploads", Blurb = "Avatars, blob storage, Magick.NET image processing." },
            new FeatureItem { Title = "Export & import", Blurb = "CSV / Excel / PDF export plus CSV / XLSX import." },
            new FeatureItem { Title = "Background jobs", Blurb = "Hangfire job server, recurring jobs, dashboard." },
            new FeatureItem { Title = "PWA · tour · changelog", Blurb = "Installable shell, onboarding, in-app what's-new." },
            new FeatureItem { Title = "Sales demo domain", Blurb = "A complete vertical — catalog, orders, invoices." },
        };

        public ProShowcaseDialog()
        {
            InitializeComponent();
            VsTheme.Apply(this);
            FeatureList.ItemsSource = new List<FeatureItem>(Features);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var url = (sender as Button)?.Tag as string;
            if (!string.IsNullOrEmpty(url)) NetForgeUrls.Open(url);
        }
    }
}
