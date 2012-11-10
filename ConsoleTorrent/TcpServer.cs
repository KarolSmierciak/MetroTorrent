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

        /// <summary>
        /// TcpListener object - core of the TcpServer class.
        /// </summary>
        private TcpListener tcpListener;

        /// <summary>
        /// Client for the TCP/IP communication.
        /// </summary>
        private TcpClient tcpClient;

        /// <summary>
        /// Thread responsible for listening to the MetroTorrent front-end.
        /// </summary>
        private Thread listenThread;

        /// <summary>
        /// Thread responsible for updating the MetroTorrent front-end.
        /// </summary>
        private Thread updateThread;

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
                System.Console.WriteLine("Client connected");
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
                    /*tcpStream.ReadTimeout = 500;
                    byteArray = new byte[1024];
                    int read = tcpStream.Read(byteArray, 0, byteArray.Length);
                    if (read > 0)
                    {
                        string temp = Encoding.Default.GetString(byteArray);
                        ProcessQuery(tcpStream, temp);
                    }*/
                    tcpClient.Close();

                    //Thread.Sleep(500);
                }
                catch
                {
                    //return;
                }
            }
        }

        private void ProcessQuery(NetworkStream stream, string str)
        {
            System.Console.WriteLine(str);
        }

    }
}
