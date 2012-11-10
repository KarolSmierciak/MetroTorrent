using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commons;

namespace MetroTorrent.DataStorage
{
    public class TorrentData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<FileData> files = new ObservableCollection<FileData>();

        public TorrentData()
        {
            
        }

        public TorrentData (TorrentInfo ti)
        {
            foreach (string s in ti.Files)
                files.Add(new FileData(s));
            this.torrentName = ti.Name;
            this.torrentProgress = ti.Progress;
            this.downSpeed = ti.DownloadSpeed;
            this.upSpeed = ti.UploadSpeed;
            this.eta = ti.ETA;
            this.peers = ti.Peers;
            this.seeds = ti.Seeds;
        }

        public void Update(TorrentData td)
        {
            this.torrentProgress = td.torrentProgress;
            this.downSpeed = td.DownloadSpeed;
            this.upSpeed = td.upSpeed;
            this.eta = td.eta;
            this.peers = td.peers;
            this.seeds = td.seeds;
        }

        public TorrentData(string name)
        {
            this.TorrentName = name;
            //files.Add(new FileData("asdf.jpg"));
            //files.Add(new FileData("asdf.avi"));
        }

        private double torrentProgress;
        private string torrentName;
        private int downSpeed;
        private int upSpeed;
        private string eta;
        private int seeds;
        private int peers;

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
            get { return seeds; }
        }

        public int Peers
        {
            get { return peers; }
        }

        public string ETA
        {
            get
            {
                return eta;
            }
        }

        public bool EqualsTorrent(object obj)
        {
            if (obj is TorrentData)
            {
                if ((obj as TorrentData).torrentName == this.torrentName)
                    return true;
            }
            return false;
        }
    }
}
