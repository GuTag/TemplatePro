using Panuon.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.MainClient.Models
{
    public class LanguageModel : NotifyPropertyChangedBase
    {

        public string Index { get => _index; set => Set(ref _index, value); }
        private string _index;
        public string Language_zh { get => _language_zh; set => Set(ref _language_zh, value); }
        private string _language_zh;
        public string Language_cn { get => _language_cn; set => Set(ref _language_cn, value); }
        private string _language_cn;
        public DateTime AddTime { get => _addtime; set => Set(ref _addtime, value); }
        private DateTime _addtime;
    }
}
