using Panuon.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PrintManager.MainClient.Models
{
    public class PrintItemModel : NotifyPropertyChangedBase
    {
        public string DisplayName { get => _displayName; set => Set(ref _displayName, value); }
        private string _displayName;

        public Brush AccentBrush { get => _accentBrush; set => Set(ref _accentBrush, value); }
        private Brush _accentBrush;

    }
}
