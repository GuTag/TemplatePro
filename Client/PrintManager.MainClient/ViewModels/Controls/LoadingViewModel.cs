using PrintManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PrintManager.MainClient.ViewModels.Controls
{
    public class LoadingViewModel:ViewModelBase
    {
        public LoadingViewModel()
        {
            Loading();
        }

        #region 属性
        public int LoadingValue { get => _loadingValue; set => Set(ref _loadingValue, value); }
        private int _loadingValue;


        private DispatcherTimer mDataTimer = null; //定时器
        #endregion

        #region 事件与命令


        public void Loading()
        {
            //LoadingValue = 0;
            if (mDataTimer == null)
            {
                mDataTimer = new DispatcherTimer();
                mDataTimer.Tick += new EventHandler(DataTimer_Tick);
                mDataTimer.Interval = TimeSpan.FromSeconds(0.01);
                mDataTimer.IsEnabled = true;
                mDataTimer.Start();
                
            }
        }

        public void DataTimer_Tick(object sender, EventArgs e)
        {
            LoadingValue += 5;

            if (LoadingValue >= 100) onCancelCommand(); 
        }

        public void onCancelCommand()
        {
            mDataTimer.Stop();
            TryClose();
        }
        #endregion
    }
}
