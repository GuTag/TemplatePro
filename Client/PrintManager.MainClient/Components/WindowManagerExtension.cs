using Caliburn.Micro;
using PrintManager.MainClient.ViewModels.Dialog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PrintManager.MainClient.Components
{
    public class WindowManagerExtension
    {
        public static void ShowMessageDialog(IWindowManager windowManager,object hintText)
        {
            Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                dynamic settings = new ExpandoObject();
                settings.Title = "消息";
                settings.ResizeMode = ResizeMode.CanResize;
                settings.ShowInTaskBar = false;
                settings.Background = new SolidColorBrush(Colors.Transparent);
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                windowManager.ShowDialog(new MessageViewModel(hintText.ToString()), null, settings);
            }));
            
        }

        public static bool? ShowAckDialog(IWindowManager windowManager, string title, string hintText)
        {
            dynamic settings = new ExpandoObject();
            settings.Height = 200;
            settings.Width = 500;
            settings.Title = "询问"; 
            settings.ShowInTaskBar = false;
            settings.SizeToContent = SizeToContent.Height;
            settings.ResizeMode = ResizeMode.NoResize;
            settings.Background = new SolidColorBrush(Colors.Transparent);
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            return windowManager.ShowDialog(new AckViewModel(title, hintText), null, settings);
        }

    }
}
