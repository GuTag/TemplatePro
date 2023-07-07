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
    public class ManualViewModel : ViewModelBase
    {
        #region 属性

        public string MO { get => _mo; set => Set(ref _mo, value); }
        private string _mo;

        public int Index { get => _index; set 
            {
                if(value<= 0) value = 1;
                Set(ref _index, value);
            } }
        private int _index = 1;
        #endregion


        #region 变量
        #endregion

        #region 方法
        #endregion

        #region 事件

        #endregion

        #region 命令

        public void KeyInputCommand(int num)
        {
            MO += num.ToString();
        }
        public void KeyDelateCommand()
        {
            if (!string.IsNullOrEmpty(MO))
            {
                MO = MO.Remove(MO.Length - 1, 1);
            }
        }
        public void EnterCommand(ActionExecutionContext context)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                SureCommand();
            }
        }

        public void CancelCommand()
        {
            MO = "";
            TryClose(false);
        }

        public void SureCommand()
        {
            if (string.IsNullOrEmpty(MO))
            {
                WindowManagerExtension.ShowMessageDialog(WindowManager, "MO码不能为空");
            }
            else
            {
                TryClose(true);
            }
        }
        #endregion
    }
}
