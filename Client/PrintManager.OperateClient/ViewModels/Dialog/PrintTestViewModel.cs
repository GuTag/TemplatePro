using Newtonsoft.Json;
using PrintManager.OperateClient.Models;
using PrintManager.Shared.TCP;
using PrintManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PrintManager.UI.Controls.PrintCanvas;
using System.Windows;
using System.Runtime.Remoting.Contexts;
using System.Windows.Input;
using Caliburn.Micro;
using PrintManager.OperateClient.Components;
using System.Diagnostics;

namespace PrintManager.OperateClient.ViewModels.Dialog
{
    public class PrintTestViewModel : ViewModelBase
    {
        #region 属性

        public string MO { get => _mo; set => Set(ref _mo, value); }
        private string _mo;
        #endregion


        #region 变量
        #endregion

        #region 方法
        #endregion

        #region 事件

        #endregion

        #region 命令
        public void PrintTestCommand(int no)
        {
            switch (no)
            {
                case 1:
                    //F89
                    var data1 = new RequestInfo("TEST", JsonConvert.SerializeObject("F89"));
                    TCPClient.Instance.Send(data1);
                    break;
                case 2:
                    //RPX
                    var data2 = new RequestInfo("TEST", JsonConvert.SerializeObject("RPX"));
                    TCPClient.Instance.Send(data2);
                    break ; 
                case 3:
                    //Bundle F89RPX
                    var data3 = new RequestInfo("TEST", JsonConvert.SerializeObject("F89RPX"));
                    TCPClient.Instance.Send(data3);
                    break;
            }
        }

        public void CancelCommand()
        {
            MO = "";
            TryClose(false);
        }


        #endregion
    }
}
