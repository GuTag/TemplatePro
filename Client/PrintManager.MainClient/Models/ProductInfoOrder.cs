using Panuon.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.MainClient.Models
{
    public  class ProductInfoOrder : NotifyPropertyChangedBase
    {
        public string Name { get => _requestNum; set => Set(ref _requestNum, value); }
        private string _requestNum;

        public string IP { get => _ip; set => Set(ref _ip, value); }
        private string _ip;

        public int SubNodeNbr { get => _subNodeNbr; set => Set(ref _subNodeNbr, value); }
        private int _subNodeNbr;
        public string WebAdr { get => _webAdr; set => Set(ref _webAdr, value); }
        private string _webAdr;

        public bool IsOK { get => _isOK; set => Set(ref _isOK, value); }
        private bool _isOK;

        public DateTime AddTime { get => _addTime; set => Set(ref _addTime, value); }
        private DateTime _addTime = DateTime.Now;
    }
}
