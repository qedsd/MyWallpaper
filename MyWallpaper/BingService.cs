using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MyWallpaper
{
    public class ImagesItem
    {
        public string Startdate { get; set; }
        public string Fullstartdate { get; set; }
        public string Enddate { get; set; }
        public string Url { get; set; }
        public string Urlbase { get; set; }
        public string Copyright { get; set; }
        public string Copyrightlink { get; set; }
        public string Title { get; set; }
        public string Quiz { get; set; }
        public string Wp { get; set; }
        public string Hsh { get; set; }
        public int Drk { get; set; }
        public int Top { get; set; }
        public int Bot { get; set; }
        public List<string> Hs { get; set; }
        public string UrlWithHost { get; set; }
        public BitmapImage BitmapImage { get; set; }
    }

    public class Tooltips
    {
        public string Loading { get; set; }
        public string Previous { get; set; }
        public string Next { get; set; }
        public string Walle { get; set; }
        public string Walls { get; set; }
    }

    public class Bing
    {
        public List<ImagesItem> Images { get; set; }
        public Tooltips Tooltips { get; set; }
    }


    static class BingService
    {
        

        /// <summary>
        /// 通用网络请求Get
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task<string> HttpClientGetAsync(string uri)
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("User-Agent", "QEDSD");
            HttpResponseMessage response = null;
            try
            {
                response = await http.GetAsync(new Uri(uri));
            }
            catch (Exception)
            {
                http.Dispose();
                return null;
            }
            http.Dispose();
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// API获取bing壁纸
        /// </summary>
        /// <param name="idx">请求图片截止天数 0 今天 -1 截止中明天 （预准备的）1 截止至昨天，类推（目前最多获取到7天前的图片）</param>
        /// <param name="n">1-8 返回请求数量，目前最多一次获取8张</param>
        /// <param name="mkt">地区</param>
        public static async Task<Bing> GetWallInfo(Config config)
        {
            string result = await HttpClientGetAsync("https://cn.bing.com/HPImageArchive.aspx?format=js&idx="+config.BingIdx+"&n="+config.BingN+1+"&mkt="+config.BingRegion);
            if (string.IsNullOrEmpty(result))
                return null;
            Bing bing= Newtonsoft.Json.JsonConvert.DeserializeObject<Bing>(result);
            if (bing == null)
                return null;
            foreach (var temp in bing.Images)
            {
                temp.UrlWithHost = config.BingHost + temp.Url;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(temp.UrlWithHost);
                bitmapImage.DecodePixelWidth = 400;
                bitmapImage.EndInit();
                //bitmapImage.Freeze();

                temp.BitmapImage = bitmapImage;
            }
            return bing;
        }
    }
}
