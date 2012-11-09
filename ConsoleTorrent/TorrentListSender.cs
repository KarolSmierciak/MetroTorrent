namespace ConsoleTorrent
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using MonoTorrent.Client;

    /// <summary>
    /// Represents a torrent list sender, communicating with the Windows 8 UI application.
    /// </summary>
    [DataContract]
    class TorrentListSender
    {
        /* Fields */

        /// <summary>
        /// List of loaded torrents.
        /// </summary>
        [DataMember]
        private List<TorrentManager> torrents;

        /* Constructor */

        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentListSender" /> class.
        /// </summary>
        public TorrentListSender(List<TorrentManager> torrents)
        {
            this.torrents = new List<TorrentManager>();
        }

        /* Properties */

        /// <summary>
        /// Gets the list of loaded torrents.
        /// </summary>
        public List<TorrentManager> Torrents
        {
            get { return this.torrents; }
            private set { this.torrents = value; }
        }

        /* Methods */

        /// <summary>
        /// Performs serialization of the torrents list.
        /// </summary>
        /// <returns></returns>
        private bool Serialize()
        {
            return true;
        }
    }
}
