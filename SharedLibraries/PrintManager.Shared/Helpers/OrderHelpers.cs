using Newtonsoft.Json;
using NPOI.OpenXmlFormats.Vml;
using PrintManager.Shared.Entity;
using PrintManager.Shared.Enums;
using PrintManager.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using static PrintManager.Shared.Entity.OrderEntity;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace PrintManager.Shared.Helpers
{
    public static class OrderHelpers
    {
        public static List<string> GetPrintTemplateFilePath(OrderType orderType, string itemNo)
        {
            List<string> fileList = new List<string>();
            switch (orderType)
            {
                case OrderType.F89:
                    fileList.Add(Path.Combine(Environments.PrintFolderPath, "F89.pm"));
                    fileList.Add(Path.Combine(Environments.PrintFolderPath, "F89RPX.pm"));
                    break;
                case OrderType.RPX:
                    fileList.Add(Path.Combine(Environments.PrintFolderPath, "RPX.pm"));
                    fileList.Add(Path.Combine(Environments.PrintFolderPath, "F89RPX.pm"));
                    break;
                case OrderType.Bundle:
                    var itemType = OrderHelpers.GetOrderType(itemNo);
                    fileList.Add(Path.Combine(Environments.PrintFolderPath, "Bundle.pm"));
                    fileList.Add(Path.Combine(Environments.PrintFolderPath, itemType.ToString() + ".pm"));
                    break;
                default: break;
            }
            return fileList;
        }

        public static CanvasData GetPrintData(string filepath, string itemNo, string mo, int complateNum, string soItm, string mltNo, OrderType orderType, string desc, string newItemNo)
        {
            CanvasData printData = new CanvasData();
            printData.PrintTemplate = GetPrintTemplate(filepath);
            printData.OrderDetail = new OrderDetail(printData.PrintTemplate.Type, itemNo, mo, complateNum, soItm, mltNo, orderType,desc,newItemNo);
            return printData;
        }
        #region Ini格式 遗弃
        public static PrintTemplate GetPrintTemplateIni(string filepath)
        {
            PrintTemplate template = null;
            if (File.Exists(filepath))
            {
                template = new PrintTemplate() { FilePath = filepath };
                template.Width = double.Parse(IniUtil.IniReadvalue(filepath, "Base", "Width"));
                template.Height = double.Parse(IniUtil.IniReadvalue(filepath, "Base", "Height"));

                template.Num = int.Parse(IniUtil.IniReadvalue(filepath, "Base", "Num"));
                template.Type = IniUtil.IniReadvalue(filepath, "Base", "Type");

                template.Background = IniUtil.IniReadvalue(filepath, "Base", "Background");



                template.ControlItems.Clear();
                for (int i = 1; i <= template.Num; i++)
                {
                    var controlItem = new ControlItem();
                    //获取枚举值
                    ControlType controlType;
                    Enum.TryParse(IniUtil.IniReadvalue(filepath, i.ToString(), "ControlType"), out controlType);
                    controlItem.ControlType = controlType;

                    controlItem.Width = double.Parse(IniUtil.IniReadvalue(filepath, i.ToString(), "Width"));
                    controlItem.Height = double.Parse(IniUtil.IniReadvalue(filepath, i.ToString(), "Height"));
                    controlItem.PosX = double.Parse(IniUtil.IniReadvalue(filepath, i.ToString(), "PosX"));
                    controlItem.PosY = double.Parse(IniUtil.IniReadvalue(filepath, i.ToString(), "PosY"));

                    controlItem.FontSize = double.Parse(IniUtil.IniReadvalue(filepath, i.ToString(), "FontSize"));
                    controlItem.FontFamily = IniUtil.IniReadvalue(filepath, i.ToString(), "FontFamily");
                    controlItem.FontWeight = IniUtil.IniReadvalue(filepath, i.ToString(), "FontWeight");
                    controlItem.FontStyle = IniUtil.IniReadvalue(filepath, i.ToString(), "FontStyle");

                    controlItem.DisplayName = IniUtil.IniReadvalue(filepath, i.ToString(), "DisplayName");
                    controlItem.VarName = IniUtil.IniReadvalue(filepath, i.ToString(), "VarName");

                    controlItem.IsAssociation = Convert.ToBoolean(int.Parse(IniUtil.IniReadvalue(filepath, i.ToString(), "IsAssociation")));

                    controlItem.Image = IniUtil.IniReadvalue(filepath, i.ToString(), "Image");
                    if (controlItem.ControlType == ControlType.Image)
                    {
                        string imagefile = Path.Combine(Environments.PrintFolderPath, controlItem.Image);
                        if (File.Exists(imagefile))
                        {
                            controlItem.ImageData = ImageHelpers.ImageConvertString(imagefile);
                        }
                    }

                    template.ControlItems.Add(controlItem);
                }
            }
            return template;
        }

        public static void SavePrintTemplateIni(PrintTemplate template)
        {
            foreach (var item in template.ControlItems)
            {
                if (item.ControlType == ControlType.Text)
                {
                    if (string.IsNullOrEmpty(item.FontFamily) || item.FontSize <= 0) throw new Exception($"{item.DisplayName}文本控件的字体类型缺失");
                }
                if (item.IsAssociation)
                {
                    if (string.IsNullOrEmpty(item.VarName)) throw new Exception($"{item.DisplayName}文本控件的关联变量缺失");
                }
            }

            if (!File.Exists(template.FilePath))
            {
                File.Create(template.FilePath).Close();
            }
            using (StreamWriter sw = new StreamWriter(template.FilePath))
            {
                sw.WriteLine("");
            }

            IniUtil.IniWritevalue(template.FilePath, "Base", "Width", template.Width);
            IniUtil.IniWritevalue(template.FilePath, "Base", "Height", template.Height);
            IniUtil.IniWritevalue(template.FilePath, "Base", "Num", template.ControlItems.Count);
            IniUtil.IniWritevalue(template.FilePath, "Base", "Type", template.Type);
            IniUtil.IniWritevalue(template.FilePath, "Base", "Background", template.Background);

            for (int i = 1; i <= template.ControlItems.Count; i++)
            {

                var control = template.ControlItems[i - 1];
                var section = i.ToString();
                IniUtil.IniWritevalue(template.FilePath, section, "ControlType", control.ControlType);
                IniUtil.IniWritevalue(template.FilePath, section, "Width", control.Width);
                IniUtil.IniWritevalue(template.FilePath, section, "Height", control.Height);
                IniUtil.IniWritevalue(template.FilePath, section, "PosX", control.PosX);
                IniUtil.IniWritevalue(template.FilePath, section, "PosY", control.PosY);
                IniUtil.IniWritevalue(template.FilePath, section, "FontSize", control.FontSize);
                IniUtil.IniWritevalue(template.FilePath, section, "FontFamily", control.FontFamily);
                IniUtil.IniWritevalue(template.FilePath, section, "FontWeight", control.FontWeight);
                IniUtil.IniWritevalue(template.FilePath, section, "FontStyle", control.FontStyle);
                IniUtil.IniWritevalue(template.FilePath, section, "DisplayName", control.DisplayName);
                IniUtil.IniWritevalue(template.FilePath, section, "VarName", control.VarName);
                IniUtil.IniWritevalue(template.FilePath, section, "IsAssociation", control.IsAssociation ? 1 : 0);
                IniUtil.IniWritevalue(template.FilePath, section, "Image", control.Image);
            }
        }
        #endregion

        public static PrintTemplate GetPrintTemplate(string filepath)
        {
            if (!filepath.ToLower().EndsWith(".pm") || !File.Exists(filepath)) return null;
            string data = "";
            using (StreamReader sr = new StreamReader(filepath))
            {
                data = sr.ReadToEnd();
            }
            data = SecurityUtil.Decrypt(data);

            PrintTemplate template = JsonConvert.DeserializeObject<PrintTemplate>(data);
            template.FilePath= filepath;
            return template;
        }
        public static void SavePrintTemplate(PrintTemplate template)
        {
            foreach (var item in template.ControlItems)
            {
                if (item.ControlType == ControlType.Text)
                {
                    if (string.IsNullOrEmpty(item.FontFamily) || item.FontSize <= 0) throw new Exception($"{item.DisplayName}文本控件的字体类型缺失");
                }
                if (item.IsAssociation)
                {
                    if (string.IsNullOrEmpty(item.VarName)) throw new Exception($"{item.DisplayName}文本控件的关联变量缺失");
                }
            }
            if (!File.Exists(template.FilePath))
            {
                File.Create(template.FilePath).Close();
            }
            template.Num = template.ControlItems.Count;
            using (StreamWriter sw = new StreamWriter(template.FilePath))
            {
                var data = JsonConvert.SerializeObject(template);
                data = SecurityUtil.Encrypt(data);
                sw.WriteLine(data);
            }
        }
        public static string GetOrderItemNo(string desc)
        {
            if(!string.IsNullOrEmpty(desc) && desc.Length >= 33)
            {
                return desc.Substring(9,24);
            }
            return "";
            
        }


        public static OrderType GetOrderType(string itemNo)
        {
            if (string.IsNullOrEmpty(itemNo))
            {
                return OrderType.NONE;
            }
            if(itemNo.StartsWith("RPX"))
            {
                return OrderType.RPX;
            }
            if (itemNo.StartsWith("89"))
            {
                return OrderType.F89;
            }
            return OrderType.NONE;
        }

        public static string GetOrderA(string itemNo)
        {
            string s1, s2, s3;
            var orderType = GetOrderType(itemNo);
            switch(orderType)
            {
                case OrderType.F89:
                    s1 = itemNo.Substring(0, 3);
                    s2 = itemNo.Substring(3, 3);
                    return "F" + s1 + "-" + s2;
                case OrderType.RPX:
                    s1 = itemNo.Substring(0, 3);
                    s2 = itemNo.Substring(3, 3);
                    s3 = itemNo.Substring(15, 1);
                    return s1 + "-" + s2 + s3;
                default:
                    return "";
            }
        }

        public static string GetOrderB(string itemNo)
        {
            var s = itemNo.Substring(8, 2);
            if ("XX".Equals(s))
            {
                return "DA";
            }
            else
            {
                return "SR";
            }
        }

        public static string GetOrderC(string itemNo)
        {
            string s;
            var orderType = GetOrderType(itemNo);
            switch (orderType)
            {
                case OrderType.F89:
                    s = itemNo.Substring(13, 1);
                    if ("H".Equals(s))
                    {
                        return "-15/+150°C(+5/+302°F)";
                    }
                    else if ("N".Equals(s))
                    {
                        return "-20/ +80°C(-4/ +176°F)";
                    }
                    else if ("L".Equals(s))
                    {
                        return "-40/ +65°C(-40/ +149°F)";
                    }
                    else if ("E".Equals(s))
                    {
                        return "-52/ +65°C(-62/ +149°F)";
                    }
                    return "";
                case OrderType.RPX:
                    s = itemNo.Substring(13, 1);
                    if ("H".Equals(s))
                    {
                        return "-15°C(+5°F)to +150°C(+302°F)";
                    }
                    else if ("N".Equals(s))
                    {
                        return "-20°C(-4°F)to +80°C(+176°F)";
                    }
                    else if ("L".Equals(s))
                    {
                        return "-40°C(-40°F)to +65°C(+149°F)";
                    }
                    else if ("E".Equals(s))
                    {
                        return "-52°C(-62°F)to +65°C(+149°F)";
                    }
                    return "";
                default:
                    return "";
            }

            
        }

        public static string GetOrderD(string itemNo)
        {
            string s;
            s = itemNo.Substring(10, 1);
            if ("P".Equals(s))
            {
                return "EN ISO 228-1";
            }
            else if ("N".Equals(s))
            {
                return "ASME B1.20.1";
            }
            return "";
        }

        public static string GetOrderE(string itemNo)
        {
            //string s;
            var orderType = GetOrderType(itemNo);
            switch (orderType)
            {
                case OrderType.F89:
                    return "Max 8.3bar (120psi)";
                case OrderType.RPX:
                    return "8 Bar (120PSI)";
                default:
                    return "";
            }
        }

        public static string GetOrderF(string itemNo)
        {
            string s;
            var orderType = GetOrderType(itemNo);
            switch (orderType)
            {
                case OrderType.F89:
                    s= itemNo.Substring(3, 3);
                    return "89X" + s + "-NO";
                case OrderType.RPX:
                    return "";
                default:
                    return "";
            }
        }

        public static string GetOrderG(string itemNo, string newItemNo)
        {
            //string s;
            if (!string.IsNullOrEmpty(newItemNo))
            {
                if(newItemNo.Length > 40)
                {
                    var lastIndex = itemNo.LastIndexOf("00");
                    var s = itemNo.Remove(lastIndex, 2);
                    return s.Insert(3, "-");
                }
               
                return newItemNo;
            }
            else
            {
                return itemNo;
            }
        }

        public static string GetOrderH(string itemNo)
        {
            string s;
            var orderType = GetOrderType(itemNo);
            switch (orderType)
            {
                case OrderType.F89:
                    s = itemNo.Substring(15, 1);
                    if ("M".Equals(s))
                    {
                        return "Metric";
                    }
                    else if("U".Equals(s))
                    {
                        return "Imperial";
                    }
                    return "";
                case OrderType.RPX:
                    s = itemNo.Substring(15, 1);
                    if ("M".Equals(s))
                    {
                        return "Metric";
                    }
                    else if ("U".Equals(s))
                    {
                        return "Imperial";
                    }
                    return "";
                default:
                    return "";
            }
        }

        public static string GetOrderI(string mo, int complateNum)
        {
            return mo + "-" + (complateNum + 1).ToString();
        }

        public static string GetOrderJ(string printType,OrderType orderType)
        {
            switch (orderType)
            {
                case OrderType.F89:
                    return DateTime.Now.ToString("MM-yyyy");
                case OrderType.RPX:
                    if (printType.Equals(OrderType.RPX.ToString()))
                    {
                        return DateTime.Now.ToString("yyyy");
                    }
                    return DateTime.Now.ToString("MM-yyyy");
                default:
                    return DateTime.Now.ToString("MM-yyyy");
            }
        }
        public static string GetOrderK(string mo)
        {
            return mo;
        }

        public static string GetOrderL(string soItm)
        {
            return soItm;
        }

        public static string GetOrderM(string mtlNo)
        {
            return mtlNo;
        }

        public static string GetOrderN(string desc)
        {
            return desc;
        }
    }
}
