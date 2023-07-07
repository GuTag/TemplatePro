using Newtonsoft.Json;
using Panuon.WPF;
using PrintManager.Shared.Enums;
using PrintManager.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PrintManager.UI.Controls.PrintCanvas;

namespace PrintManager.MainClient.Models
{
    public class ProductOrderModel : NotifyPropertyChangedBase
    {

        /// <summary>
        /// Client 
        /// </summary>

        public string ClientName { get => _clientName; set => Set(ref _clientName, value); }
        private string _clientName;
        public string NodeType { get => _nodeType; set => Set(ref _nodeType, value); }
        private string _nodeType;
        public string NodeAdr { get => _nodeAdr; set => Set(ref _nodeAdr, value); }
        private string _nodeAdr;
        public string NodeDes { get => _nodeDes; set => Set(ref _nodeDes, value); }
        private string _nodeDes;

        public string NodeIndexLang { get => _nodeIndexLang; set => Set(ref _nodeIndexLang, value); }
        private string _nodeIndexLang;
        public string NodeTypeView { get => _nodeTypeView; set => Set(ref _nodeTypeView, value); }
        private string _nodeTypeView;































        public int Id { get; set; }
        public OrderType ProductOrderType { get => _productOrderType; set => Set(ref _productOrderType, value); }
        private OrderType _productOrderType = OrderType.NONE;

        public bool IsOK { get => _isOK; set => Set(ref _isOK, value); }
        private bool _isOK;

        public bool IsPrinting { get => _isPrinting; set => Set(ref _isPrinting, value); }
        private bool _isPrinting;
        public int RequestNum { get => _requestNum; set => Set(ref _requestNum, value); }
        private int _requestNum;

        public int ComplatedNum { get => _complatedNum; set => Set(ref _complatedNum, value); }
        private int _complatedNum;

        public string MO { get => _mo; set => Set(ref _mo, value); }
        private string _mo;

        public string Desc { get => _desc; set => Set(ref _desc, value); }
        private string _desc;

        public string ItemNo { get => _itemNo; set => Set(ref _itemNo, value); }
        private string _itemNo;

        public string NewItemNo { get => _newItemNo; set => Set(ref _newItemNo, value); }
        private string _newItemNo;

        public string SOItem { get => _sOItem; set => Set(ref _sOItem, value); }
        private string _sOItem;

        //Mtl No
        public string MtlNo { get => _mtlNo; set => Set(ref _mtlNo, value); }
        private string _mtlNo;

        public string CustomerCode { get => _customerCode; set => Set(ref _customerCode, value); }
        private string _customerCode;

        public string CPQCode { get => _CPQCode; set => Set(ref _CPQCode, value); }
        private string _CPQCode;

        public DateTime AddTime { get => _addtime; set => Set(ref _addtime, value); }
        private DateTime _addtime;

        public DateTime UpdateTime { get => _updateTime; set => Set(ref _updateTime, value); }
        private DateTime _updateTime;


        //public PrintTemplate PrintTemplate { get; set; }
        #region 方法
        
        

        #endregion

        
    }
}
