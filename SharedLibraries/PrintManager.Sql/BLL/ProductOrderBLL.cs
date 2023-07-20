using Newtonsoft.Json;
using Opc.Ua;
using PrintManager.Sql.IBLL;
using PrintManager.Sql.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Sql.BLL
{
    public class ProductOrderBLL
    {
        public static bool Add(string stringItem)
        {
            var item = JsonConvert.DeserializeObject<ProductOrder>(stringItem);
            item.AddTime= DateTime.Now;
            item.UpdateTime= DateTime.Now;
            var result = SqlSugarHelper.Instance.db.Insertable(item).ExecuteCommand();
            return result > 0;
        }

        public static bool Add(ProductOrder item)
        {
            var isExit = false; 
            var getItem = SqlSugarHelper.Instance.Find<ProductOrder>((o) => o.NodeAdr == item.NodeAdr).FirstOrDefault();
            if (getItem == null)
            {
                item.AddTime = DateTime.Now;
                item.UpdateTime = DateTime.Now;
                var result = SqlSugarHelper.Instance.db.Insertable(item).ExecuteCommand();
            }
            else
            {
                isExit = true;
            }

            return isExit;
        }

        //public static bool AddList(string stringItems)
        //{
        //    var items = JsonConvert.DeserializeObject<List<ProductOrder>>(stringItems);
        //    var result = SqlSugarHelper.Instance.db.Insertable(items).ExecuteCommand();
        //    return result > 0;
        //}
        public static bool AddList(string stringItems)
        {
            var items = JsonConvert.DeserializeObject<List<ProductOrder>>(stringItems);
            var itemList = new List<ProductOrder>();
            foreach (var item in items)
            {
                var getItem = SqlSugarHelper.Instance.Find<ProductOrder>((o) => o.NodeAdr == item.NodeAdr).FirstOrDefault();
                if (getItem == null)
                {
                    itemList.Add(item);
                }
            }
            var result = SqlSugarHelper.Instance.db.Insertable(itemList).ExecuteCommand();
            return result > 0;
        }
        public static bool AddList(List<ProductOrder> items)
        {
            var itemList = new List<ProductOrder>();
            foreach (var item in items)
            {
                var getItem = SqlSugarHelper.Instance.Find<ProductOrder>((o) => o.NodeAdr == item.NodeAdr).FirstOrDefault();
                if (getItem == null)
                {
                    itemList.Add(item);
                }
            }
            var result = SqlSugarHelper.Instance.db.Insertable(itemList).ExecuteCommand();
            return result > 0;
        }

        public static void Delete(string NodeAdr)
        {
            var result = SqlSugarHelper.Instance.db.Deleteable<ProductOrder>((o) => o.NodeAdr == NodeAdr).ExecuteCommand();
        }

        public static bool UpdateComplatedNum(string stringItem)
        {
            var item = JsonConvert.DeserializeObject<ProductOrder>(stringItem);
            
            var NewItem = SqlSugarHelper.Instance.Find<ProductOrder>((o) => o.Id == item.Id).FirstOrDefault();
            if (NewItem == null) return false;
            if(NewItem.ComplatedNum == item.ComplatedNum - 1)
            {
                NewItem.ComplatedNum = item.ComplatedNum;
            }
            if(NewItem.RequestNum > NewItem.ComplatedNum)
            {
                NewItem.IsOK = false;
            }
            NewItem.IsPrinting= false;
            NewItem.UpdateTime = DateTime.Now;
            var result = SqlSugarHelper.Instance.db.Updateable(NewItem).ExecuteCommand();
            return result > 0;
        }

        public static bool UpdateProductOK(string mo)
        {
            var item = SqlSugarHelper.Instance.Find<ProductOrder>((o) => o.MO == mo).FirstOrDefault();
            item.IsOK = true;
            item.IsPrinting= true;
            var result = SqlSugarHelper.Instance.db.Updateable(item).ExecuteCommand();
            return result > 0;
        }

        public static bool UpdateProductPrinting(string mo, bool isPrinting)
        {
            var item = SqlSugarHelper.Instance.Find<ProductOrder>((o) => o.MO == mo).FirstOrDefault();
            item.IsPrinting = isPrinting;
            var result = SqlSugarHelper.Instance.db.Updateable(item).ExecuteCommand();
            return result > 0;
        }

        //public static string GetAll()
        //{
        //    var data = SqlSugarHelper.Instance.Find<ProductOrder>((o) => true);
        //    return JsonConvert.SerializeObject(data);
        //}
        public static void UpdateOfNodeAdr(ProductOrder productOrder)
        {
            var item = SqlSugarHelper.Instance.Find<ProductOrder>((o) => o.NodeAdr == productOrder.NodeAdr).FirstOrDefault();
            if (item != null)
            {
                //item.ClientName = productOrder.ClientName;
                //item.NodeType = productOrder.NodeType;
                item.NodeTypeView = productOrder.NodeTypeView;
                item.NodeIndexLang = productOrder.NodeIndexLang;
                //item.NodeAdr = productOrder.NodeAdr;
                item.UpdateTime = DateTime.Now;

                var result = SqlSugarHelper.Instance.db.Updateable(item).ExecuteCommand();
            }
        }

        public static int GetToAll()
        {
            return SqlSugarHelper.Instance.Find<ProductOrder>((o) => true).Count;
        }
        public static List<ProductOrder> GetToDayAll()
        {
            return SqlSugarHelper.Instance.Find<ProductOrder>((o) => o.AddTime.Date == DateTime.Now.Date);
        }

        public static List<ProductOrder> GetToLastAndDayAll()
        {
            return SqlSugarHelper.Instance.Find<ProductOrder>((o) => o.AddTime.Date >= DateTime.Now.AddDays(-1).Date && o.AddTime <= DateTime.Now.Date);
        }

        public static List<ProductOrder> GetToLastWeekAll()
        {
            return SqlSugarHelper.Instance.Find<ProductOrder>((o) => o.AddTime.Date >= DateTime.Now.AddDays(-7).Date && o.AddTime <= DateTime.Now.Date);
        }
        
        public static List<ProductOrder> GetToLastMonthAll()
        {
            return SqlSugarHelper.Instance.Find<ProductOrder>((o) => o.AddTime.Date >= DateTime.Now.AddDays(-30).Date && o.AddTime <= DateTime.Now.Date);
        }

        public static string GetPage(int pageIndex, int pageSize, ref int totalCount, DateTime starttime, DateTime endtime, string type, string searchText)
        {
            var exp = Expressionable.Create<ProductOrder>()
                          .And( it => it.AddTime.Date >= starttime.Date && it.AddTime.Date <= endtime.Date)
                          .AndIF(!string.IsNullOrEmpty(type), it => it.NodeIndexLang.Contains(type))
                          .AndIF(!string.IsNullOrEmpty(searchText), it => it.NodeAdr.Contains(searchText))
                          .ToExpression();//拼接表达式

            var data = SqlSugarHelper.Instance.db.Queryable<ProductOrder>().Where(exp).ToPageList(pageIndex, pageSize, ref totalCount);
            return JsonConvert.SerializeObject(data);
        }

        public static string GetItemOfDesc(string desc)
        {
            var data = SqlSugarHelper.Instance.Find<ProductOrder>((o) => o.Desc == desc).FirstOrDefault();
            return JsonConvert.SerializeObject(data);
        }

        public static string GetItemOfID(int id)
        {
            var data = SqlSugarHelper.Instance.Find<ProductOrder>((o) => o.Id == id).FirstOrDefault();
            return JsonConvert.SerializeObject(data);
        }

        public static string GetItemOfMO(string mo)
        {
            var data = SqlSugarHelper.Instance.Find<ProductOrder>((o) => o.MO == mo).FirstOrDefault();
            return JsonConvert.SerializeObject(data);
        }

        public static List<ProductOrder> GetItemOfSO(string so)
        {
            var data = SqlSugarHelper.Instance.Find<ProductOrder>((o) => o.SOItem == so);
            return data;
        }
        public static List<ProductOrder> GetAll()
        {
            return SqlSugarHelper.Instance.Find<ProductOrder>((o) => true);
        }
        public static ProductOrder GetOfNodeAdr(string nodeAdr)
        {
            return SqlSugarHelper.Instance.Find<ProductOrder>((o) => o.NodeAdr.Equals(nodeAdr)).FirstOrDefault();
        }
    }
}
