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
            List<TorrentManager> torrents = new List<TorrentManager>();
            TorrentListSender sender = new TorrentListSender(torrents);
            Serializator.Serialize(sender);
        }
    }
}
