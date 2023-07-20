using Caliburn.Micro;
using Newtonsoft.Json;
using PrintManager.MainClient.Models;
using PrintManager.Shared.Enums;
using PrintManager.Sql.BLL;
using PrintManager.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PrintManager.MainClient.ViewModels.Shell
{
    public class EventLogViewModel : ViewModelBase
    {
        public EventLogViewModel()
        {

        }

        #region 数据
        public ObservableCollection<EventLogModel> EventLogList { get => _eventLogList; set => Set(ref _eventLogList, value); }
        private ObservableCollection<EventLogModel> _eventLogList = new ObservableCollection<EventLogModel>();
        #endregion

        #region 方法
        //public void AddLog(LogType logType,string source,string message)
        //{
            
        //    var logModel = new EventLogModel() 
        //    { 
        //        AddTime = DateTime.Now, 
        //        LogType = logType, 

        //        Message = message.Replace("\r\n", " ") 
        //    };
        //    AddLog(logModel);

        //}

        public void AddLog(EventLogModel logModel)
        {
            EventLogBLL.Add(JsonConvert.SerializeObject(logModel));
            Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                EventLogList.Insert(0, logModel);
                if (EventLogList.Count > 500)
                {
                    EventLogList.Remove(EventLogList[500]);
                }
            }));

        }



        #endregion

        #region 命令
        public void HistoryLogCommand()
        {
            dynamic settings = new ExpandoObject();
            settings.Title = "历史记录";
            settings.Height = 600;
            settings.Width = 1000;
            settings.SizeToContent = SizeToContent.Manual;
            settings.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            settings.Owner = null;
            WindowManager.ShowDialog(new EventHistoryLogViewModel(), null, settings);
        }
        #endregion
    }
}
