using Caliburn.Micro;
using Newtonsoft.Json;
using PrintManager.MainClient.Models;
using PrintManager.Shared;
using PrintManager.Shared.Helpers;
using PrintManager.Shared.TCP;
using PrintManager.Shared.Utils;
using PrintManager.Sql.BLL;
using PrintManager.UI.Controls;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static PrintManager.Shared.Entity.OrderEntity;

namespace PrintManager.MainClient.Components
{
    public class Communication
    {
        public static (ProductOrderModel, List<CanvasData>) GetMO(string mo)
        {
            ProductOrderModel item = null;
            List<CanvasData> PrintDatas = null;
            var stringItem = ProductOrderBLL.GetItemOfMO(mo);
            if (!string.IsNullOrEmpty(stringItem))
            {
                item = JsonConvert.DeserializeObject<ProductOrderModel>(stringItem);
                if (item != null)
                {
                    var fileList = OrderHelpers.GetPrintTemplateFilePath(item.ProductOrderType, item.ItemNo);
                    PrintDatas = new List<CanvasData>();
                    foreach (var filepath in fileList)
                    {
                        PrintDatas.Add(OrderHelpers.GetPrintData(filepath, item.ItemNo, item.MO, item.ComplatedNum, item.SOItem, item.MtlNo, item.ProductOrderType, item.Desc, item.NewItemNo));
                    }
                }
            }
            return (item, PrintDatas);
        }

        public static (ProductOrderModel, List<CanvasData>) GetManulMO(string mo, int index)
        {
            ProductOrderModel item = null;
            List<CanvasData> PrintDatas = null;
            var stringItem = ProductOrderBLL.GetItemOfMO(mo);
            if (!string.IsNullOrEmpty(stringItem))
            {
                item = JsonConvert.DeserializeObject<ProductOrderModel>(stringItem);
                if (item != null)
                {
                    var fileList = OrderHelpers.GetPrintTemplateFilePath(item.ProductOrderType, item.ItemNo);
                    PrintDatas = new List<CanvasData>();
                    foreach (var filepath in fileList)
                    {
                        PrintDatas.Add(OrderHelpers.GetPrintData(filepath, item.ItemNo, item.MO, index, item.SOItem, item.MtlNo, item.ProductOrderType, item.Desc, item.NewItemNo));
                    }
                }
            }
            return (item, PrintDatas);
        }

        public static void GetDesc(string ip, string desc)
        {
            //Task.Run(() =>
            //{
            //    var order = JsonConvert.DeserializeObject<ProductOrderModel>(data);
            //    string response = "";
            //    if (order != null)
            //    {
            //        var stringItem = ProductOrderBLL.GetItemForDesc(order.Desc);
            //        if (string.IsNullOrEmpty(stringItem))
            //        {
            //            var item = JsonConvert.DeserializeObject<ProductOrderModel>(stringItem);
            //            if (item != null)
            //            {
            //                response = JsonConvert.SerializeObject(item);
            //            }
            //        }

            //    }
            //    TCPServer.Instance.SendClient(new RequestInfo("Desc", response), TCPServer.Instance.GetClientID(ip));
            //});
        }


        public static void UpdateComplateNum(string ip, string data)
        {
            //Task.Run(() =>
            //{
                ProductOrderBLL.UpdateComplatedNum(data);
            //});
        }
    }
}
