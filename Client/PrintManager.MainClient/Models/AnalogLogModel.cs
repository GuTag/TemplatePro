using Panuon.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.MainClient.Models
{
    public class AnalogLogModel: NotifyPropertyChangedBase
    {
        public string ClientName { get => _clientName; set => Set(ref _clientName, value); }
        private string _clientName;
        public string NodeType { get => _nodeType; set => Set(ref _nodeType, value); }
        private string _nodeType;
        public string NodeTypeView { get => _nodeTypeView; set => Set(ref _nodeTypeView, value); }
        private string _nodeTypeView;
        public string NodeAdr { get => _nodeAdr; set => Set(ref _nodeAdr, value); }
        private string _nodeAdr;
        public int NodeValHH { get => _nodeValHH; set => Set(ref _nodeValHH, value); }
        private int _nodeValHH;
        public int NodeValH { get => _nodeValH; set => Set(ref _nodeValH, value); }
        private int _nodeValH;
        public int NodeValLL { get => _nodeValLL; set => Set(ref _nodeValLL, value); }
        private int _nodeValLL;
        public int NodeValL { get => _nodeValL; set => Set(ref _nodeValL, value); }
        private int _nodeValL;
        public float ActualValve { get => _actualValve; set => Set(ref _actualValve, value); }
        private float _actualValve;
        public string NodeLanguageHH { get => _nodeLanguageHH; set => Set(ref _nodeLanguageHH, value); }
        private string _nodeLanguageHH;
        public string NodeLanguageH { get => _nodeLanguageH; set => Set(ref _nodeLanguageH, value); }
        private string _nodeLanguageH;
        public string NodeLanguageLL { get => _nodeLanguageLL; set => Set(ref _nodeLanguageLL, value); }
        private string _nodeLanguageLL;
        public string NodeLanguageL { get => _nodeLanguageL; set => Set(ref _nodeLanguageL, value); }
        private string _nodeLanguageL;
        public string NodeUnit { get => _nodeUnit; set => Set(ref _nodeUnit, value); }
        private string _nodeUnit;
        public string NodeDes { get => _nodeDes; set => Set(ref _nodeDes, value); }
        private string _nodeDes;
        public DateTime AddTime { get => _addtime; set => Set(ref _addtime, value); }
        private DateTime _addtime;
    }
}
