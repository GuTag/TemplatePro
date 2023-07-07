using PrintManager.Shared.Enums;
using PrintManager.Shared.Helpers;
using PrintManager.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PrintManager.Shared.Entity
{
    public class OrderEntity
    {
        #region 内部类
        public class OrderDetail
        {
            public OrderDetail()
            {

            }
            public OrderDetail(string printTyple, string itemNo, string mo, int complateNum, string soItm, string mltNo, OrderType orderType, string desc, string newItemNo)
            {
                ItemNo = itemNo;
                OrderType = orderType;

                A = OrderHelpers.GetOrderA(itemNo);
                B = OrderHelpers.GetOrderB(itemNo);
                C = OrderHelpers.GetOrderC(itemNo);
                D = OrderHelpers.GetOrderD(itemNo);
                E = OrderHelpers.GetOrderE(itemNo);
                F = OrderHelpers.GetOrderF(itemNo);
                G = OrderHelpers.GetOrderG(itemNo, newItemNo);
                H = OrderHelpers.GetOrderH(itemNo);
                I = OrderHelpers.GetOrderI(mo, complateNum);
                J = OrderHelpers.GetOrderJ(printTyple, OrderType);
                K = OrderHelpers.GetOrderK(mo);
                L = OrderHelpers.GetOrderL(soItm);
                M = OrderHelpers.GetOrderM(mltNo);
                N = OrderHelpers.GetOrderN(desc);


            }
            public string ItemNo { get; set; }
            public OrderType OrderType { get; set; }
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
            public string D { get; set; }
            public string E { get; set; }
            public string F { get; set; }
            public string G { get; set; }
            public string H { get; set; }
            public string I { get; set; }
            public string J { get; set; }
            public string K { get; set; }
            public string L { get; set; }
            public string M { get; set; }
            public string N { get; set; }
        }

        //public class PrintTemplate
        //{
        //    public string FilePath { get; set; }
        //    public string Name { get; set; }
        //    public double Width { get; set; }
        //    public double Height { get; set; }
        //    public int Num { get; set; }
        //    public string Type { get; set; }
        //    public string Background { get; set; }
        //    public int DPI { get; set; }

        //    public List<ControlItem> ControlItems { get; set; } = new List<ControlItem>();
        //}

        

        public class CanvasData
        {
            public PrintTemplate PrintTemplate { get; set; }
            public OrderDetail OrderDetail { get; set; }
        }
        #endregion
    }
}
