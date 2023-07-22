using Panuon.WPF;
using System;
using System.Collections.Generic;
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

namespace PrintManager.MainClient.Views.Controls
{
    /// <summary>
    /// PagePicker.xaml 的交互逻辑
    /// </summary>
    public partial class PagePicker : UserControl
    {
        public PagePicker()
        {
            InitializeComponent();


        }


        #region 属性

        public PageModel PageData
        {
            get { return (PageModel)GetValue(PageDataProperty); }
            set { SetValue(PageDataProperty, value); }
        }

        public static readonly DependencyProperty PageDataProperty =
            DependencyProperty.Register("PageData", typeof(PageModel), typeof(PagePicker), new FrameworkPropertyMetadata(new PageModel(), ItemSouceChangeCallBack));

        private static void ItemSouceChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var pg = d as PagePicker;
        }


        #endregion

        public class PageModel : NotifyPropertyChangedBase
        {

            public Action PageChangeEvent;



            #region 数据

            public int Pages { get => _pages; set => Set(ref _pages, value); }
            private int _pages;

            public int Page { get => _page; set => Set(ref _page, value); }
            private int _page = 1;

            public int PageSize { get => _pageSize; set { Set(ref _pageSize, value); PageChange(0); } }
            private int _pageSize = 50;

            public int Totals
            {
                get => _totals;
                set
                {
                    Set(ref _totals, value);
                    var per = _totals / _pageSize;
                    Pages = _totals % _pageSize == 0 ? per : per + 1;
                }
            }
            private int _totals;

            public List<int> PageSizeList { get; set; } = new List<int>() { 20, 50, 100, 200, 500, 1000 };





            #endregion

            #region 方法

            private void PageChange(int interval)
            {
                if (interval == 0 || Pages == 0 || Page + interval < 1)
                {
                    Page = 1;
                }
                else if (interval == Pages || Page + interval > Pages)
                {
                    Page = Pages;
                }
                else
                {
                    Page += interval;
                }
                PageChangeEvent?.Invoke();
            }


            #endregion

            #region 命令

            public void onPageChangeButtonClick(string interval)
            {
                try
                {
                    PageChange(int.Parse(interval));
                }
                catch (Exception)
                {

                }
            }

            //public ICommand FirstPageCommand { get => new DelegateCommand(() => { PageChange(1); }); }
            //public ICommand LastPageCommand { get => new DelegateCommand(() => { PageChange(Pages); }); }
            //public ICommand Fore10PageCommand { get => new DelegateCommand(() => { PageChange(-10); }); }
            //public ICommand Fore1PageCommand { get => new DelegateCommand(() => { PageChange(-1); }); }
            //public ICommand Back10PageCommand { get => new DelegateCommand(() => { PageChange(10); }); }
            //public ICommand Back1PageCommand { get => new DelegateCommand(() => { PageChange(1); }); }

            #endregion

        }
    }
}
