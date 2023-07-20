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
    public class LogViewModel : ViewModelBase
    {
        public LogViewModel()
        {

        }

        #region 数据
        public ObservableCollection<HistoryLogModel> HistoryLogList { get => _historyLogList; set => Set(ref _historyLogList, value); }
        private ObservableCollection<HistoryLogModel> _historyLogList = new ObservableCollection<HistoryLogModel>();
        #endregion

        #region 方法
        public void AddLog(LogType logType,string source,string message)
        {
            
            var logModel = new HistoryLogModel() 
            { 
                AddTime = DateTime.Now, 
                LogType = logType, 
                Source = source,
                Message = message.Replace("\r\n", " ") 
            };
            AddLog(logModel);

        }

        public void AddLog(HistoryLogModel logModel)
        {

            HistoryLogBLL.Add(JsonConvert.SerializeObject(logModel));
            Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                HistoryLogList.Insert(0, logModel);
                if (HistoryLogList.Count > 500)
                {
                    HistoryLogList.Remove(HistoryLogList[500]);
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
            WindowManager.ShowDialog(new HistoryLogViewModel(), null, settings);
        }
        #endregion
    }
}
