namespace ConsoleTorrent
{
    using System.Collections.Generic;
    using MonoTorrent.Client;

    /// <summary>
    /// Represents the program itself.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main entry point for the application.
        /// </summary>
        /// <param name="args">Arguments for the application.</param>
        static void Main(string[] args)
        {
            int port;
            TcpServer server;
            if (args.Length > 1 && int.TryParse(args[1], out port))
            {
                server = new TcpServer(port);
            }
            else
            {
                server = new TcpServer();
            }
            server.Initialize();
            server.Listen();
        }
    }
}
