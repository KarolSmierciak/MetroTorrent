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
        private static string port1 = "60606";
        private static string port2 = "60607";

        public delegate void TorrentInfoReceinvedHandler(TorrentInfo ti);
        public event TorrentInfoReceinvedHandler TorrentInfoReceived;

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

        public async void SendMessage(string msg)
        {
            HostName hostName;
            try
            {
                hostName = new HostName("localhost");
            }
            catch (ArgumentException)
            {
                return;
            }
            StreamSocket socket;
            try
            {
                using (socket = new StreamSocket())
                {
                    await socket.ConnectAsync(hostName, port2);
                    //CoreApplication.Properties.Add("connected", null);
                    DataWriter dw = new DataWriter(socket.OutputStream);
                    dw.WriteString(msg);
                    await dw.StoreAsync();
                }
            }
            catch
            {
                //break;
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
                    await socket.ConnectAsync(hostName, port1);
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
                }
                catch
                {
                    //break;
                }
            }
           
        }
    }
}
