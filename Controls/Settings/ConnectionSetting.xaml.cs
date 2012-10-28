using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MetroTorrent.Controls.Settings
{
    public sealed partial class ConnectionSetting : UserControl
    {
        public ConnectionSetting()
        {
            this.InitializeComponent();
            this.oldport = this.portbox.Text = "60606";
            this.oldup = this.upSpeedBox.Text = "0";
            this.olddown = this.downSpeedBox.Text = "0";
        }

        private string oldport;
        private string oldup;
        private string olddown;

        private void portbox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                int port = int.Parse(this.portbox.Text);
                if (port < 1 || port > 65535)
                    this.portbox.Text = this.oldport;
                else
                    this.oldport = this.portbox.Text;
            }
            catch { this.portbox.Text = this.oldport; }
            //var md = new MessageDialog("trolololo");
            //await md.ShowAsync();
        }

        
        private void upSpeedBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                int.Parse(this.upSpeedBox.Text);
                this.oldup = this.upSpeedBox.Text;
            }
            catch { this.upSpeedBox.Text = this.oldup; }
        }

        private void downSpeedBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                int.Parse(this.downSpeedBox.Text);
                this.olddown = this.downSpeedBox.Text;
            }
            catch { this.downSpeedBox.Text = this.olddown; }
        }
    }
}
