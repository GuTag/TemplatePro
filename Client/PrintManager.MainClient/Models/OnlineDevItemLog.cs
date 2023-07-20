using Panuon.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.MainClient.Models
{
    public class OnlineDevItemLog : NotifyPropertyChangedBase
    {
        public string ClientName { get => _clientName; set => Set(ref _clientName, value); }
        private string _clientName;
        public string OPCAdr { get => _oPCAdr; set => Set(ref _oPCAdr, value); }
        private string _oPCAdr;
        public int TotalNodeNbr { get => _totalNodeNbr; set => Set(ref _totalNodeNbr, value); }
        private int _totalNodeNbr;
        public string IssueWebAdr { get => _issueWebAdr; set => Set(ref _issueWebAdr, value); }
        private string _issueWebAdr;
        public bool IsStatus { get => _isStatus; set => Set(ref _isStatus, value); }
        private bool _isStatus;

        public DateTime AddTime { get => _addtime; set => Set(ref _addtime, value); } 
        private DateTime _addtime = DateTime.Now;
    }
}
