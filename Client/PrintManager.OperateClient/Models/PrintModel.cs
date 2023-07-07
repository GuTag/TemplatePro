using Newtonsoft.Json;
using Panuon.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using static PrintManager.UI.Controls.PrintCanvas;

namespace PrintManager.OperateClient.Models
{
    public class PrintModel : NotifyPropertyChangedBase
    {
        public int Id { get; set; }
        public bool IsOK { get => _isOK; set => Set(ref _isOK, value); }
        private bool _isOK;
        public int RequestNum { get => _requestNum; set => Set(ref _requestNum, value); }
        private int _requestNum;

        public int ComplatedNum { get => _complatedNum; set => Set(ref _complatedNum, value); }
        private int _complatedNum;

        public string MO { get => _mo; set => Set(ref _mo, value); }
        private string _mo;

        public string ItemNo { get => _itemNo; set => Set(ref _itemNo, value); }
        private string _itemNo;

        public string Desc { get => _desc; set => Set(ref _desc, value); }
        private string _desc;

    }
}
