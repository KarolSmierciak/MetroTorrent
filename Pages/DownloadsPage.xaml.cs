using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetroTorrent.Settings;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MetroTorrent.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class DownloadsPage : MetroTorrent.Common.LayoutAwarePage
    {

        private Popup generalSettingsPopup = null;
        private Popup connectionSettingsPopup = null;

        // Desired width for the settings UI. UI guidelines specify this should be 346 or 646 depending on your needs.
        private const int settingsWidth = 646;

        public DownloadsPage()
        {
            this.InitializeComponent();
            SettingsPane.GetForCurrentView().CommandsRequested += onCommandsRequested;
        }

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
    }
}
