using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
    /// AdaGrid.xaml 的交互逻辑
    /// </summary>
    public partial class AdaGrid : UserControl
    {
        ObservableCollection<PictureCard> Items;
        public AdaGrid()
        {
            InitializeComponent();
            this.SizeChanged += AdaGrid_SizeChanged;
        }

        private void AdaGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ReSetItems(this);
        }

        public static readonly DependencyProperty GetItemSource = DependencyProperty.Register
            (
            //name    要注册的依赖项对象的名称
            "ItemSource",
            //propertyType    该属性的类型，作为类型参考
            typeof(ObservableCollection<PictureCard>),
            //ownerType    正在注册依赖项属性的所有者类型，作为类型参考
            typeof(UserControl),
            //defaultMetadata    属性元数据实例。这可以包含一个 PropertyChangedCallback 实现引用。
            new PropertyMetadata(null, new PropertyChangedCallback(SetItemSource))
            );

        private static void SetItemSource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AdaGrid adaGrid = d as AdaGrid;
            adaGrid.Items = e.NewValue as ObservableCollection<PictureCard>;
            ReSetItems(adaGrid);
            adaGrid.Items.CollectionChanged += adaGrid.Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ReSetItems(this);
        }

        static void ReSetItems(AdaGrid adaGrid)
        {
            adaGrid.Grid_Main.Children.Clear();
            if (adaGrid.Items == null || adaGrid.Items.Count == 0)
                return;
            //断开父元素
            foreach(var temp in adaGrid.Items)
            {
                if (temp.Parent != null)
                {
                    StackPanel p = temp.Parent as StackPanel;
                    p.Children.Clear();
                    p = null;
                }
            }
            double grdiW = adaGrid.ActualWidth;
            int columnCount = (int)(grdiW / (adaGrid.ItemWidth + 10));
            for (int i = 0; i < adaGrid.Items.Count; i++)
            {
                StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                for (int j = 0; j < columnCount && i < adaGrid.Items.Count; j++, i++)
                {
                    stackPanel.Children.Add(adaGrid.Items[i]);
                }
                i--;
                adaGrid.Grid_Main.Children.Add(stackPanel);
                //}

                //if (adaGrid.Grid_Main.Children.Count % columnCount == 0)
                //    adaGrid.Grid_Main.Children.Add();
            }
        }

        public static readonly DependencyProperty GetItemWidth = DependencyProperty.Register
            (
            //name    要注册的依赖项对象的名称
            "ItemWidth",
            //propertyType    该属性的类型，作为类型参考
            typeof(double),
            //ownerType    正在注册依赖项属性的所有者类型，作为类型参考
            typeof(UserControl)
            );

        public static readonly DependencyProperty GetItemHeight = DependencyProperty.Register
            (
            //name    要注册的依赖项对象的名称
            "ItemHeigh",
            //propertyType    该属性的类型，作为类型参考
            typeof(double),
            //ownerType    正在注册依赖项属性的所有者类型，作为类型参考
            typeof(UserControl)
            );


        public ObservableCollection<PictureCard> ItemSource
        {
            get { return (ObservableCollection<PictureCard>)GetValue(GetItemSource); }

            set { SetValue(GetItemSource, value); }
        }
        public double ItemWidth
        {
            get { return (double)GetValue(GetItemWidth); }

            set { SetValue(GetItemWidth, value); }
        }
        public double ItemHeight
        {
            get { return (double)GetValue(GetItemHeight); }

            set { SetValue(GetItemHeight, value); }
        }
    }
}
