using Panuon.WPF;
using PrintManager.Sql.Models;
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

namespace PrintManager.UI.Controls
{
    /// <summary>
    /// PagePicker.xaml 的交互逻辑
    /// </summary>
    public partial class ParModifyPicker : UserControl
    {
        public ParModifyPicker()
        {
            InitializeComponent();


        }


        //注册事件
        public PageModel ParModifyPageData
        {
            get { return (PageModel)GetValue(PageDataProperty); }
            set { SetValue(PageDataProperty, value); }
        }

        public static readonly DependencyProperty PageDataProperty =
            DependencyProperty.Register("ParModifyPageData", typeof(PageModel), typeof(PagePicker), new FrameworkPropertyMetadata(new PageModel(), ItemSouceChangeCallBack));

        private static void ItemSouceChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var pg = d as PagePicker;
        }


        #region 属性
        public class PageModel : NotifyPropertyChangedBase
        {
            public string ActualValue { get => _actualValue; set => Set(ref _actualValue, value); }
            private string _actualValue;
            public string Des { get => _des; set => Set(ref _des, value); }
            private string _des;


            public void ActualItem(ParModify par)
            {
                ActualValue = par.ActualValue;
                Des = par.NodeDes;

            }
        }
        #endregion




    }
}
