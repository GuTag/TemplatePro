using Caliburn.Micro;
using Panuon.WPF.UI;
using PrintManager.MainClient.ViewModels;
using PrintManager.UI;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PrintManager.MainClient.Views
{
    /// <summary>
    /// ShellView.xaml 的交互逻辑
    /// </summary>
    public partial class ShellView : WindowX
    {
        public ShellView()
        {
            InitializeComponent();
            this.StateChanged += WindowState_Changed;
        }

        private void WindowState_Changed(object sender, EventArgs e)
        {
            //if (this.WindowState == WindowState.Maximized)
            //{
                
            //}
            //else if (this.WindowState == WindowState.Normal)
            //{
                
            //}
        }

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            Minimize();
        }

        private void MaxButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Normalmize();
            }
            else
            {
                Maximize();
            }
            
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            //Close();
            //App.Current.MainWindow.Close();
            Application.Current.Shutdown();
            base.OnClosed(e);
        }

        #region 重写方法
        //protected override void OnClosed(EventArgs e)
        //{
        //    Application.Current.Shutdown();
        //    base.OnClosed(e);
        //}
        #endregion
    }
}
