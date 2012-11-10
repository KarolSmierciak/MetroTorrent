namespace MetroTorrent.DataStorage
{
    public class FileData
    {
        public FileData(string name)
        {
            this.name = name;
        }

        private string name;

        public string FileImage
        {
            get
            {
                switch (FileType)
                {
                    case "Audios":
                        return "/Assets/audio.png";
                    case "Videos":
                        return "/Assets/video.png";
                    case "Images":
                        return "/Assets/image.png";
                    default:
                        return "/Assets/other.png";
                }
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
                if (ext == "avi" || ext == "mov")
                    ret = "Videos";
                else if (ext == "ogg" || ext == "wma" || ext == "wav" || ext == "mp3")
                    ret = "Audios";
                else if (ext == "jpg" || ext == "png" || ext == "bmp" || ext == "psd" || ext == "jpeg")
                    ret = "Images";
                else
                    ret = "Others";
                return ret;
            }
        }
    }
}
