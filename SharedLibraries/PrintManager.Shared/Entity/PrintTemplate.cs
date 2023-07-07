using Panuon.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PrintManager.Shared.Entity.OrderEntity;

namespace PrintManager.Shared.Entity
{
    [Serializable]
    public class PrintTemplate : NotifyPropertyChangedBase
    {
        [field: NonSerialized]
        public Action UpdateViewEvent;
        public int Num { get; set; }

        public string FilePath { get => _filePath; set => Set(ref _filePath, value); }
        private string _filePath;

        public string Name { get => _name; set => Set(ref _name, value); }
        private string _name;

        public string Type { get => _type; set => Set(ref _type, value); }
        private string _type = "F89";

        public string Background { get => _background; set { Set(ref _background, value); UpdateView(); } }
        private string _background = "#FFFFFF";

        public double Width { get => _width; set { Set(ref _width, value); UpdateView(); } }
        private double _width = 90;

        public double Height { get => _height; set { Set(ref _height, value); UpdateView(); } }
        private double _height = 50;

        public int DPI { get => _dpi; set => Set(ref _dpi, value); }
        private int _dpi;

        public List<ControlItem> ControlItems { get => _controlItems; set { Set(ref _controlItems, value); } }
        private List<ControlItem> _controlItems = new List<ControlItem>();


        public void UpdateView()
        {
            UpdateViewEvent?.Invoke();
        }
    }
}
