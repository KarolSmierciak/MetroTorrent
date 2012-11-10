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
            tcpListener = new TcpListener(IPAddress.Any, port);
        }

        /* Methods */
        
        /// <summary>
        /// Additional initialization which due to its weight and pessimistic
        /// executing time cannot be in constructor.
        /// </summary>
        public void Initialize()
        {
            // Blocks until a client has connected to the server.
            this.tcpListener.Start();
            this.tcpClient = this.tcpListener.AcceptTcpClient();
            System.Console.WriteLine("Client connected");

            //this.listenThread = new Thread(new ThreadStart(this.ListenToClient));
            //this.listenThread.Start();

            this.updateThread = new Thread(new ThreadStart(this.UpdateClient));
            this.updateThread.Start();
        }

        /// <summary>
        /// Listen to the client's (MetroTorrent front-end's) inquiries.
        /// </summary>
        private void ListenToClient()
        {
            while (true)
            {
                /*if (this.SendToMetro(Serializer.Deserialize<List<TorrentInfo>>(this.GetTorrentsInfo(metroClient))))
                {
                    Debug.WriteLine("Receiving and deserializing successful.");
                }*/
            }
        }

        /// <summary>
        /// Update client (MetroTorrent front-end) with info about the tasks status.
        /// </summary>
        private void UpdateClient()
        {
            while (true)
            {
                /*if (this.SendToMetro(Serializer.Serialize(this.GetTorrentsInfo(metroClient))))
                {
                    Debug.WriteLine("Serializing and sending successful.");
                }*/

                TorrentInfo torrentInfo = new TorrentInfo()
                {
                    Name = "torrentName",
                    Peers = 2,
                    Seeds = 3,
                    DownloadSpeed = 10
                };
                var tcpStream = this.tcpClient.GetStream();
                string update = Serializer.Serialize(torrentInfo);
                string test = "asdf";
                byte[] byteArray = Encoding.UTF8.GetBytes(test);
                tcpStream.Write(byteArray, 0, byteArray.Length);
                tcpStream.Flush();

                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void ChangeSomething(object client)
        {
        }



        /// <summary>
        /// Sends important tasks-related data to the MetroTorrent front-end.
        /// </summary>
        /// <param name="json">json string to be sent.</param>
        /// <returns>True if succeeded in sending the data, false if not.</returns>
        private bool SendToMetro(string json)
        {
            /*var tcpStream = this.tcpClient.GetStream();
            

            string update = Serializer.Serialize(torrentInfo);
            byte[] byteArray = Encoding.UTF8.GetBytes(update);
            tcpStream.Write(byteArray, 0, 1024);*/
            return true;
        }

        /// <summary>
        /// Extracts torrents' information from the torrent collection.
        /// </summary>
        /// <param name="metroClient">MetroTorrent torrents' collection manager.</param>
        /// <returns>True if succeeded in getting the data, false if not.</returns>
        private bool GetTorrentsInfo(MetroClient metroClient)
        {
            return true;
        }
    }
}
