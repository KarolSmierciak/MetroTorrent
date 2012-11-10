namespace ConsoleTorrent
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading;
    using MonoTorrent.BEncoding;
    using MonoTorrent.Client;
    using MonoTorrent.Client.Tracker;
    using MonoTorrent.Common;
    using MonoTorrent.Dht;
    using MonoTorrent.Dht.Listeners;

    /// <summary>
    /// Represents a MetroTorrent client.
    /// </summary>
    public class TorrentsData
    {
        /* Fields */

        /// <summary>
        /// 
        /// </summary>
        public TorrentSettings torrentDefaults;

        /// <summary>
        /// Base directory for the application.
        /// </summary>
        private string baseDir;

        /// <summary>
        /// Directory for complete downloads.
        /// </summary>
        private string downloadDir;

        /// <summary>
        /// Directory for torrent files.
        /// </summary>
        private string torrentsDir;

        /// <summary>
        /// Current upload speed in bytes.
        /// </summary>
        private int uploadSpeed;

        /// <summary>
        /// Current download speed in bytes.
        /// </summary>
        private int downloadSpeed;

        /// <summary>
        /// File used for fast resuming of downloads.
        /// </summary>
        private string fastResumeFile;

        /// <summary>
        /// File used for DHT discovery.
        /// </summary>
        private string dhtNodeFile;

        /// <summary>
        /// 
        /// </summary>
        private ClientEngine clientEngine;

        /// <summary>
        /// Collection of loaded torrents.
        /// </summary>
        private List<TorrentManager> torrentManagers;

        /// <summary>
        /// Collection of top ten trackers.
        /// </summary>
        private TopListeners topTrackers;

        /* Constructor */

        /// <summary>
        /// Initializes a new instance of the <see cref="TorrentsData" /> class.
        /// </summary>
        public TorrentsData()
        {
            this.baseDir = Environment.CurrentDirectory;
            this.GetValues(out this.torrentsDir, out this.downloadDir, out this.uploadSpeed, out this.downloadSpeed);
            this.fastResumeFile = Path.Combine(this.torrentsDir, "fastresume.data");
            this.dhtNodeFile = Path.Combine(this.baseDir, "DhtNodes");
            this.torrentManagers = new List<TorrentManager>();
            this.topTrackers = new TopListeners(10);

            Console.CancelKeyPress += delegate { this.Shutdown(); };
            AppDomain.CurrentDomain.ProcessExit += delegate { this.Shutdown(); };
            AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e) { Console.WriteLine(e.ExceptionObject); this.Shutdown(); };
            Thread.GetDomain().UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e) { Console.WriteLine(e.ExceptionObject); this.Shutdown(); };

            this.InitializeEngine();
            this.torrentDefaults = new TorrentSettings(4, 150, this.downloadSpeed, this.uploadSpeed);
        }

        /* Properties */

        /// <summary>
        /// 
        /// </summary>
        public string DownloadDir
        {
            get { return this.downloadDir; }
            set { this.downloadDir = value; }
        }

        /// <summary>
        /// Gets the collection of loaded torrents.
        /// </summary>
        public List<TorrentManager> TorrentManagers
        {
            get { return this.torrentManagers; }
            private set { this.torrentManagers = value; }
        }

        /// <summary>
        /// Gets the top trackers.
        /// </summary>
        public TopListeners TopTrackers
        {
            get { return this.topTrackers; }
            private set { this.topTrackers = value; }
        }

        /* Methods */

        /// <summary>
        /// Gets paths for new and complete downloads.
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        private void GetValues(out string torrentsDir, out string downloadDir, out int uploadSpeed, out int downloadSpeed)
        {
            try
            {
                using (StreamReader sr = new StreamReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "metroconfig.dat")))
                {
                    String[] line = sr.ReadToEnd().Split('\n');
                    torrentsDir = line[0].Trim();
                    downloadDir = line[1].Trim();
                    int.TryParse(line[3].Trim(), out uploadSpeed);
                    int.TryParse(line[4].Trim(), out downloadSpeed);
                    // The file contained download/upload speeds in kilobytes, multiply by 1024.
                    uploadSpeed *= 1024;
                    downloadSpeed *= 1024;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read: {0}", e.Message);
                torrentsDir = string.Empty;
                downloadDir = string.Empty;
                uploadSpeed = 0;
                downloadSpeed = 0;
            }
        }

        /// <summary>
        /// Shuts down the download action.
        /// </summary>
        private void Shutdown()
        {
            BEncodedDictionary fastResume = new BEncodedDictionary();
            for (int i = 0; i < torrentManagers.Count; ++i)
            {
                torrentManagers[i].Stop();
                while (torrentManagers[i].State != TorrentState.Stopped)
                {
                    Console.WriteLine("{0} is {1}", torrentManagers[i].Torrent.Name, torrentManagers[i].State);
                    Thread.Sleep(250);
                }

                fastResume.Add(torrentManagers[i].Torrent.InfoHash.ToHex(), torrentManagers[i].SaveFastResume().Encode());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeEngine()
        {
            int port = 60606;

            EngineSettings engineSettings = new EngineSettings(downloadDir, port)
            {
                PreferEncryption = false,
                AllowedEncryption = MonoTorrent.Client.Encryption.EncryptionTypes.All
            };

            this.clientEngine = new ClientEngine(engineSettings);

            // Change to loopback, perhaps?
            this.clientEngine.ChangeListenEndpoint(new IPEndPoint(IPAddress.Any, port));

            byte[] nodes = null;

            try
            {
                nodes = File.ReadAllBytes(dhtNodeFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("No existing DHT nodes could be loaded: {0}", e.Message);
            }

            DhtListener dhtListener = new DhtListener(new IPEndPoint(IPAddress.Any, port));
            DhtEngine dhtEngine = new DhtEngine(dhtListener);

            this.clientEngine.RegisterDht(dhtEngine);
            dhtListener.Start();
            this.clientEngine.DhtEngine.Start(nodes);

            // If the SavePath does not exist, we want to create it.
            if (!Directory.Exists(this.clientEngine.Settings.SavePath))
            {
                Directory.CreateDirectory(this.clientEngine.Settings.SavePath);
            }

        }

        public void InitializeTorrent(string torrentDir)
        {
            BEncodedDictionary fastResume;
            try
            {
                fastResume = BEncodedValue.Decode<BEncodedDictionary>(File.ReadAllBytes(this.fastResumeFile));
            }
            catch
            {
                fastResume = new BEncodedDictionary();
            }

            Torrent torrent = null;

            // Load the file into the engine if it ends with .torrent.

            if (torrentDir.EndsWith(".torrent"))
            {
                try
                {
                    // Load the .torrent file from the file into a Torrent instance
                    // You can use this to do preprocessing should you need to.
                    torrent = Torrent.Load(torrentDir);
                    Console.WriteLine(torrent.InfoHash.ToString());
                }
                catch (Exception e)
                {
                    Console.Write("Could not decode {0}: {1}", torrentDir, e.Message);
                    return;
                }

                // When any preprocessing has been completed, you create a TorrentManager which you then register with the engine.
                TorrentManager postProcessManager = new TorrentManager(torrent, this.downloadDir, torrentDefaults);
                if (fastResume.ContainsKey(torrent.InfoHash.ToHex()))
                {
                    postProcessManager.LoadFastResume(new FastResume((BEncodedDictionary)fastResume[torrent.InfoHash.ToHex()]));
                }

                this.clientEngine.Register(postProcessManager);

                // Store the torrent manager in our list so we can access it later.
                this.torrentManagers.Add(postProcessManager);

                postProcessManager.PeersFound += new EventHandler<PeersAddedEventArgs>(this.manager_PeersFound);
            }

            // For each torrent manager we loaded and stored in our list, hook into the events in the torrent manager and start the engine.
            foreach (TorrentManager torrentManager in this.torrentManagers)
            {
                // Every time a piece is hashed, this is fired.
                torrentManager.PieceHashed += delegate(object o, PieceHashedEventArgs e)
                {
                    lock (this.topTrackers)
                    {
                        topTrackers.WriteLine(string.Format("Piece hashed: {0} - {1} ", e.PieceIndex, e.HashPassed ? "passed" : "failed"));
                    }
                };

                // Everytime the state changes (stopped -> seeding -> downloading -> hashing), this is fired.
                foreach (TrackerTier tier in torrentManager.TrackerManager)
                {
                    List<Tracker> trackers = tier.GetTrackers();
                    foreach (Tracker tracker in trackers)
                    {
                        tracker.AnnounceComplete += delegate(object sender, AnnounceResponseEventArgs e)
                        {
                            this.topTrackers.WriteLine(string.Format("{0}: {1}", e.Successful, e.Tracker.ToString()));
                        };
                    }
                }

                // Start the torrentManager. The file will then hash (if required) and begin loading.
                torrentManager.Start();
            }

            Thread.Sleep(500);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void manager_PeersFound(object sender, PeersAddedEventArgs e)
        {
            lock (this.topTrackers)
            {
                this.topTrackers.WriteLine(string.Format("Found {0} new peers and {1} existing peers.", e.NewPeers, e.ExistingPeers));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ChangeListenEndPoint()
        {
        }
    }
}
 