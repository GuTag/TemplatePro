using Newtonsoft.Json.Linq;
using Panuon.WPF;
using PrintManager.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.OperateClient.Models
{
    public class HintModel : NotifyPropertyChangedBase
    {
        public string Message { get => _message; set { Set(ref _message, value); } }
        private string _message;

        public string Color { get => _color; set => Set(ref _color, value); }
        private string _color;

        public void Clean()
        {
            Message = "";
            Color = "#000000";
        }
        public void Update(LogType type, object message)
        {
            if (message != null)
            {
                Message = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:{message}";
            }
            switch (type)
            {
                case LogType.Info:
                    Color = "#000000";
                    break;
                case LogType.Warning:
                    Color = "#FF962D";
                    break;
                case LogType.Error:
                    Color = "#FF0000";
                    break;
                case LogType.OK:
                    Color = "#00FF00";
                    break;
                default:
                    Color = "#000000";
                    break;

            }
        }

        
        //public override string ToString()
        //{
        //    return $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:{Message}";
        //}
    }
}
