using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentServer
{
    class Server
    {
        private cpListener tcpListener;

        public void Listen()
        {
            this.tcpListener = new TcpListener(IPAddress.Any, 60606);
            Thread listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.Start();
        }
    }
}
