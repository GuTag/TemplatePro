using PrintManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.MainClient.ViewModels.Dialog
{
    public class AckViewModel : ViewModelBase
    {
        public AckViewModel(string title , string text)
        {
            Title = title;
            HintText = text;
        }

        #region 属性

        public string Title { get => _title; set => Set(ref _title, value); }
        private string _title;
        public string HintText { get => _hintText; set => Set(ref _hintText, value); }
        private string _hintText;


        #endregion

        #region 事件与命令

        public void onSureButton()
        {
            TryClose(true);
        }

        public void onCancelButton()
        {
            TryClose(false);
        }
        #endregion
    }
}
