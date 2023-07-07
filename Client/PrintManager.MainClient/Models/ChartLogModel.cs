using Panuon.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.MainClient.Models
{
    public  class ChartLogModel : NotifyPropertyChangedBase
    {
        public int RequestNum { get => _requestNum; set => Set(ref _requestNum, value); }
        private int _requestNum;
        public int ComplatedNum { get => _complatedNum; set => Set(ref _complatedNum, value); }
        private int _complatedNum;
        public string MO { get => _mo; set => Set(ref _mo, value); }
        private string _mo;
        public string Line { get => _line; set => Set(ref _line, value); }
        private string _line;
        public string Message { get => _message; set => Set(ref _message, value); }
        private string _message;

        public int ProducNum { get => _producNum; set => Set(ref _producNum, value); }
        private int _producNum;

        public bool IsOK { get => _isOK; set => Set(ref _isOK, value); }
        private bool _isOK;

        public DateTime AddTime { get => _addTime; set => Set(ref _addTime, value); }
        private DateTime _addTime = DateTime.Now;
    }
}
