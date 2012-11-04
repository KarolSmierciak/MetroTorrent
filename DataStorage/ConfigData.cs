using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;

namespace MetroTorrent.DataStorage
{
    public class ConfigData
    {
        private static UserSettings instance = null;
        private static StorageFolder storageFolder;

        private const string filename = "metroconfig.dat";

        public async static void Save()
        {
            StorageFile file = await storageFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, instance.ToString());
            //Stream s = file.OpenStreamForWriteAsync().Result;
            //XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
            //serializer.Serialize(s, instance);
        }

        public async static void Load()
        {
            instance = new UserSettings();
            try
            {
                StorageFile file = await storageFolder.GetFileAsync(filename);
                string text = await FileIO.ReadTextAsync(file);
                instance.TryParse(text);
                //Stream s = file.OpenStreamForReadAsync().Result;
                //XmlSerializer deserializer = new XmlSerializer(typeof(List<UserSettings>));
                //List<UserSettings> us = (List<UserSettings>)deserializer.Deserialize(s);
                //instance = us[0];
            }
            catch
            {
                IsFirstRun = true;
            }
        }

        static ConfigData()
        {
            storageFolder = KnownFolders.DocumentsLibrary;
            IsFirstRun = false;
        }

        public static bool IsFirstRun
        {
            get;
            set;
        }

        public class UserSettings
        {
            public string tempFilePath = KnownFolders.DocumentsLibrary.Path;
            public string downFilePath = KnownFolders.DocumentsLibrary.Path;
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

        public static bool RunWithOS
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

        public static string TempFilePath
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
        public static string DownFilePath
        {
            get
            {
                return instance.downFilePath;;
            }
            set
            {
                instance.downFilePath = value;
            }
        }

        public static int MaxUploadSpeed
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

        public static int MaxDownloadSpeed
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

    }
}
