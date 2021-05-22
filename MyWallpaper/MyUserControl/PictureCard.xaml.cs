using MyWallpaper.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyWallpaper.MyUserControl
{
    /// <summary>
    /// PictureCard.xaml 的交互逻辑
    /// </summary>
    public partial class PictureCard : UserControl
    {
        static double defaultW = 0;
        static double defaultH = 0;
        public string ImgPath = null;
        public Wallpaper Wallpaper = null;
        public PictureCard()
        {
            InitializeComponent();
        }
        public PictureCard(Wallpaper wallpaper)
        {
            Wallpaper = wallpaper;
            InitializeComponent();
            //ImgPath = path;
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            if (wallpaper.IsFloder)
            {
                bitmapImage.UriSource = new Uri(Directory.GetCurrentDirectory() + "\\img\\floder.png");
                Image_Main.Width = 42;
                Image_Main.Height = 42;
            }
            else
            {
                if (!File.Exists(wallpaper.Path))
                {
                    return;
                }
                bitmapImage.UriSource = new Uri(wallpaper.Path);
                Image_Main.Width = 134;
                Image_Main.Height = 134;
            }
            
            bitmapImage.DecodePixelWidth = 400;
            bitmapImage.EndInit();
            //bitmapImage.Freeze();

            defaultW = bitmapImage.Width;
            defaultH = bitmapImage.Height;
            
            Image_Main.Source = bitmapImage;

            if (!wallpaper.IsFloder)
                TextBlock_TipName.Visibility = Visibility.Collapsed;
            else
            {
                TextBlock_TipName.Visibility = Visibility.Visible;
                TextBlock_TipName.Text = System.Web.HttpUtility.UrlDecode(new Uri(wallpaper.Path).Segments.Last().ToUpper(), Encoding.UTF8);
            }
        }

        public static readonly DependencyProperty GetImgSource = DependencyProperty.Register
            (
            //name    要注册的依赖项对象的名称
            "ImgSource",
            //propertyType    该属性的类型，作为类型参考
            typeof(string),
            //ownerType    正在注册依赖项属性的所有者类型，作为类型参考
            typeof(UserControl),
            //defaultMetadata    属性元数据实例。这可以包含一个 PropertyChangedCallback 实现引用。
            new PropertyMetadata(null, new PropertyChangedCallback(SetImgSource))
            );

        private static void SetImgSource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PictureCard pictureCard = d as PictureCard;
            //Config config = Config.Load();
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                Uri uri= new Uri(e.NewValue as string);
                bool isFloder = !uri.Segments.Last().Contains(".");
                //bool isFloder = Regex.IsMatch(uri.OriginalString, config.RegexStr);
                if (isFloder)
                {
                    bitmapImage.UriSource = new Uri(Directory.GetCurrentDirectory() + "\\img\\floder.png");
                }
                else
                    bitmapImage.UriSource = uri;

                bitmapImage.DecodePixelWidth = 400;

                bitmapImage.EndInit();

                //bitmapImage.Freeze();
                
                pictureCard.ImgPath = e.NewValue as string;
                defaultW = bitmapImage.Width;
                defaultH = bitmapImage.Height;
                pictureCard.Image_Main.Source = bitmapImage;

                if (!isFloder)
                    pictureCard.TextBlock_TipName.Visibility = Visibility.Collapsed;
                else
                {
                    pictureCard.TextBlock_TipName.Visibility = Visibility.Visible;
                    pictureCard.TextBlock_TipName.Text = System.Web.HttpUtility.UrlDecode(uri.Segments.Last().ToUpper(), Encoding.UTF8);
                }
            }
            catch(Exception)
            {

            }
            
        }

        public static Bitmap KiResizeImage(Bitmap bmp, int newW, int newH)
        {
            try
            {
                Bitmap b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量 
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new System.Drawing.Rectangle(0, 0, newW, newH), new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }

        public static readonly DependencyProperty GetImgWidth = DependencyProperty.Register
            (
            //name    要注册的依赖项对象的名称
            "ImgWidth",
            //propertyType    该属性的类型，作为类型参考
            typeof(double),
            //ownerType    正在注册依赖项属性的所有者类型，作为类型参考
            typeof(UserControl),
            //defaultMetadata    属性元数据实例。这可以包含一个 PropertyChangedCallback 实现引用。
            new PropertyMetadata(defaultW, new PropertyChangedCallback(SetImgWidth))
            );

        private static void SetImgWidth(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PictureCard pictureCard = d as PictureCard;
            pictureCard.Image_Main.Width = (double)e.NewValue;
        }

        public static readonly DependencyProperty GetImgHeigh = DependencyProperty.Register
            (
            //name    要注册的依赖项对象的名称
            "ImgHeigh",
            //propertyType    该属性的类型，作为类型参考
            typeof(double),
            //ownerType    正在注册依赖项属性的所有者类型，作为类型参考
            typeof(UserControl),
            //defaultMetadata    属性元数据实例。这可以包含一个 PropertyChangedCallback 实现引用。
            new PropertyMetadata(defaultH, new PropertyChangedCallback(SetImgHeigh))
            );

        private static void SetImgHeigh(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PictureCard pictureCard = d as PictureCard;
            pictureCard.Image_Main.Height = (double)e.NewValue;
        }

        public string ImgSource
        {
            get { return (string)GetValue(GetImgSource); }

            set { SetValue(GetImgSource, value); }
        }
        public double ImgWidth
        {
            get { return (double)GetValue(GetImgWidth); }

            set { SetValue(GetImgWidth, value); }
        }
        public double ImgHeigh
        {
            get { return (double)GetValue(GetImgHeigh); }

            set { SetValue(GetImgHeigh, value); }
        }

        private void Button_Main_Click(object sender, RoutedEventArgs e)
        {
            WhenMainButtonClick(this);
        }
        public delegate void MainButtonClick(PictureCard pictureCard);
        public event MainButtonClick OnMainButtonClick;
        private void WhenMainButtonClick(PictureCard pictureCard)
        {
            OnMainButtonClick?.Invoke(pictureCard);
        }

        //private void Button_Main_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    StackPanel_Button.Visibility = Visibility.Visible;
        //}

        //private void Button_Main_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    StackPanel_Button.Visibility = Visibility.Collapsed;
        //}
        public delegate void BrowerClick(PictureCard pictureCard);
        public event BrowerClick OnBrowerClick;
        private void WhenBrowerClick(PictureCard pictureCard)
        {
            OnBrowerClick?.Invoke(pictureCard);
        }

        public delegate void DelClick(PictureCard pictureCard);
        public event DelClick OnDelClick;
        private void WhenDelClick(PictureCard pictureCard)
        {
            OnDelClick?.Invoke(pictureCard);
        }

        public void MenuItem_Del_Click(object sender, RoutedEventArgs e)
        {
            WhenDelClick(this);
        }

        public void MenuItem_Brower_Click(object sender, RoutedEventArgs e)
        {
            WhenBrowerClick(this);
        }
    }
}
