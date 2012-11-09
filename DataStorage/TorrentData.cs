using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroTorrent.DataStorage
{
    public class TorrentData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<FileData> files = new ObservableCollection<FileData>();

        public TorrentData()
        {
            files.Add(new FileData(null, "asdf.jpg"));
            files.Add(new FileData(null,"asdf.avi"));
        }

        public TorrentData(string name)
        {
            this.TorrentName = name;
        }

        private double torrentProgress;
        private string torrentName;
        private int downSpeed;
        private int upSpeed;

        public ObservableCollection<FileData> Files
        {
            get
            {
                return files;
            }
        }

        public double TorrentProgress
        {
            get
            {
                return torrentProgress;
            }
            set
            {
                torrentProgress = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TorrentProgress"));
            }
        }

        public string TorrentName
        {
            get
            {
                return torrentName;
            }
            set
            {
                torrentName = value;
                if (PropertyChanged != null)
                    PropertyChanged(this,new PropertyChangedEventArgs("TorrentName"));
            }
        }

        public int DownloadSpeed
        {
            get
            {
                return downSpeed;
            }
            set
            {
                downSpeed = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("DownloadSpeed"));
            }
        }

        public int UploadSpeed
        {
            get
            {
                return upSpeed;
            }
            set
            {
                upSpeed = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("UploadSpeed"));
            }
        }

        public string FileTypes
        {
            get
            {
                string ret = "";
                string temp;
                for (int i = 0; i < files.Count; i++)
                {
                    temp = files[i].FileType;
                    if (ret == "")
                        ret = temp;
                    else if (!ret.Contains(temp))
                        ret += ", " + temp;
                }
                return ret;
            }
        }

        public int Seeds
        {
            get { return 0; }
        }

        public int Peers
        {
            get { return 0; }
        }

        public string ETA
        {
            get
            {
                return "some ETA";
            }
        }
    }
}
