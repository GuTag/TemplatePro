using Panuon.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.MainClient.Models.Extension
{
    public class TimeProgramTreeParent:NotifyPropertyChangedBase
    {
        public int  Index { get => _index; set => Set(ref _index, value); }
        private int _index;
        public string Content { get => _content; set => Set(ref _content, value); }
        private string _content;
    }
}
