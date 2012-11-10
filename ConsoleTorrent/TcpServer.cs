namespace ConsoleTorrent
{
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using Commons;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a TCP server - back-end for MetroTorrent application.
    /// </summary>
    public class TcpServer
    {
        /* Fields */

        public delegate void TorrentAddedHandler(string path);
        public event TorrentAddedHandler TorrentAdded;

        public delegate void TorrentRemovedHanlder(string path);
        public event TorrentRemovedHanlder TorrentRemoved;


        /// <summary>
        /// TcpListener object - core of the TcpServer class.
        /// </summary>
        private TcpListener tcpListener;
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
            
        }

        public void Initialize()
        {
            tcpListener = new TcpListener(IPAddress.Any, 60606);
            this.tcpListener.Start();
        }

        public void Listen()
        {
            while (true)
            {
                this.tcpClient = this.tcpListener.AcceptTcpClient();
                //System.Console.WriteLine("Client connected");
                try
                {
                    TorrentInfo torrentInfo = new TorrentInfo()
                    {
                        Name = "torrentName",
                        Peers = 2,
                        Seeds = 3,
                        DownloadSpeed = 10
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
                    //return;
                }
            }
        }

        public void ListenToQueries()
        {
            queryListener = new TcpListener(IPAddress.Any, 60607);
            this.queryListener.Start();
            queryThread = new Thread(new ThreadStart(ProcessQueries));
            queryThread.Start();
        }

        private void ProcessQueries()
        {
            while (true)
            {
                var Client = this.queryListener.AcceptTcpClient();
                try
                {
                    var tcpStream = Client.GetStream();
                    byte [] byteArray = new byte[1024];
                    int read = tcpStream.Read(byteArray, 0, byteArray.Length);
                    if (read > 0)
                    {
                        string temp = Encoding.Default.GetString(byteArray);
                        ProcessQuery(tcpStream, temp);
                    }
                    tcpClient.Close();
                }
                catch
                {
                }
            }
        }

        private void ProcessQuery(NetworkStream stream, string str)
        {
            System.Console.WriteLine(str);
            string id = str.Substring(0,1);
            string path = str.Substring(1);

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

    }
}
