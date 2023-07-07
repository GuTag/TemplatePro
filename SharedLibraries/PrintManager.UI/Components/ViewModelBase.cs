using Caliburn.Micro;
using Panuon.WPF;
using PrintManager.Shared;
using PrintManager.Shared.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PrintManager.UI
{
    public class ViewModelBase : Screen
    {
        public IWindowManager WindowManager => IoC.Get<IWindowManager>();

        public IEventAggregator EventAggregator  => IoC.Get<IEventAggregator>();

        public ILogger Logger  => IoC.Get<ILogger>();

        public IThemeManager ThemeManager => IoC.Get<IThemeManager>();

        public EventHandler<object> LogEvent;

        public void LogEventMessage(object logModel)
        {
            LogEvent?.Invoke(this, logModel);
        }
    }
}
