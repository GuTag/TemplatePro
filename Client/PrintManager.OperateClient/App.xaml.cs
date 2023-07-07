using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PrintManager.OperateClient
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private Mutex mutex;
        public App()
        {
            this.Startup += new StartupEventHandler(Startup_EventHandler);

#if DEBUG
            this.Startup -= new StartupEventHandler(Startup_EventHandler);
#endif
        }

        private void Startup_EventHandler(object sender, StartupEventArgs e)
        {
            bool ret;
            mutex = new Mutex(true, "PrintManager.MainClient", out ret);
            if (!ret)
            {
                MessageBox.Show("程序已经打开", "", MessageBoxButton.OK, MessageBoxImage.Stop);
                Environment.Exit(0);
            }
        }
    }
    
}
