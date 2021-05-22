using System;
using System.Collections.Generic;
using MyWallpaper;
using MyWallpaper.Model;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;

namespace MyWallpaperService
{
    class Program
    {
        static void Main(string[] args)
        {
            IntoUnknown();
        }

        static void IntoUnknown()
        {
            StreamWriter streamWriter = new StreamWriter("log.txt", true);
            streamWriter.WriteLine(DateTime.Now.ToString() + "启用服务");
            streamWriter.Close();
            streamWriter.Dispose();
            Bing bing = null;
            List<Spotlight> spotlights = null;
            DateTime startDataTime = new DateTime();
            Random random = new Random();
            while (true)
            {
                Config.IsChangedFromUser = false;
                Config config = Config.Load();
                Config.IsChangedFromUser = true;
                imgPaths.Clear();
                //找出全部符合文件
                foreach (var temp in config.EnableFunctions)
                {
                    switch (temp)
                    {
                        case 0://我的
                            {
                                foreach (var temp2 in config.Wallpapers)
                                {
                                    if (temp2.IsFloder)
                                        SearchImg(config, temp2.Path);
                                    else
                                        imgPaths.Add(temp2.Path);
                                }
                            }
                            break;
                        case 1://必应
                            {

                                TimeSpan sp = DateTime.Now.Subtract(startDataTime);
                                if (bing == null || sp.Hours > 3)
                                {
                                    bing = BingService.GetWallInfo(config).Result;
                                }
                                if (bing != null)
                                {
                                    foreach (var temp2 in bing.Images)
                                        imgPaths.Add(temp2.UrlWithHost);
                                }
                            }
                            break;
                        case 2:
                            {
                                TimeSpan sp = DateTime.Now.Subtract(startDataTime);
                                if (spotlights == null || sp.Hours > 3)
                                {
                                    spotlights = GetSpotlight();
                                }
                                if (spotlights != null)
                                {
                                    foreach (var temp2 in spotlights)
                                        imgPaths.Add(temp2.Path);
                                }
                            }
                            break;
                    }
                }

                if (imgHistory.Count == imgPaths.Count)//已全部出现过，重置
                    imgHistory.Clear();
                if (config.Type == 0)
                {
                    int index = 0;
                    if (imgHistory.Count != 0)
                        index = imgPaths.IndexOf(imgHistory.Last()) + 1;
                    SetDestPicture(imgPaths[index]);
                    imgHistory.Add(imgPaths[index]);
                }
                else if (config.Type == 1)
                {
                    int index = -1;
                    do
                    {
                        index = random.Next(0, imgPaths.Count - 1);
                    }
                    while (imgHistory.Contains(imgPaths[index]));
                    SetDestPicture(imgPaths[index]);
                    imgHistory.Add(imgPaths[index]);
                }
                //await Task.Delay((int)(config.Duration * 60 * 1000));
                GC.Collect();
                //GC.WaitForFullGCComplete();
                Thread.Sleep((int)(config.Duration * 60 * 1000));
            }
        }

        public static bool IsDone = false;
        static List<string> imgPaths = new List<string>();
        static List<string> imgHistory = new List<string>();

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(
            int uAction,
            int uParam,
            string lpvParam,
            int fuWinIni
            );

        /// <summary>
        /// 设置背景图片
        /// </summary>
        /// <param name="picture">图片路径</param>
        public static void SetDestPicture(string picture)
        {
            StreamWriter streamWriter = new StreamWriter("log.txt", true);
            streamWriter.WriteLine(DateTime.Now.ToString() + picture);
            streamWriter.Close();
            streamWriter.Dispose();
            if (File.Exists(picture))//本地存在
            {
                if (Path.GetExtension(picture).ToLower() != "bmp")
                {
                    // 其它格式文件先转换为bmp再设置
                    string tempFile = @"img\temp.bmp";
                    Image image = Image.FromFile(picture);
                        image.Save(tempFile, System.Drawing.Imaging.ImageFormat.Bmp);
                    FileInfo fileInfo = new FileInfo(tempFile);
                    _ = SystemParametersInfo(20, 0, fileInfo.FullName, 0x2);
                }
            }
            else//可能为网络图片
            {
                try
                {
                    //BitmapImage bitmapImage = new BitmapImage(new Uri(picture));
                    string tempFile = @"img\temp.bmp";
                    FileInfo tempFile2 = new FileInfo(@"img\download");
                    var httpClient = new HttpClient();
                    var response = httpClient.GetAsync(picture).Result;
                    httpClient.Dispose();
                    if (response.IsSuccessStatusCode == false)
                        return;
                    var stream = response.Content.ReadAsStreamAsync().Result;
                    using (var fileStream = tempFile2.Create())
                    using (stream)
                    {
                        stream.CopyTo(fileStream);
                    }
                    Image image = Image.FromFile(@"img\download");
                    image.Save(tempFile, System.Drawing.Imaging.ImageFormat.Bmp);
                    FileInfo fileInfo = new FileInfo(tempFile);
                    SystemParametersInfo(20, 0, fileInfo.FullName, 0x2);
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// 递归寻找路径下的所有文件
        /// </summary>
        /// <param name="config"></param>
        /// <param name="path"></param>
        static void SearchImg(Config config, string path)
        {
            //if (Regex.IsMatch(path, config.RegexStr))//图片文件
            //{
            //    imgPaths.Add(path);
            //}

            //else if(!Regex.IsMatch(path, @".+(\.)"))//是否为文件夹
            //{
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                return;
            }
            //获取全部符合文件
            FileInfo[] fileInfo = dir.GetFiles();
            foreach (FileInfo item in fileInfo)
            {
                if (Regex.IsMatch(item.Name, config.RegexStr))
                    imgPaths.Add(item.FullName);
            }
            //获取全部文件夹
            var floders = dir.GetDirectories();
            foreach (var temp in floders)
            {
                SearchImg(config, temp.FullName);
            }
            //}
        }

        public static List<Spotlight> GetSpotlight()
        {
            string SpotlightPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";
            DirectoryInfo dir = new DirectoryInfo(SpotlightPath);
            if (!dir.Exists)
            {
                return null;
            }

            FileInfo[] fileInfo = dir.GetFiles();
            List<Spotlight> Spotlights = new List<Spotlight>();
            foreach (var temp in fileInfo)
            {
                try
                {
                    //BitmapImage spotlightImage = new BitmapImage();
                    //spotlightImage.BeginInit();
                    //spotlightImage.UriSource = new Uri(temp.FullName);
                    //spotlightImage.DecodePixelWidth = 400;
                    //spotlightImage.EndInit();
                    Image image = Image.FromFile(temp.FullName);
                    if (image.Width > image.Height)
                    {

                        Spotlights.Add(new Spotlight() { Path = temp.FullName, Name = temp.Name });

                    }
                }
                catch (Exception) { }
            }
            return Spotlights;
        }
    }
}
