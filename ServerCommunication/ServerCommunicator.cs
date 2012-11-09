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
        private static StreamSocket socket = new StreamSocket();
        private static HostName hostName;
        private static string port = "60606";

        public delegate void TorrentInfoReceinvedHandler(TorrentInfo ti);
        public event TorrentInfoReceinvedHandler TorrentInfoReceived;

        private ServerCommunicator()
        {
            try
            {
                hostName = new HostName("localhost");
            }
            catch (ArgumentException)
            {
                return;
            }
            CoreApplication.Properties.Add("clientSocket", socket);
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
            
        }

        public async void StartListening()
        {
            while (true)
            {
                try
                {
                    await socket.ConnectAsync(hostName, port);
                    CoreApplication.Properties.Add("connected", null);
                }
                catch (Exception exception)
                {
                    if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                    {
                        throw;
                    }
                    return;
                }

                Windows.Storage.Streams.Buffer b = new Windows.Storage.Streams.Buffer(6000);

                string str;

                while (true)
                {
                    try
                    {
                        var xxx = await socket.InputStream.ReadAsync(b, b.Capacity, InputStreamOptions.None);
                        DataReader dr = DataReader.FromBuffer(b);
                        str = dr.ReadString(dr.UnconsumedBufferLength);
                        if (this.TorrentInfoReceived != null)
                        {
                            TorrentInfo ti = Commons.Serializer.Deserialize<TorrentInfo>(str);
                            TorrentInfoReceived(ti);
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
            }
           
        }
    }
}
