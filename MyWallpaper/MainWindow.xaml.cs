using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using MyWallpaper.Model;
using MyWallpaper.MyUserControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
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

namespace MyWallpaper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<PictureCard> pictureCards = new ObservableCollection<PictureCard>();
        Config Config = null;
        public MainWindow()
        {
            InitializeComponent();
            Config=Config.Load();
            Config.IsChangedFromUser = true;
            TextBox_Duration.Text = Config.Duration.ToString();
            ComboBox_ChangeType.SelectedIndex = Config.Type;
            TextBox_Filter.Text = Config.RegexStr;
            LoadImgs();
            Loaded += MainWindow_Loaded;
            pictureCards.CollectionChanged += PictureCards_CollectionChanged;
        }

        private void PictureCards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Config.Wallpapers.Clear();
            for(int i=0;i<pictureCards.Count-1;i++)
                Config.Wallpapers.Add(pictureCards[i].Wallpaper);
            Config.Save();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_mini_click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_max_click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                Button_max.ToolTip = "最大化";
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                Button_max.ToolTip = "正常化";
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetBg(Directory.GetCurrentDirectory() + "\\img\\bg.jpg");

            LoadMain();
            LoadBing();
            LoadSpotlight();

            var pr = Process.GetProcessesByName("MyWallpaperService");
            if(pr.Length==0)
            {
                Button_StartService.Content = "启用服务";
            }
            else
                Button_StartService.Content = "关闭服务";
        }

        void LoadMain()
        {
            pictureCards.Add(new PictureCard() { ImgSource = Directory.GetCurrentDirectory() + "\\img\\plus.png", ImgWidth = 42, ImgHeigh = 42 });
            //pictureCards.Add(new PictureCard(new Wallpaper() { Path = Directory.GetCurrentDirectory() + "\\img\\plus.png", IsFloder = true }));
            pictureCards.Last().OnMainButtonClick += AddNewPictureByExproer;
            pictureCards.Last().Grid_Main.ContextMenu.Items.Clear();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "手动输入路径（支持网络资源)";
            menuItem.Click += MenuItem_Click;
            MenuItem menuItem2 = new MenuItem();
            menuItem2.Header = "添加文件夹";
            menuItem2.Click += MenuItem2_Click;
            pictureCards.Last().Grid_Main.ContextMenu.Items.Add(menuItem2);
            pictureCards.Last().Grid_Main.ContextMenu.Items.Add(menuItem);
            AdaGrid_Pics.ItemWidth = 134;
            AdaGrid_Pics.ItemHeight = 134;
            AdaGrid_Pics.ItemSource = pictureCards;
        }

        async void LoadBing()
        {
            //ProgressBar_Loading.Visibility = Visibility = Visibility.Visible;
            var bings = await BingService.GetWallInfo(Config);
            if (bings != null)
                ListBox_Bings.ItemsSource = bings.Images;
            //ProgressBar_Loading.Visibility = Visibility = Visibility.Collapsed;

            ComboBox_Bing_Idx.DataContext = Config;
            ComboBox_Bing_N.DataContext = Config;
            TextBox_Bing_Region.DataContext = Config;
            TextBox_Bing_Host.DataContext = Config;
            TextBox_Duration.DataContext = Config;
            ComboBox_ChangeType.DataContext = Config;
            TextBox_Filter.DataContext = Config;
            ListBox_EnableFunc.DataContext = Config;
        }

        void LoadSpotlight()
        {
            ListBox_Spotlights.ItemsSource = WallpaperService.GetSpotlight();
        }

        

        void LoadImgs()
        {
            if (Config.Wallpapers.Count==0)
                return;
            foreach (var temp in Config.Wallpapers)
            {
                PictureCard newItem = new PictureCard(temp);
                //newItem.ImgSource = temp;
                //if (temp.Contains("."))
                //{
                //    newItem.ImgWidth = 134;
                //    newItem.ImgHeigh = 134;
                //}
                //else
                //{
                //    newItem.ImgWidth = 42;
                //    newItem.ImgHeigh = 42;
                //}

                newItem.OnBrowerClick += NewItem_OnBrowerClick;
                newItem.OnDelClick += NewItem_OnDelClick;
                newItem.OnMainButtonClick += NewItem_OnMainButtonClick;
                pictureCards.Add(newItem);
            }
        }

        private void MenuItem2_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;//设置为选择文件夹
            if(dialog.ShowDialog()==CommonFileDialogResult.Ok)
            {
                string folderPath = dialog.FileName;
                var t = pictureCards.Last();
                pictureCards.Remove(t);
                //PictureCard newItem = new PictureCard() { ImgSource = folderPath, ImgWidth = 42, ImgHeigh = 42 };
                PictureCard newItem = new PictureCard(new Wallpaper() { Path = folderPath, IsFloder = true });
                newItem.OnBrowerClick += NewItem_OnBrowerClick;
                newItem.OnDelClick += NewItem_OnDelClick;
                newItem.OnMainButtonClick += NewItem_OnMainButtonClick;
                pictureCards.Add(newItem);
                pictureCards.Add(t);
            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DialogHost_AddImgPath.IsOpen = true;
        }

        private void AddNewPictureByExproer(PictureCard pictureCard)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "全部文件|*.*|图片(.png)|*.png|图片(.jpg)|*.jpg",
                Multiselect=true
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                if (!IsRealImage(openFileDialog.FileName))
                    return;
                var t = pictureCards.Last();
                pictureCards.Remove(t);
                foreach (var temp in openFileDialog.FileNames)
                {
                    //PictureCard newItem = new PictureCard() { ImgSource = temp, ImgWidth = 134, ImgHeigh = 134 };
                    PictureCard newItem = new PictureCard(new Wallpaper() { Path = temp, IsFloder = false });
                    newItem.OnBrowerClick += NewItem_OnBrowerClick;
                    newItem.OnDelClick += NewItem_OnDelClick;
                    newItem.OnMainButtonClick += NewItem_OnMainButtonClick;
                    pictureCards.Add(newItem);
                }
                pictureCards.Add(t);
            }
        }

        private void NewItem_OnMainButtonClick(PictureCard pictureCard)
        {
            NewItem_OnBrowerClick(pictureCard);
        }

        private void NewItem_OnBrowerClick(PictureCard pictureCard)
        {
            Process.Start("explorer.exe", pictureCard.Wallpaper.Path);
        }

        private void NewItem_OnDelClick(PictureCard pictureCard)
        {
            pictureCards.Remove(pictureCard);
        }

        /// <summary>
        /// 由指定路径设置背景
        /// </summary>
        /// <param name="path"></param>
        void SetBg(string path)
        {
            ImageBrush b = new ImageBrush();
            
            var image = new BitmapImage(new Uri(path));
            //image.DecodePixelWidth = 100;
            b.ImageSource = image;
            b.Stretch = Stretch.UniformToFill;
            this.Background = b;
        }

        private void AdaGrid_Pics_Drop(object sender, DragEventArgs e)
        {
            AdaGrid_Pics.Opacity = 1;
            TextBlock_DropTip.Visibility = Visibility.Collapsed;
            string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);
            var t = pictureCards.Last();
            pictureCards.Remove(t);
            foreach (var temp in data)
            {
                if(IsRealImage(temp))
                {

                    //PictureCard newItem = new PictureCard() { ImgSource = temp, ImgWidth = 134, ImgHeigh = 134 };
                    PictureCard newItem = new PictureCard(new Wallpaper() { Path=temp,IsFloder=false});
                    newItem.OnBrowerClick += NewItem_OnBrowerClick;
                    newItem.OnDelClick += NewItem_OnDelClick;
                    newItem.OnMainButtonClick += NewItem_OnMainButtonClick;
                    pictureCards.Add(newItem);
                    
                }
            }
            pictureCards.Add(t);
        }

        public static bool IsRealImage(string path)
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri(path));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void AdaGrid_Pics_PreviewDrop(object sender, DragEventArgs e)
        {
            
        }

        private void AdaGrid_Pics_DragOver(object sender, DragEventArgs e)
        {
            AdaGrid_Pics.Opacity = 0.1;
            TextBlock_DropTip.Visibility = Visibility.Visible;
        }

        private void AdaGrid_Pics_Leave(object sender, DragEventArgs e)
        {
            AdaGrid_Pics.Opacity = 1;
            TextBlock_DropTip.Visibility = Visibility.Collapsed;
        }

        private void Button_CancelAddPic_Click(object sender, RoutedEventArgs e)
        {
            DialogHost_AddImgPath.IsOpen = false;
        }

        private void Button_ConfirmAddPic_Click(object sender, RoutedEventArgs e)
        {
            var t = pictureCards.Last();
            pictureCards.Remove(t);
            //PictureCard newItem = new PictureCard() { ImgSource = FruitTextBox.Text, ImgWidth = 134, ImgHeigh = 134 };
            PictureCard newItem = new PictureCard(new Wallpaper() { Path= FruitTextBox.Text,IsFloder=false});
            newItem.OnBrowerClick += NewItem_OnBrowerClick;
            newItem.OnDelClick += NewItem_OnDelClick;
            newItem.OnMainButtonClick += NewItem_OnMainButtonClick;
            pictureCards.Add(newItem);
            pictureCards.Add(t);
            DialogHost_AddImgPath.IsOpen = false;
        }

        private void TextBox_Duration_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Config == null)
                return;
            double d;
            if (double.TryParse((sender as TextBox).Text, out d))
                Config.Duration = d;
            Config.Save();
        }

        private void ComboBox_ChangeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Config == null)
                return;
            Config.Type = (sender as ComboBox).SelectedIndex;
            Config.Save();
        }

        private void Button_StartService_Click(object sender, RoutedEventArgs e)
        {
            //if(Button_StartService.Tag.ToString()=="0")
            //{
            //    WallpaperService.IsDone = false;
            //    Task.Run(() => WallpaperService.StartService());
            //    Button_StartService.Content = "关闭服务";
            //    Button_StartService.Tag = "1";
            //}
            //else
            //{
            //    WallpaperService.IsDone = true;
            //    Button_StartService.Content = "开启服务";
            //    Button_StartService.Tag = "0";
            //}
            //ServiceController[] service = ServiceController.GetServices();
            //var found = service.FirstOrDefault(p => p.DisplayName == "MyWallpaper");
            //if (found == null)
            //{
            //    //Process.Start("sc.exe", "create MyServiceName binPath="+ Directory.GetCurrentDirectory()+"\\workerservice\\MyWallpaperService.exe");

            //}
            //else if (found.Status == ServiceControllerStatus.Running)
            //    found.Stop();
            //else
            //    found.Start();
            //List<string> p = new List<string>();
            //p.Add("cd "+Directory.GetCurrentDirectory()+"\\workerservice");
            //p.Add("MyWallpaperService.exe");
            //Process.Start("cmd.exe",  p);
            var pr=Process.GetProcessesByName("MyWallpaperService");
            if(pr.Length==0)
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo(Directory.GetCurrentDirectory() + "\\workerservice\\MyWallpaperService.exe");
                //Process.Start(Directory.GetCurrentDirectory() + "\\workerservice\\MyWallpaperService.exe");
                //processStartInfo.UseShellExecute = true;
                Process.Start(processStartInfo);
                Button_StartService.Content = "关闭服务";

                //添加自启
                //string StartupPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
                //string exeDir = Directory.GetCurrentDirectory() + "\\workerservice\\MyWallpaperService.exe";
                //File.Copy(exeDir, StartupPath + @"\MyWallpaperService.exe.lnk", true);

                string path = Directory.GetCurrentDirectory() + "\\MyWallpaperServiceSetup.bat";
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                rk2.SetValue("MyWallpaperService", path);
                rk2.Close();
                rk.Close();
            }
            else
            {
                pr[0].Kill();
                Button_StartService.Content = "启用服务";

                //关闭自启
                //string StartupPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
                //File.Delete(StartupPath + @"\MyWallpaperService.exe.lnk");

                string path = Directory.GetCurrentDirectory() + "\\MyWallpaperServiceSetup.bat";
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                rk2.DeleteValue("MyWallpaperService", false);
                rk2.Close();
                rk.Close();
            }
        }

        private void TextBox_Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Config == null)
                return;
            Config.RegexStr = (sender as TextBox).Text;
            Config.Save();
        }

        private async void TextBox_Bing_Region_TextChanged(object sender, TextChangedEventArgs e)
        {
            var bings = await BingService.GetWallInfo(Config);
            ListBox_Bings.ItemsSource = bings?.Images;
        }

        private async void ComboBox__Bing_Idx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var bings = await BingService.GetWallInfo(Config);
            ListBox_Bings.ItemsSource = bings?.Images;
        }

        private async void ComboBox_Bing_N_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var bings = await BingService.GetWallInfo(Config);
            ListBox_Bings.ItemsSource = bings?.Images;
        }

        private async void TextBox_Bing_Host_TextChanged(object sender, TextChangedEventArgs e)
        {
            var bings = await BingService.GetWallInfo(Config);
            ListBox_Bings.ItemsSource = bings?.Images;
        }

        private void ListBox_Bings_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //无法调用浏览器打开使用参数指定图片的uri
            return;
            //System.Diagnostics.Process.Start("explorer.exe", ((sender as ListBox).SelectedItem as ImagesItem).UrlWithHost);
            //http://src.qedsd.club/qedsd.jpg
            //Process.Start("explorer.exe", ((sender as ListBox).SelectedItem as ImagesItem).UrlWithHost);
            Uri uri = new Uri(((sender as ListBox).SelectedItem as ImagesItem).UrlWithHost);
            //var array = ((sender as ListBox).SelectedItem as ImagesItem).Url.Split("?");
            //if(array.Length)
            var array2 = uri.Query.Split('?','&');
            var array1 = ((sender as ListBox).SelectedItem as ImagesItem).UrlWithHost.Split("?");
            List<string> ps = new List<string>();
            ps.Add(array1[0]);
            for (int i = 1; i < array2.Length; i++)
                ps.Add(array2[i]);


            //Process.Start("explorer.exe", array1[0], array1[1]);

            string p1=((sender as ListBox).SelectedItem as ImagesItem).UrlWithHost.Split("https://").Last();
            Process.Start("explorer.exe", p1);
        }

        private void MenuItem_CopyBingUri_Click(object sender, RoutedEventArgs e)
        {
            string uri=(((sender as MenuItem).Parent as ContextMenu).DataContext as ImagesItem).UrlWithHost;
            Clipboard.SetText(uri);

        }

        private void MenuItem_CopySpotlightUri_Click(object sender, RoutedEventArgs e)
        {
            string uri = (((sender as MenuItem).Parent as ContextMenu).DataContext as Spotlight).Path;
            Clipboard.SetText(uri);
        }

        private void ListBox_Spotlights_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("explorer.exe", ((sender as ListBox).SelectedItem as Spotlight).Path);
        }
    }
}
