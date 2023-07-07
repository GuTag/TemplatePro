using Panuon.WPF;
using PrintManager.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PrintManager.Shared.Entity
{
    public class ControlItem : NotifyPropertyChangedBase
    {
        public ControlItem()
        {
            ID = Guid.NewGuid().ToString();
        }
        public string ID { get; set; }
        public Point MousePoint { get; set; }
        public bool IsSelected { get; set; }
        public ControlType ControlType { get; set; }

        public double Width { get; set; }

        public double WidthFactor { get; set; } = 1;
        public double Spacing { get; set; } = 1;
        public double Height { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double FontSize { get; set; }
        public string FontFamily { get; set; } = "微软雅黑";
        public string FontWeight { get; set; } = "Regular";
        public string FontStyle { get; set; }
        public string DisplayName { get; set; }
        public string VarName { get; set; }

        public string Image { get; set; }
        public string ImageData { get; set; }
        //public bool IsAssociation { get; set; }

        

        //public double Width { get => _width; set => Set(ref _width, value); }
        //private double _width;

        //public double Height { get => _height; set => Set(ref _height, value); }
        //private double _height;

        //public double PosX { get => _posX; set => Set(ref _posX, value); }
        //private double _posX;

        //public double PosY { get => _posY; set => Set(ref _posY, value); }
        //private double _posY;

        //public double FontSize { get => _fontSize; set => Set(ref _fontSize, value); }
        //private double _fontSize;

        //public string FontFamily { get => _fontFamily; set => Set(ref _fontFamily, value); }
        //private string _fontFamily;

        //public string FontWeight { get => _fontWeight; set => Set(ref _fontWeight, value); }
        //private string _fontWeight;

        //public string FontStyle { get => _fontStyle; set => Set(ref _fontStyle, value); }
        //private string _fontStyle;

        //public string DisplayName { get => _displayName; set => Set(ref _displayName, value); }
        //private string _displayName;

        //public string VarName { get => _varName; set => Set(ref _varName, value); }
        //private string _varName;

        public bool IsAssociation { get => _isAssociation; set => Set(ref _isAssociation, value); }
        private bool _isAssociation;
        //AspectRatio
        public bool IsAspectRatio { get => _isAspectRatio; set => Set(ref _isAspectRatio, value); }
        private bool _isAspectRatio;


    }
}
