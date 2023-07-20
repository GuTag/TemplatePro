using Panuon.WPF;
using PrintManager.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.MainClient.Models
{
    public  class EventLogModel : NotifyPropertyChangedBase
    {
        public LogType LogType { get => _logType; set => Set(ref _logType, value); }
        private LogType _logType;

        public string ClientName { get => _clientName; set => Set(ref _clientName, value); }
        private string _clientName;
        public string NodeAdr { get => _nodeAdr; set => Set(ref _nodeAdr, value); }
        private string _nodeAdr;
        public string ActualValue { get => _actualValue; set => Set(ref _actualValue, value); }
        private string _actualValue;

        public string NodeTypeView { get => _nodeTypeView; set => Set(ref _nodeTypeView, value); }
        private string _nodeTypeView;
        public string Message { get => _message; set => Set(ref _message, value); }
        private string _message;

        public DateTime AddTime { get => _addTime; set => Set(ref _addTime, value); }
        private DateTime _addTime = DateTime.Now;
    }
}
