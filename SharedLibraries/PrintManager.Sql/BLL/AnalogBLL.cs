using Newtonsoft.Json;
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
            item.AddTime = DateTime.Now;
            var result = SqlSugarHelper.Instance.db.Insertable(item).ExecuteCommand();
            return result > 0;
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

        public static bool Delete(string item)
        {
            throw new NotImplementedException();
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

    }
}
