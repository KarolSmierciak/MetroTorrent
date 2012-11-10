using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MetroTorrent.Controls
{
    public sealed partial class TorrentItem : UserControl
    {
        public TorrentItem(string torrentName)
        {
            this.InitializeComponent();
            this.TorrentNameBox.Text = torrentName;
            this.TorrentProgress.Value = 50;
        }

        public string TorrentName
        {
            get
            {
                return this.TorrentNameBox.Text;
            }
            set
            {
                this.TorrentNameBox.Text = value;
            }
        }

    }
}
