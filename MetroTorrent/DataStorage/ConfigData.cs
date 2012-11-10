using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;
using Windows.Storage.AccessCache;
using System.Threading;

namespace MetroTorrent.DataStorage
{
    public class ConfigData
    {
        private static ConfigData _instance = null;
        private static Mutex instanceLock = new Mutex();

        public static ConfigData Instance
        {
            get
            {
                instanceLock.WaitOne();
                if (_instance == null)
                    _instance = new ConfigData();
                instanceLock.ReleaseMutex();
                return _instance;
            }
        }

        private UserSettings instance = null;
        private StorageFolder storageFolder;

        public delegate void FirstRunDelegate();
        public event FirstRunDelegate OnFirstRun;

        public delegate void ConfigurationErrorDelegate(string str);
        public event ConfigurationErrorDelegate OnConfigurationError;
        private bool loaded = false;

        private const string filename = "metroconfig.dat";

        public async void Save()
        {
            StorageFile file = await storageFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, instance.ToString());
            //Stream s = file.OpenStreamForWriteAsync().Result;
            //XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
            //serializer.Serialize(s, instance);
        }

        public async void Load()
        {
            instanceLock.WaitOne();
            if (loaded)
            {
                instanceLock.ReleaseMutex();
                return;
            }
            instanceLock.ReleaseMutex();
            instance = new UserSettings();
            try
            {
                StorageFile file = await storageFolder.GetFileAsync(filename);
                string text = await FileIO.ReadTextAsync(file);
                instance.TryParse(text);
                instanceLock.WaitOne();
                loaded = true;
                instanceLock.ReleaseMutex();
                //Stream s = file.OpenStreamForReadAsync().Result;
                //XmlSerializer deserializer = new XmlSerializer(typeof(List<UserSettings>));
                //List<UserSettings> us = (List<UserSettings>)deserializer.Deserialize(s);
                //instance = us[0];
            }
            catch
            {
                IsFirstRun = true;
                if (OnFirstRun != null)
                    OnFirstRun();
            }
            //ApplyPermissions();
        }

        ConfigData()
        {
            storageFolder = KnownFolders.DocumentsLibrary;
            IsFirstRun = false;
        }

        public bool IsFirstRun
        {
            get;
            set;
        }

        public class UserSettings
        {
            public string tempFilePath;
            public string downFilePath;
            public bool startWithWindows = true;
            public int maxup = 0;
            public int maxdown = 0;

            public override string ToString()
            {
                return tempFilePath + "\n" + downFilePath + "\n" + startWithWindows + "\n" + maxup + "\n" + maxdown;
            }

            public bool TryParse(string str)
            {
                string[] arr = str.Split('\n');
                if (arr.Length == 5)
                {
                    tempFilePath = arr[0];
                    downFilePath = arr[1];
                    startWithWindows = bool.Parse(arr[2]);
                    maxup = int.Parse(arr[3]);
                    maxdown = int.Parse(arr[4]);
                    return true;
                }
                return false;
            }
        }

        public bool RunWithOS
        {
            set
            {
                instance.startWithWindows = value;
            }
            get
            {
                return instance.startWithWindows;
            }
        }

        public string TempFilePath
        {
            get
            {
                return instance.tempFilePath;
            }
            set
            {
                instance.tempFilePath = value;
            }
        }
        public string DownFilePath
        {
            get
            {
                return instance.downFilePath;
            }
            set
            {
                instance.downFilePath = value;
            }
        }

        public int MaxUploadSpeed
        {
            get
            {
                return instance.maxup;
            }
            set
            {
                instance.maxup = value;
            }
        }

        public int MaxDownloadSpeed
        {
            get
            {
                return instance.maxdown;
            }
            set
            {
                instance.maxdown = value;
            }
        }

        public async void ApplyPermissions()
        {
            string err = "";
            try
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", await StorageFolder.GetFolderFromPathAsync(TempFilePath));
            }
            catch
            {
                err = "Unable to get writing permission for location " + TempFilePath;
            }
            try
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", await StorageFolder.GetFolderFromPathAsync(DownFilePath));
            }
            catch
            {
                if (err != "")
                    err += "\n";
                err = "Unable to get writing permission for location " + DownFilePath;
            }
            if (err != "" && OnConfigurationError != null && !IsFirstRun)
                OnConfigurationError(err);
        }

    }
}
