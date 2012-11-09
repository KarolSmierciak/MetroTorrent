using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MetroTorrent.DataStorage;
using MetroTorrent.ServerCommunication;
using MetroTorrent.Settings;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

namespace MetroTorrent.Pages
{
    /// <summary>
    /// A page that displays a group title, a list of items within the group, and details for
    /// the currently selected item.
    /// </summary>
    public sealed partial class DownloadPage : MetroTorrent.Common.LayoutAwarePage
    {

        private ObservableCollection<TorrentData> torrents = new ObservableCollection<TorrentData>();

        private Popup generalSettingsPopup = null;
        private Popup connectionSettingsPopup = null;

        // Desired width for the settings UI. UI guidelines specify this should be 346 or 646 depending on your needs.
        private const int settingsWidth = 646;

        public DownloadPage()
        {
            this.InitializeComponent();
            SettingsPane.GetForCurrentView().CommandsRequested += onCommandsRequested;
            this.itemListView.ItemsSource = torrents;

            AddTorrent(new TorrentData("aaaaaaaaaaaaaaaa"));
            AddTorrent(new TorrentData("aaaaaaaaaaaaaaaa"));
            AddTorrent(new TorrentData("aaaaaaaaaaaaaaaa"));
        }

        #region Page state management

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
            // TODO: Assign a bindable group to this.DefaultViewModel["Group"]
            // TODO: Assign a collection of bindable items to this.DefaultViewModel["Items"]

            if (pageState == null)
            {
                // When this is a new page, select the first item automatically unless logical page
                // navigation is being used (see the logical page navigation #region below.)
                if (!this.UsingLogicalPageNavigation() && this.itemsViewSource.View != null)
                {
                    this.itemsViewSource.View.MoveCurrentToFirst();
                }
            }
            else
            {
                // Restore the previously saved state associated with this page
                if (pageState.ContainsKey("SelectedItem") && this.itemsViewSource.View != null)
                {
                    // TODO: Invoke this.itemsViewSource.View.MoveCurrentTo() with the selected
                    //       item as specified by the value of pageState["SelectedItem"]
                }
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            if (this.itemsViewSource.View != null)
            {
                var selectedItem = this.itemsViewSource.View.CurrentItem;
                // TODO: Derive a serializable navigation parameter and assign it to
                //       pageState["SelectedItem"]
            }
        }

        #endregion

        #region Logical page navigation

        // Visual state management typically reflects the four application view states directly
        // (full screen landscape and portrait plus snapped and filled views.)  The split page is
        // designed so that the snapped and portrait view states each have two distinct sub-states:
        // either the item list or the details are displayed, but not both at the same time.
        //
        // This is all implemented with a single physical page that can represent two logical
        // pages.  The code below achieves this goal without making the user aware of the
        // distinction.

        /// <summary>
        /// Invoked to determine whether the page should act as one logical page or two.
        /// </summary>
        /// <param name="viewState">The view state for which the question is being posed, or null
        /// for the current view state.  This parameter is optional with null as the default
        /// value.</param>
        /// <returns>True when the view state in question is portrait or snapped, false
        /// otherwise.</returns>
        private bool UsingLogicalPageNavigation(ApplicationViewState? viewState = null)
        {
            if (viewState == null) viewState = ApplicationView.Value;
            return viewState == ApplicationViewState.FullScreenPortrait ||
                viewState == ApplicationViewState.Snapped;
        }

        /// <summary>
        /// Invoked when an item within the list is selected.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is Snapped)
        /// displaying the selected item.</param>
        /// <param name="e">Event data that describes how the selection was changed.</param>
        void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*SerialTest st = new SerialTest();
            st.Name = "trolololo";
            SerialTest x = LocalStorage.Deserialize<SerialTest>(LocalStorage.Serialize(st));
            MessageDialog md = new MessageDialog(x.Name);
            await md.ShowAsync();*/
            int id = this.itemListView.SelectedIndex;
            if (filetypelabel.Visibility == Visibility.Collapsed)
            {
                ShowAllLabels();
            }

            if (id > -1)
                filesListBox.ItemsSource = torrents[id].Files;
            else
                HideAllLabels();

            if (this.UsingLogicalPageNavigation()) this.InvalidateVisualState();
        }

        void ShowAllLabels()
        {
            filetypelabel.Visibility = Visibility.Visible;
            downloadlabel.Visibility = Visibility.Visible;
            uploadlabel1.Visibility = Visibility.Visible;
            seedslabel.Visibility = Visibility.Visible;
            peerslabel.Visibility = Visibility.Visible;
            etalabel.Visibility = Visibility.Visible;
            uploadUnitLabel.Visibility = Visibility.Visible;
            downloadUnitName.Visibility = Visibility.Visible;
            filesListBox.Visibility = Visibility.Visible;
            smallskull.Visibility = Visibility.Visible;

            bigskull.Visibility = Visibility.Collapsed;
        }

        void HideAllLabels()
        {
            filetypelabel.Visibility = Visibility.Collapsed;
            downloadlabel.Visibility = Visibility.Collapsed;
            uploadlabel1.Visibility = Visibility.Collapsed;
            seedslabel.Visibility = Visibility.Collapsed;
            peerslabel.Visibility = Visibility.Collapsed;
            etalabel.Visibility = Visibility.Collapsed;
            uploadUnitLabel.Visibility = Visibility.Collapsed;
            downloadUnitName.Visibility = Visibility.Collapsed;
            filesListBox.Visibility = Visibility.Collapsed;
            smallskull.Visibility = Visibility.Collapsed;

            bigskull.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Invoked when the page's back button is pressed.
        /// </summary>
        /// <param name="sender">The back button instance.</param>
        /// <param name="e">Event data that describes how the back button was clicked.</param>
        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.UsingLogicalPageNavigation() && itemListView.SelectedItem != null)
            {
                // When logical page navigation is in effect and there's a selected item that
                // item's details are currently displayed.  Clearing the selection will return to
                // the item list.  From the user's point of view this is a logical backward
                // navigation.
                this.itemListView.SelectedItem = null;
            }
            else
            {
                // When logical page navigation is not in effect, or when there is no selected
                // item, use the default back button behavior.
                base.GoBack(sender, e);
            }
        }

        /// <summary>
        /// Invoked to determine the name of the visual state that corresponds to an application
        /// view state.
        /// </summary>
        /// <param name="viewState">The view state for which the question is being posed.</param>
        /// <returns>The name of the desired visual state.  This is the same as the name of the
        /// view state except when there is a selected item in portrait and snapped views where
        /// this additional logical page is represented by adding a suffix of _Detail.</returns>
        protected override string DetermineVisualState(ApplicationViewState viewState)
        {
            // Update the back button's enabled state when the view state changes
            var logicalPageBack = this.UsingLogicalPageNavigation(viewState) && this.itemListView.SelectedItem != null;
            var physicalPageBack = this.Frame != null && this.Frame.CanGoBack;
            this.DefaultViewModel["CanGoBack"] = logicalPageBack || physicalPageBack;

            // Determine visual states for landscape layouts based not on the view state, but
            // on the width of the window.  This page has one layout that is appropriate for
            // 1366 virtual pixels or wider, and another for narrower displays or when a snapped
            // application reduces the horizontal space available to less than 1366.
            if (viewState == ApplicationViewState.Filled ||
                viewState == ApplicationViewState.FullScreenLandscape)
            {
                var windowWidth = Window.Current.Bounds.Width;
                if (windowWidth >= 1366) return "FullScreenLandscapeOrWide";
                return "FilledOrNarrow";
            }

            // When in portrait or snapped start with the default visual state name, then add a
            // suffix when viewing details instead of the list
            var defaultStateName = base.DetermineVisualState(viewState);
            return logicalPageBack ? defaultStateName + "_Detail" : defaultStateName;
        }

        #endregion

        private void onGeneralSettingsCommand(IUICommand command)
        {
            SettingsCommand settingsCommand = (SettingsCommand)command;
            if (generalSettingsPopup == null)
            {
                generalSettingsPopup = new Popup();

                generalSettingsPopup.IsLightDismissEnabled = true;
                generalSettingsPopup.Width = settingsWidth;
                generalSettingsPopup.Height = Window.Current.Bounds.Height;

                generalSettingsPopup.ChildTransitions = new TransitionCollection();
                generalSettingsPopup.ChildTransitions.Add(new PaneThemeTransition()
                {
                    Edge = (SettingsPane.Edge == SettingsEdgeLocation.Right) ?
                           EdgeTransitionLocation.Right :
                           EdgeTransitionLocation.Left
                });

                GeneralSettings mypane = new GeneralSettings();
                mypane.Width = settingsWidth;
                mypane.Height = Window.Current.Bounds.Height;

                generalSettingsPopup.Child = mypane;

                generalSettingsPopup.SetValue(Canvas.LeftProperty, SettingsPane.Edge == SettingsEdgeLocation.Right ? (Window.Current.Bounds.Width - settingsWidth) : 0);
                generalSettingsPopup.SetValue(Canvas.TopProperty, 0);
            }
            generalSettingsPopup.IsOpen = true;
        }

        private void onConnectionSettingsCommand(IUICommand command)
        {
            SettingsCommand settingsCommand = (SettingsCommand)command;
            if (connectionSettingsPopup == null)
            {
                connectionSettingsPopup = new Popup();

                connectionSettingsPopup.IsLightDismissEnabled = true;
                connectionSettingsPopup.Width = settingsWidth;
                connectionSettingsPopup.Height = Window.Current.Bounds.Height;

                connectionSettingsPopup.ChildTransitions = new TransitionCollection();
                connectionSettingsPopup.ChildTransitions.Add(new PaneThemeTransition()
                {
                    Edge = (SettingsPane.Edge == SettingsEdgeLocation.Right) ?
                           EdgeTransitionLocation.Right :
                           EdgeTransitionLocation.Left
                });

                ConnectionSettings mypane = new ConnectionSettings();
                mypane.Width = settingsWidth;
                mypane.Height = Window.Current.Bounds.Height;

                connectionSettingsPopup.Child = mypane;

                connectionSettingsPopup.SetValue(Canvas.LeftProperty, SettingsPane.Edge == SettingsEdgeLocation.Right ? (Window.Current.Bounds.Width - settingsWidth) : 0);
                connectionSettingsPopup.SetValue(Canvas.TopProperty, 0);
            }
            connectionSettingsPopup.IsOpen = true;
        }

        private void onCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs eventArgs)
        {
            UICommandInvokedHandler generalHandler = new UICommandInvokedHandler(onGeneralSettingsCommand);
            UICommandInvokedHandler connectionHandler = new UICommandInvokedHandler(onConnectionSettingsCommand);

            SettingsCommand generalCommand = new SettingsCommand("generalSettingsId", "General", generalHandler);
            eventArgs.Request.ApplicationCommands.Add(generalCommand);

            SettingsCommand connectionCommand = new SettingsCommand("connectionSettingsId", "Connection", connectionHandler);
            eventArgs.Request.ApplicationCommands.Add(connectionCommand);
        }

        private static bool firststart = true;
        private static Object firststartlock = new Object();
        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {

            lock (firststartlock)
            {
                ConfigData.Instance.OnConfigurationError += ErrorOccured;
                ConfigData.Instance.OnFirstRun += FirstRunHanlder;
                if (firststart)
                {
                    firststart = false;
                    ConfigData.Instance.Load();
                }
            }
        }

        private async void ErrorOccured(string str)
        {
            MessageDialog dial = new MessageDialog(str);
            await dial.ShowAsync();
        }

        //private bool firstTest = true;

        private void FirstRunHanlder()
        {
            this.Frame.Navigate(typeof(FirstRunConfiguration));
        }

        public void AddTorrent(TorrentData torrent)
        {
            torrents.Add(torrent);
        }

        private void AppBar_Opened_1(object sender, object e)
        {
            if (itemListView.SelectedItem != null)
            {
                bremove.Visibility = Visibility.Visible;
                bdelete.Visibility = Visibility.Visible;
            }
            else
            {
                bremove.Visibility = Visibility.Collapsed;
                bdelete.Visibility = Visibility.Collapsed;
            }
        }

        private void RemoveTorrent()
        {
            filesListBox.ItemsSource = null;
            torrents.RemoveAt(itemListView.SelectedIndex);
        }

        private void bremove_Click(object sender, RoutedEventArgs e)
        {
            RemoveTorrent();
        }

        private void bdelete_Click(object sender, RoutedEventArgs e)
        {
            RemoveTorrent();
        }

        private async void badd_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".torrent");
            IReadOnlyList<StorageFile> files = await openPicker.PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                foreach (StorageFile file in files)
                {
                    
                }
            }
        }

    }

}
