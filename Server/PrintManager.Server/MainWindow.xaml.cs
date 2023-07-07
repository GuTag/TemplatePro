using Panuon.WPF.UI;
using PrintManager.Shared.TCP;
using System;
using System.Collections.Generic;
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

namespace PrintManager.Server
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX
    {
        public MainWindow()
        {
            InitializeComponent();

            TCPServer.Instance.UpdateStatusEvent += TCPServer_UpdateStatusEvent;
            TCPServer.Instance.RecviceMsg += TCPServer_RecviceMsg;
            TCPServer.Instance.Start();
        }

        private void TCPServer_RecviceMsg(TCPRecviceMsg obj)
        {
            ;
        }

        private void TCPServer_UpdateStatusEvent(object sender, TCPEventArgs e)
        {
            
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
