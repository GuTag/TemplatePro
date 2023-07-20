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
    public class ParUpdateTemp2ViewModel : ViewModelBase
    {
        public ParUpdateTemp2ViewModel()
        {           
        }

        #region 属性
        public string ClientName { get => _clientName; set => Set(ref _clientName, value); }
        private string _clientName;
        public string NodeType { get => _nodeType; set => Set(ref _nodeType, value); }
        private string _nodeType;
        public string NodeTypeView { get => _nodeTypeView; set => Set(ref _nodeTypeView, value); }
        private string _nodeTypeView;
        public string NodeAdr { get => _nodeAdr; set => Set(ref _nodeAdr, value); }
        private string _nodeAdr;
        public int NodeValHH { get => _nodeValHH; set => Set(ref _nodeValHH, value); }
        private int _nodeValHH;
        public int NodeValH { get => _nodeValH; set => Set(ref _nodeValH, value); }
        private int _nodeValH;
        public int NodeValLL { get => _nodeValLL; set => Set(ref _nodeValLL, value); }
        private int _nodeValLL;
        public int NodeValL { get => _nodeValL; set => Set(ref _nodeValL, value); }
        private int _nodeValL;
        public string NodeLanguageHH { get => _nodeLanguageHH; set => Set(ref _nodeLanguageHH, value); }
        private string _nodeLanguageHH;
        public string NodeLanguageH { get => _nodeLanguageH; set => Set(ref _nodeLanguageH, value); }
        private string _nodeLanguageH;
        public string NodeLanguageLL { get => _nodeLanguageLL; set => Set(ref _nodeLanguageLL, value); }
        private string _nodeLanguageLL;
        public string NodeLanguageL { get => _nodeLanguageL; set => Set(ref _nodeLanguageL, value); }
        private string _nodeLanguageL;
        public string NodeUnit { get => _nodeUnit; set => Set(ref _nodeUnit, value); }
        private string _nodeUnit;
        public string NodeDes { get => _nodeDes; set => Set(ref _nodeDes, value); }
        private string _nodeDes;

        public bool IsUpdateItem { get => _isUpdateItem; set => Set(ref _isUpdateItem, value); }
        private bool _isUpdateItem;
        public bool IsAddeItem { get => _isAddItem; set => Set(ref _isAddItem, value); }
        private bool _isAddItem;

        #endregion

        #region 变量
        #endregion

        #region 方法
        private void WriteConfig()
        {
            Analog analog = new Analog()
            {
                ClientName = ClientName,
                NodeType = NodeType,
                NodeTypeView = NodeTypeView,
                NodeAdr = NodeAdr,
                NodeValHH = NodeValHH,
                NodeValH = NodeValH,
                NodeValLL = NodeValLL,
                NodeValL = NodeValL,
                NodeLanguageHH = NodeLanguageHH,
                NodeLanguageH = NodeLanguageH,
                NodeLanguageLL = NodeLanguageLL,
                NodeLanguageL = NodeLanguageL,
                NodeUnit = NodeUnit,
                NodeDes = NodeDes
        };
            AnalogBLL.UpdateOfNodeAdr(analog);
        }


        public void ShowParView(AnalogModel orderModel, bool UpdateItem)
        {
            if (orderModel != null)
            {
                ClientName = orderModel.ClientName;
                NodeType = orderModel.NodeType;
                NodeTypeView = orderModel.NodeTypeView;
                NodeAdr = orderModel.NodeAdr;
                NodeValHH = orderModel.NodeValHH;
                NodeValH = orderModel.NodeValH;
                NodeValLL = orderModel.NodeValLL;
                NodeValL = orderModel.NodeValL;
                NodeLanguageHH = orderModel.NodeLanguageHH;
                NodeLanguageH = orderModel.NodeLanguageH;
                NodeLanguageLL = orderModel.NodeLanguageLL;
                NodeLanguageL = orderModel.NodeLanguageL;
                NodeUnit = orderModel.NodeUnit;
                NodeDes = orderModel.NodeDes;
            }
            IsUpdateItem = UpdateItem;
            IsAddeItem = !UpdateItem;
        }
        #endregion

        #region 事件
        #endregion

        #region 命令

        public void onUpdateCommand()
        {
            WriteConfig();
            TryClose() ;
        }

        public void onAddCommand()
        {

            var isExit = AnalogBLL.Add(new Analog()
            {
                ClientName = ClientName,
                NodeType = NodeType,
                NodeTypeView = NodeTypeView,
                NodeAdr = NodeAdr,
                NodeValHH = NodeValHH,
                NodeValH = NodeValH,
                NodeValLL = NodeValLL,
                NodeValL = NodeValL,
                NodeLanguageHH = NodeLanguageHH,
                NodeLanguageH = NodeLanguageH,
                NodeLanguageLL = NodeLanguageLL,
                NodeLanguageL = NodeLanguageL,
                NodeUnit = NodeUnit,
                NodeDes = NodeDes
            });

            if (isExit)
            {
                WindowManagerExtension.ShowMessageDialog(WindowManager, "已存在相同地址:" + NodeAdr);
            }
            else
            {
                TryClose();
                //productViewModel.UpdateProductOrderList();
            }
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