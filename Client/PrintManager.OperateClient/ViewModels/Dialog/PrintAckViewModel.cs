﻿using PrintManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrintManager.OperateClient.ViewModels.Dialog
{
    public class PrintAckViewModel : ViewModelBase
    {
        public PrintAckViewModel(string text = "")
        {
            HintText = text;
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                TryClose();
            });
        }

        #region 属性
        public string HintText { get => _hintText; set => Set(ref _hintText, value); }
        private string _hintText;


        #endregion

        #region 事件与命令

        public void PrintTestCommand()
        {
            
        }

        public void onCanceButtonClick()
        {
            TryClose(false);
        }
        #endregion
    }
}