using Caliburn.Micro;
using PrintManager.OperateClient.ViewModels.Dialog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PrintManager.OperateClient.Components
{
    public class WindowManagerExtension
    {
        public static void ShowMessageDialog(IWindowManager windowManager,string hintText)
        {
            Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                dynamic settings = new ExpandoObject();
                settings.Title = "消息";
                settings.ResizeMode = ResizeMode.CanResize;
                settings.ShowInTaskBar = false;
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                windowManager.ShowDialog(new MessageViewModel(hintText), null, settings);
            }));
        }

        public static bool? ShowAckDialog(IWindowManager windowManager,string title, string hintText)
        {
            dynamic settings = new ExpandoObject();
            settings.Height = 200;
            settings.Width = 500;
            settings.Title = "询问";
            settings.SizeToContent = SizeToContent.Height;
            settings.ResizeMode = ResizeMode.NoResize;
            settings.ShowInTaskBar = false;
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            return windowManager.ShowDialog(new AckViewModel(title, hintText), null, settings);
        }




        //public static void ShowPrintDialog(IWindowManager windowManager, string hintText)
        //{
        //    Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
        //    {
        //        dynamic settings = new ExpandoObject();
        //        settings.Height = 150;
        //        settings.Width = 600;
        //        //settings.Title = "询问";
        //        settings.SizeToContent = SizeToContent.Manual;
        //        settings.ResizeMode = ResizeMode.NoResize;
        //        settings.WindowStyle = WindowStyle.None;
        //        settings.AllowsTransparency = true;
        //        settings.Background = new SolidColorBrush(Colors.Transparent);
        //        settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        //        windowManager.ShowDialog(new PrintAckViewModel(hintText), null, settings);
        //    }));
        //}
    }
}
