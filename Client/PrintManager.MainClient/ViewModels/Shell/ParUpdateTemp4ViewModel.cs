using PrintManager.MainClient.Components;
using PrintManager.Shared.Utils;
using PrintManager.Shared;
using PrintManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintManager.MainClient.Models;
using PrintManager.UI.Controls;
using PrintManager.Sql.BLL;
using PrintManager.Sql.Models;
using System.Windows;
using Spire.Pdf.Exporting.XPS.Schema.Mc;

namespace PrintManager.MainClient.ViewModels.Shell
{
    public class ParUpdateTemp4ViewModel : ViewModelBase
    {
        public ParUpdateTemp4ViewModel()
        {           
        }

        #region 属性
        public string ActualValue { get => _actualValue; set => Set(ref _actualValue, value); }
        private string _actualValue;
        public string NodeDes { get => _nodeDes; set => Set(ref _nodeDes, value); }
        private string _nodeDes;

        public string NodeAdr { get => _nodeAdr; set => Set(ref _nodeAdr, value); }
        private string _nodeAdr;

        #endregion

        #region 变量
        #endregion

        #region 方法
        private void WriteConfig()
        {
            ParModify parModify = new ParModify()
            {
                ActualValue = ActualValue,
                NodeDes = NodeDes,
                NodeAdr = NodeAdr,
            };
            ParModifyBLL.UpdateOfNodeAdr(parModify);
        }


        public void ShowParView(ParModify parModify)
        {
            if (parModify != null)
            {
                ActualValue = parModify.ActualValue;
                NodeDes = parModify.NodeDes;
                NodeAdr = parModify.NodeAdr;
            } 
        }
        #endregion

        #region 事件
        #endregion

        #region 命令

        public void onUpdateCommand()
        {
            ParModifyViewModel parModifyViewModel = new ParModifyViewModel();
            parModifyViewModel.UpdateProductOrderList();
            WriteConfig();
            TryClose() ;
        }

        public void onCancelCommand()
        {

            TryClose();

        }
        #endregion

        #region 重写方法
        public override void CanClose(Action<bool> callback)
        {

            base.CanClose(callback);
            #endregion
        }

    }
}