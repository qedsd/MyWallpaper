using MyWallpaper.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyWallpaper
{
    [Serializable]
    public class Config : INotifyPropertyChanged
    {
        private List<Wallpaper> _Wallpapers;
        public List<Wallpaper> Wallpapers
        {
            get { return _Wallpapers; }
            set
            {
                _Wallpapers = value;
                NotifyPropertyChanged("Wallpapers");
            }
        }
        private string _RegexStr;
        public string RegexStr
        {
            get { return _RegexStr; }
            set
            {
                _RegexStr = value;
                NotifyPropertyChanged("RegexStr");
            }
        }
        private double _Duration;
        public double Duration
        {
            get { return _Duration; }
            set
            {
                _Duration = value;
                NotifyPropertyChanged("Duration");
            }
        }

        /// <summary>
        /// 0 顺序 1 随机
        /// </summary>
        private int _Type;
        public int Type
        {
            get { return _Type; }
            set
            {
                _Type = value;
                NotifyPropertyChanged("Type");
            }
        }

        private int _BingIdx;
        public int BingIdx
        {
            get { return _BingIdx; }
            set
            {
                _BingIdx = value;
                NotifyPropertyChanged("BingIdx");
            }
        }

        private int _BingN;
        public int BingN
        {
            get { return _BingN; }
            set
            {
                _BingN = value;
                NotifyPropertyChanged("BingN");
            }
        }

        private string _BingHost;
        public string BingHost
        {
            get { return _BingHost; }
            set
            {
                _BingHost = value;
                NotifyPropertyChanged("BingHost");
            }
        }

        private string _BingRegion;
        public string BingRegion
        {
            get { return _BingRegion; }
            set
            {
                _BingRegion = value;
                NotifyPropertyChanged("BingRegion");
            }
        }

        /// <summary>
        /// 启用的功能
        /// 0我的 1必应 2聚焦
        /// </summary>
        public ObservableCollection<int> EnableFunctions
        {
            get; set;
        }

        private bool _SetMine;
        public bool SetMine
        {
            get { return _SetMine; }
            set
            {
                _SetMine = value;
                NotifyPropertyChanged("SetMine");
                EnableFunctions.Remove(0);
                if (_SetMine)
                {
                    EnableFunctions.Add(0);
                } 
            }
        }

        private bool _SetBing;
        public bool SetBing
        {
            get { return _SetBing; }
            set
            {
                _SetBing = value;
                NotifyPropertyChanged("SetBing");
                EnableFunctions.Remove(1);
                if (_SetBing)
                    EnableFunctions.Add(1);
            }
        }

        private bool _SetSpotlight;
        public bool SetSpotlight
        {
            get { return _SetSpotlight; }
            set
            {
                _SetSpotlight = value;
                NotifyPropertyChanged("SetSpotlight");
                EnableFunctions.Remove(2);
                if (_SetSpotlight)
                    EnableFunctions.Add(2);
               
            }
        }


        public Config() 
        {
            if(Wallpapers== null)
                Wallpapers = new List<Wallpaper>();
            if(EnableFunctions==null)
                EnableFunctions = new ObservableCollection<int>(); 
        }
        public Config(bool getNew)
        {
            Wallpapers = new List<Wallpaper>();
            RegexStr= @".+(\.jpg|\.png|\.bmp|\.jpeg)";
            Duration = 30;
            Type = 0;
            BingIdx = 0;
            BingN = 0;
            BingHost = "https://cn.bing.com";
            BingRegion = "zh-CN";
            EnableFunctions = new ObservableCollection<int>();
            SetMine = true;
            SetBing = true;
            SetSpotlight = true;
            
        }
        public static bool IsChangedFromUser = false;
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public static Config Load()
        {
            string p = Directory.GetCurrentDirectory();
            Config config = null;
            if (File.Exists("config.xml"))
                config= DeserializeFromXml("config.xml", typeof(Config)) as Config;
            else
                config= new Config(true);
            return config;
        }

        public static object DeserializeFromXml(string path, Type object_type)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(object_type);
                object result = null;
                using (StreamReader reader = new StreamReader(path))
                {
                    result = serializer.Deserialize(reader);
                }
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
