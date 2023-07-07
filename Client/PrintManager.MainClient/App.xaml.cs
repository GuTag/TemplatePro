using Hardcodet.Wpf.TaskbarNotification;
using PrintManager.Shared;
using PrintManager.Shared.Utils;
using PrintManager.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PrintManager.MainClient
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : System.Windows.Application
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
            if(!ret)
            {
                MessageBox.Show("程序已经打开", "", MessageBoxButton.OK, MessageBoxImage.Stop);
                Environment.Exit(0);
            }
        }

        //public static TaskbarIcon TaskbarIcon;
        protected override void OnStartup(StartupEventArgs e)
        {
            SplashScreen splashScreen = new SplashScreen("Resources/Images/splashscreen.png");
            splashScreen.Show(true);
            //上面Show()方法中设置为true时，程序启动完成后启动图片就会自动关闭，
            //设置为false时，启动图片不会自动关闭，需要使用下面一句设置显示时间，例如23s
            //splashScreen.Close(new TimeSpan(0, 0, 3));


            //TaskbarIcon = (TaskbarIcon)FindResource("Taskbar");
            //允许输入小数
            FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false; 


            base.OnStartup(e);

            
        }
    }
}
