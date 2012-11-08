using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetroTorrent.Common;
using MetroTorrent.DataStorage;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MetroTorrent.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GeneralSettings : LayoutAwarePage
    {
        public GeneralSettings()
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

        private void LayoutAwarePage_Unloaded_1(object sender, RoutedEventArgs e)
        {
            ConfigData.Instance.TempFilePath = this.tempFilePathBox.Text;
            ConfigData.Instance.DownFilePath = this.downFilePathBox.Text;
            ConfigData.Instance.RunWithOS = this.startWithWindowsToggle.IsOn;
            ConfigData.Instance.Save();
        }

        private void LayoutAwarePage_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.tempFilePathBox.Text = ConfigData.Instance.TempFilePath;
            this.downFilePathBox.Text = ConfigData.Instance.DownFilePath;
            this.startWithWindowsToggle.IsOn = ConfigData.Instance.RunWithOS;
        }

        public static void SetFilters(FolderPicker folderPicker)
        {
            folderPicker.FileTypeFilter.Add(".docx");
            folderPicker.FileTypeFilter.Add(".mp3");
            folderPicker.FileTypeFilter.Add(".ogg");
            folderPicker.FileTypeFilter.Add(".zip");
            folderPicker.FileTypeFilter.Add(".rar");
            folderPicker.FileTypeFilter.Add(".exe");
            folderPicker.FileTypeFilter.Add(".iso");
            folderPicker.FileTypeFilter.Add(".mdf");
            folderPicker.FileTypeFilter.Add(".avi");
            folderPicker.FileTypeFilter.Add(".mpeg");
            folderPicker.FileTypeFilter.Add(".rm");
            folderPicker.FileTypeFilter.Add(".wma");
            folderPicker.FileTypeFilter.Add(".wav");
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            SetFilters(folderPicker);
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                ConfigData.Instance.TempFilePath = folder.Path;
                ConfigData.Instance.Save();
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            SetFilters(folderPicker);
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                ConfigData.Instance.DownFilePath = folder.Path;
                ConfigData.Instance.Save();
            }
        }
    }
}
