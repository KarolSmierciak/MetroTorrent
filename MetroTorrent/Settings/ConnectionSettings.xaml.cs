using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetroTorrent.DataStorage;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MetroTorrent.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConnectionSettings : MetroTorrent.Common.LayoutAwarePage
    {
        public ConnectionSettings()
        {
            this.InitializeComponent();
        }

        private void SettingsBackClicked(object sender, RoutedEventArgs e)
        {
            // First close our Flyout.
            Popup parent = this.Parent as Popup;
            if (parent != null)
            {
                parent.IsOpen = false;
            }

            // If the app is not snapped, then the back button shows the Settings pane again.
            if (Windows.UI.ViewManagement.ApplicationView.Value != Windows.UI.ViewManagement.ApplicationViewState.Snapped)
            {
                SettingsPane.Show();
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void LayoutAwarePage_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.upSpeedBox.Text = ConfigData.Instance.MaxUploadSpeed.ToString();
            this.downSpeedBox.Text = ConfigData.Instance.MaxDownloadSpeed.ToString();
        }

        private void LayoutAwarePage_Unloaded_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ConfigData.Instance.MaxUploadSpeed = int.Parse(this.upSpeedBox.Text);
            }
            catch
            {
                if (this.upSpeedBox.Text.Length == 0)
                    ConfigData.Instance.MaxUploadSpeed = 0;
            }
            try
            {
                ConfigData.Instance.MaxDownloadSpeed = int.Parse(this.downSpeedBox.Text);
            }
            catch
            {
                if (this.downSpeedBox.Text.Length == 0)
                    ConfigData.Instance.MaxDownloadSpeed = 0;
            }
            ConfigData.Instance.Save();
        }
    }
}
