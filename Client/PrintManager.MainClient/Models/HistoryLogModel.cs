using Panuon.WPF;
using PrintManager.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.MainClient.Models
{
    public  class HistoryLogModel : NotifyPropertyChangedBase
    {
        public LogType LogType { get => _logType; set => Set(ref _logType, value); }
        private LogType _logType;

        public string Source { get => _source; set => Set(ref _source, value); }
        private string _source;
        public string Message { get => _message; set => Set(ref _message, value); }
        private string _message;

        public DateTime AddTime { get => _addTime; set => Set(ref _addTime, value); }
        private DateTime _addTime = DateTime.Now;
    }
}
