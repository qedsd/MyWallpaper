using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyWallpaper.MyUserControl
{
    /// <summary>
    /// NotifyPopup.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyPopup : UserControl
    {
        private Popup m_Popup;

        private string m_TextBlockContent;
        private string m_IconContent;
        private Color m_IconColor;
        private TimeSpan m_ShowTime;

        private NotifyPopup()
        {
            this.InitializeComponent();
            m_Popup = new Popup();
            
            m_Popup.Child = this;
            this.Loaded += NotifyPopup_Loaded; ;
            this.Unloaded += NotifyPopup_Unloaded; ;
        }

        public NotifyPopup(string content, string iconStr, Color color, TimeSpan showTime) : this()
        {
            this.m_TextBlockContent = content;
            this.m_IconContent = iconStr;
            this.m_IconColor = color == new Color() ? Colors.IndianRed : color;
            this.m_ShowTime = showTime;
        }

        public NotifyPopup(string content, string iconStr = "\xE10A", Color color = new Color()) : this(content, iconStr, color, TimeSpan.FromSeconds(2))
        {
        }

        public void Show()
        {
            this.m_Popup.IsOpen = true;
        }

        private void NotifyPopup_Loaded(object sender, RoutedEventArgs e)
        {
            this.tbNotify.Text = m_TextBlockContent;
            this.TextBlock_icon.Text = m_IconContent;
            this.TextBlock_icon.Foreground = new SolidColorBrush() { Color = m_IconColor };
            Storyboard sbd = Resources["sbOut"] as Storyboard;
            sbd.BeginTime = this.m_ShowTime;
            sbd.Begin();
            sbd.Completed += SbOut_Completed;
            SizeChanged += NotifyPopup_SizeChanged;
        }

        private void NotifyPopup_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Width = e.NewSize.Width;
            this.Height = e.NewSize.Height;
        }

        private void SbOut_Completed(object sender, object e)
        {
            this.m_Popup.IsOpen = false;
        }

        //private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        //{
        //    this.Width = e.Size.Width;
        //    this.Height = e.Size.Height;
        //}

        private void NotifyPopup_Unloaded(object sender, RoutedEventArgs e)
        {
            SizeChanged -= NotifyPopup_SizeChanged;
        }
    }
}
