using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetroTorrent.DataStorage;
using MetroTorrent.Settings;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MetroTorrent.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class FirstRunConfiguration : MetroTorrent.Common.LayoutAwarePage
    {
        public FirstRunConfiguration()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            GeneralSettings.SetFilters(folderPicker);
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                this.tempFilePathBox.Text = folder.Path;
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            GeneralSettings.SetFilters(folderPicker);
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                this.downFilePathBox.Text = folder.Path;
                if (this.tempFilePathBox.Text == "")
                    this.tempFilePathBox.Text = folder.Path;
            }
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (downFilePathBox.Text != "" && tempFilePathBox.Text != "")
            {
                ConfigData.Instance.DownFilePath = this.downFilePathBox.Text;
                ConfigData.Instance.TempFilePath = this.tempFilePathBox.Text;
                ConfigData.Instance.Save();
                //this.Frame.Navigate(typeof(DownloadsPage));
                this.Frame.GoBack();
            }
            else
            {
                MessageDialog dial = new MessageDialog("Both fields need to be filled");
                await dial.ShowAsync();
            }
        }
    }
}
