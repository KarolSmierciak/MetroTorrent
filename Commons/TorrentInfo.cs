namespace Commons
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents information about a single torrent task.
    /// </summary>
    [DataContract(Name = "TorrentInfo")]
    public class TorrentInfo
    {
        /// <summary>
        /// Gets or sets the name for the torrent.
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the progress for the torrent.
        /// </summary>
        [DataMember(Name = "Progress")]
        public double Progress { get; set; }

        /// <summary>
        /// Gets or sets the download speed for the torrent.
        /// </summary>
        [DataMember(Name = "Down")]
        public int DownloadSpeed { get; set; }

        /// <summary>
        /// Gets or sets the upload speed for the torrent.
        /// </summary>
        [DataMember(Name = "Up")]
        public int UploadSpeed { get; set; }

        /// <summary>
        /// Gets or sets the estimated time of arrival for the torrent.
        /// </summary>
        [DataMember(Name = "ETA")]
        public string ETA { get; set; }

        /// <summary>
        /// Gets or sets the files collection for the torrent.
        /// </summary>
        [DataMember(Name = "Files")]
        public List<string> Files = new List<string>();

        /// <summary>
        /// Gets or sets the seeds for the torrent.
        /// </summary>
        [DataMember(Name = "Seeds")]
        public int Seeds { get; set; }

        /// <summary>
        /// Gets or sets the peers for the torrent.
        /// </summary>
        [DataMember(Name = "Peers")]
        public int Peers { get; set; }
    }
}
