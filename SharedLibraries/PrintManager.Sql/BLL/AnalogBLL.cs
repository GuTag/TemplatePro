using Newtonsoft.Json;
using Opc.Ua;
using Org.BouncyCastle.Crypto.Tls;
using PrintManager.Sql.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Sql.BLL
{
    public class AnalogBLL
    {
        public static bool Add(string stringItem)
        {
            var item = JsonConvert.DeserializeObject<Analog>(stringItem);
            item.AddTime = DateTime.Now;
            var result = SqlSugarHelper.Instance.db.Insertable(item).ExecuteCommand();
            return result > 0;
        }

        public static bool Add(Analog item)
        {
            var isExit = false;
            var getItem = SqlSugarHelper.Instance.Find<Analog>((o) => o.NodeAdr == item.NodeAdr).FirstOrDefault();
            if (getItem == null)
            {
                item.AddTime = DateTime.Now;
                var result = SqlSugarHelper.Instance.db.Insertable(item).ExecuteCommand();
            }
            else
            {
                isExit = true;
            }
            return isExit;
        }
        public static bool AddList(string stringItems)
        {
            var items = JsonConvert.DeserializeObject<List<Analog>>(stringItems);
            var itemList = new List<Analog>();
            foreach (var item in items)
            {
                var getItem = SqlSugarHelper.Instance.Find<Analog>((o) => o.NodeAdr == item.NodeAdr).FirstOrDefault();
                if (getItem == null)
                {
                    itemList.Add(item);
                }
            }
            var result = SqlSugarHelper.Instance.db.Insertable(itemList).ExecuteCommand();
            return result > 0;
        }
        public static bool AddList(List<Analog> items)
        {
            var itemList = new List<Analog>();
            foreach (var item in items)
            {
                var getItem = SqlSugarHelper.Instance.Find<Analog>((o) => o.NodeAdr == item.NodeAdr).FirstOrDefault();
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
            var result = SqlSugarHelper.Instance.db.Deleteable<Analog>((o) => o.NodeAdr == NodeAdr).ExecuteCommand();
        }

        public static string GetPage(int pageIndex, int pageSize, ref int totalCount, DateTime starttime, DateTime endtime, string searchText)
        {
            var exp = Expressionable.Create<Analog>()
                          .And(it => it.AddTime.Date >= starttime.Date && it.AddTime.Date <= endtime.Date)
                          .AndIF(!string.IsNullOrEmpty(searchText), it => it.NodeAdr.Contains(searchText))
                          .ToExpression();//拼接表达式

            var data = SqlSugarHelper.Instance.db.Queryable<Analog>().Where(exp).ToPageList(pageIndex, pageSize, ref totalCount);
            return JsonConvert.SerializeObject(data);
        }
        public static int GetToAll()
        {
            return SqlSugarHelper.Instance.Find<Analog>((o) => true).Count;
        }

        public static List<Analog> GetAll()
        {
            return SqlSugarHelper.Instance.Find<Analog>((o) => true);
        }

        public static Analog GetOfNodeAdr(string nodeAdr)
        {
            return SqlSugarHelper.Instance.Find<Analog>((o) => o.NodeAdr.Equals(nodeAdr)).FirstOrDefault();
        }

        public static void UpdateOfNodeAdr(Analog analog)
        {
            var item = SqlSugarHelper.Instance.Find<Analog>((o) => o.NodeAdr == analog.NodeAdr).FirstOrDefault();

            if (item != null) {
                //item.ClientName = analog.ClientName;
                //item.NodeType = analog.NodeType;
                item.NodeTypeView = analog.NodeTypeView;
                 //item.NodeAdr = analog.NodeAdr;
                item.NodeValHH = analog.NodeValHH;
                item.NodeValH = analog.NodeValH;
                item.NodeValLL = analog.NodeValLL;
                item.NodeValL = analog.NodeValL;
                item.NodeLanguageHH = analog.NodeLanguageHH;
                item.NodeLanguageH = analog.NodeLanguageH;
                item.NodeLanguageLL = analog.NodeLanguageLL;
                item.NodeLanguageL = analog.NodeLanguageL;
                item.NodeUnit = analog.NodeUnit;
                 item.NodeDes = analog.NodeDes; 
                 var result = SqlSugarHelper.Instance.db.Updateable(item).ExecuteCommand();
            }
        }

    }
}
