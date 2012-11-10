namespace ConsoleTorrent
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using Commons;
    using MonoTorrent.Client;
    using MonoTorrent.Common;

    /// <summary>
    /// Represents a TCP server - back-end for MetroTorrent application.
    /// </summary>
    public class TcpServer
    {
        /* Fields */

        /// <summary>
        /// Called by TorrentAdded event with a directory to the torrent file to be loaded.
        /// </summary>
        /// <param name="torrentDir"></param>
        public delegate void TorrentAddedHandler(string torrentDir);

        /// <summary>
        /// Fires everytime a torrent is added to the list.
        /// </summary>
        public event TorrentAddedHandler TorrentAdded;

        /// <summary>
        /// Called by TorrentRemoved with a name to the torrent file to be removed.
        /// </summary>
        /// <param name="torrentName"></param>
        public delegate void TorrentRemovedHandler(string torrentName);

        /// <summary>
        /// Fires everytime a torrent is deleted from the list.
        /// </summary>
        public event TorrentRemovedHandler TorrentRemoved;

        /// <summary>
        /// Port at which to listen.
        /// </summary>
        private int port;

        /// <summary>
        /// MetroTorrent client representation.
        /// </summary>
        private TorrentsData torrentClient;

        /// <summary>
        /// TcpListener object - core of the TcpServer class.
        /// </summary>
        private TcpListener tcpListener;

        /// <summary>
        /// 
        /// </summary>
        private TcpListener queryListener;

        /// <summary>
        /// Client for the TCP/IP communication.
        /// </summary>
        private TcpClient tcpClient;

        /// <summary>
        /// Thread responsible for updating the MetroTorrent front-end.
        /// </summary>
        private Thread queryThread;

        /* Constructor */

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServer" /> class.
        /// </summary>
        public TcpServer(int port = 60606)
        {
            this.port = port;
            torrentClient = new TorrentsData();

            this.TorrentAdded += AddTorrent;
            this.TorrentRemoved += RemoveTorrent;
        }

        /* Properties */

        /* Methods */

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            this.tcpListener.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Listen()
        {
            while (true)
            {
                foreach (TorrentManager torrentManager in this.torrentClient.TorrentManagers.ToList())
                {
                    this.tcpClient = this.tcpListener.AcceptTcpClient();
                    // System.Console.WriteLine("Client connected");
                    try
                    {
                        TorrentInfo torrentInfo = new TorrentInfo()
                        {
                            Files = torrentManager.Torrent.Files.Select<TorrentFile, string>(torrentFile => torrentFile.Path).ToList(),
                            Name = torrentManager.Torrent.Name,
                            Peers = torrentManager.Peers.Leechs,
                            Seeds = torrentManager.Peers.Seeds,
                            UploadSpeed = torrentManager.Monitor.UploadSpeed / 1024,
                            DownloadSpeed = torrentManager.Monitor.DownloadSpeed / 1024,
                            Progress = torrentManager.Progress,
                            ETA = this.CalculateSpeed(torrentManager)
                        };

                        var tcpStream = this.tcpClient.GetStream();
                        string update = Serializer.Serialize(torrentInfo);
                        byte[] byteArray = Encoding.UTF8.GetBytes(update);
                        tcpStream.Write(byteArray, 0, byteArray.Length);
                        tcpClient.Close();

                        Thread.Sleep(1000);
                    }
                    catch
                    {
                        // return;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ListenToQueries()
        {
            this.queryListener = new TcpListener(IPAddress.Any, 60607);
            this.queryListener.Start();
            this.queryThread = new Thread(new ThreadStart(ProcessQueries));
            this.queryThread.Start();
        }

        private void AddTorrent(string torrentDir)
        {
            this.torrentClient.InitializeTorrent(torrentDir);
        }

        private void RemoveTorrent(string torrentName)
        {
            this.torrentClient.TorrentManagers.RemoveAll(torrentManager => torrentManager.Torrent.Name == torrentName);
        }

        /// <summary>
        /// 
        /// </summary>
        private void ProcessQueries()
        {
            while (true)
            {
                TcpClient tcpClient = this.queryListener.AcceptTcpClient();
                try
                {
                    NetworkStream tcpStream = tcpClient.GetStream();
                    byte[] byteArray = new byte[1024];
                    int read = tcpStream.Read(byteArray, 0, byteArray.Length);
                    if (read > 0)
                    {
                        string temp = Encoding.Default.GetString(byteArray);
                        this.ProcessQuery(tcpStream, temp);
                    }
                    this.tcpClient.Close();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="str"></param>
        private void ProcessQuery(NetworkStream stream, string str)
        {
            System.Console.WriteLine(str);
            string id = str.Substring(0, 1).Trim();
            string path = str.Substring(1).Replace("\0", string.Empty);
            Debug.WriteLine("{0} {1}", id, id.Length);

            switch (id)
            {
                case "1":
                    if (TorrentAdded != null)
                        TorrentAdded(path);
                    break;
                case "2":
                    if (TorrentRemoved != null)
                        TorrentRemoved(path);
                    break;
            }
        }

        private string CalculateSpeed(TorrentManager torrentManager)
        {
            string Return = string.Empty;
            if (torrentManager.Complete)
            {
                if (torrentManager.Monitor.DownloadSpeed != 0)
                {
                    Return = (torrentManager.Torrent.Size / torrentManager.Monitor.DownloadSpeed).ToString() + " sec";
                }
            }
            else
            {
                if (torrentManager.Monitor.UploadSpeed != 0)
                {
                    Return = (torrentManager.Torrent.Size / torrentManager.Monitor.UploadSpeed).ToString() + " sec";
                }
            }
            return Return;
        }
    }
}
