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
    public class ParUpdateTemp1ViewModel : ViewModelBase
    {
        public ParUpdateTemp1ViewModel()
        {           
        }

        #region 属性
        public string ClientName { get => _clientName; set => Set(ref _clientName, value); }
        private string _clientName;
        public string NodeType { get => _nodeType; set => Set(ref _nodeType, value); }
        private string _nodeType;
        public string NodeTypeView { get => _nodeTypeView; set => Set(ref _nodeTypeView, value); }
        private string _nodeTypeView;
        public string NodeIndexLang { get => _nodeIndexLang; set => Set(ref _nodeIndexLang, value); }
        private string _nodeIndexLang;
        public string NodeAdr { get => _nodeAdr; set => Set(ref _nodeAdr, value); }
        private string _nodeAdr;

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
            ProductOrder productOrder = new ProductOrder()
            {
                ClientName = ClientName,
                NodeType = NodeType,
                NodeIndexLang = NodeIndexLang,
                NodeAdr = NodeAdr,
                NodeTypeView = NodeTypeView,
            };
            ProductOrderBLL.UpdateOfNodeAdr(productOrder);
        }


        public void ShowParView(ProductOrderModel orderModel , bool UpdateItem)
        {
            if (orderModel != null)
            {
                ClientName = orderModel.ClientName;
                NodeType = orderModel.NodeType;
                NodeTypeView = orderModel.NodeTypeView;
                NodeIndexLang = orderModel.NodeIndexLang;
                NodeAdr = orderModel.NodeAdr;
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
            ProductViewModel productViewModel = new ProductViewModel();
            productViewModel.UpdateProductOrderList();
        }

        public void onAddCommand()
        {

            var isExit = ProductOrderBLL.Add(new ProductOrder() 
            {
                ClientName = ClientName,
                NodeType = NodeType,
                NodeIndexLang = NodeIndexLang,
                NodeAdr = NodeAdr,
                NodeTypeView = NodeTypeView,
            });

            if (isExit)
            {
                WindowManagerExtension.ShowMessageDialog(WindowManager,"已存在相同地址:" + NodeAdr );
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