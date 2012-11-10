using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commons;
using Windows.ApplicationModel.Core;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace MetroTorrent.ServerCommunication
{
    class ServerCommunicator
    {
        private static StreamSocket socket;
        private static HostName hostName;
        private static string port = "60606";

        public delegate void TorrentInfoReceinvedHandler(TorrentInfo ti);
        public event TorrentInfoReceinvedHandler TorrentInfoReceived;

        private Queue<string> tosend = new Queue<string>();

        private ServerCommunicator()
        {
            
        }

        private static ServerCommunicator instance;

        public static ServerCommunicator Instance
        {
            get
            {
                if (instance == null)
                    instance = new ServerCommunicator();
                return instance;
            }
        }

        public void SendMessage(string msg)
        {
            lock (tosend)
            {
                tosend.Enqueue(msg);
            }
        }

        public async void StartListening()
        {
            while (true)
            {
                try
                {
                    hostName = new HostName("localhost");
                }
                catch (ArgumentException)
                {
                    continue;
                }
                //CoreApplication.Properties.Add("clientSocket", socket);
                try
                {
                    socket = new StreamSocket();
                    await socket.ConnectAsync(hostName, port);
                    //CoreApplication.Properties.Add("connected", null);
                }
                catch
                {
                    continue;
                }
                try
                {
                    Windows.Storage.Streams.Buffer b = new Windows.Storage.Streams.Buffer(6000);
                    var xxx = await socket.InputStream.ReadAsync(b, b.Capacity, InputStreamOptions.None);
                    DataReader dr = DataReader.FromBuffer(b);
                    string str = dr.ReadString(dr.UnconsumedBufferLength);
                    if (this.TorrentInfoReceived != null)
                    {
                        TorrentInfo ti = Commons.Serializer.Deserialize<TorrentInfo>(str);
                        TorrentInfoReceived(ti);
                    }
                    /*lock (tosend)
                    {
                        if (tosend.Count > 0)
                        {
                            string s = tosend.Dequeue();
                            DataWriter dw = new DataWriter(socket.OutputStream);
                            
                            dw.WriteString(s);
                        }
                    }*/
                }
                catch
                {
                    //break;
                }
            }
           
        }
    }
}
