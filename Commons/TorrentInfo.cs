using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Commons
{
    [DataContract(Name = "TorrentInfo", Namespace = "http://www.contoso.com")]
    public class TorrentInfo
    {
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "Progress")]
        public double Progress { get; set; }

        [DataMember(Name = "Down")]
        public int DownloadSpeed { get; set; }

        [DataMember(Name = "Up")]
        public int UploadSpeed { get; set; }

        [DataMember(Name = "ETA")]
        public string ETA { get; set; }

        [DataMember(Name = "Files")]
        public List<string> Files = new List<string>();

        [DataMember(Name = "Seeds")]
        public int Seeds { get; set; }

        [DataMember(Name = "Peers")]
        public int Peers { get; set; }
    }
}
