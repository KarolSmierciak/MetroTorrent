using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace MetroTorrent.DataStorage
{
    public class FileData
    {
        public FileData(string name)
        {
            this.name = name;
        }

        private string name;

        public Image FileImage
        {
            get
            {
                return null;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string FileType
        {
            get
            {
                string[] arr = name.Split('.');
                string ext = arr[arr.Length - 1].ToLower();
                string ret;
                if (ext == "avi" || ext == "mov" || ext == "mov")
                    ret = "Videos";
                else if (ext == "ogg" || ext == "wma" || ext == "wav" || ext == "mp3")
                    ret = "Audios";
                else if (ext == "jpg" || ext == "png" || ext == "bmp" || ext == "psd")
                    ret = "Images";
                else
                    ret = "Others";
                return ret;
            }
        }
    }
}
