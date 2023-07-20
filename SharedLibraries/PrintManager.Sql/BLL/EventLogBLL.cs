using Newtonsoft.Json;
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
    public class EventLogBLL
    {
        public static bool Add(string itemString)
        {
            var item = JsonConvert.DeserializeObject<EventLog>(itemString);
            var result = SqlSugarHelper.Instance.db.Insertable(item).ExecuteCommand();
            return result > 0;
        }

        public static bool AddList(string stringItems)
        {
            var items = JsonConvert.DeserializeObject<List<EventLog>>(stringItems);
            var result = SqlSugarHelper.Instance.db.Insertable(items).ExecuteCommand();
            return result > 0;
        }

        public static string GetPage(int pageIndex, int pageSize, ref int totalCount, DateTime starttime, DateTime endtime, int type, string searchText)
        {
            var exp = Expressionable.Create<EventLog>()
                          .And(it => it.AddTime.Date >= starttime.Date && it.AddTime.Date <= endtime.Date)
                          .AndIF(type != 0, it => it.LogType == type)
                          .AndIF(!string.IsNullOrEmpty(searchText), it => it.Message.Contains(searchText) || it.NodeAdr.Contains(searchText))
                          .ToExpression();//拼接表达式

            var data = SqlSugarHelper.Instance.db.Queryable<EventLog>().Where(exp).ToPageList(pageIndex, pageSize, ref totalCount);
            return JsonConvert.SerializeObject(data);
        }

        public static bool Delete(string item)
        {
            throw new NotImplementedException();
        }

        

        public static string GetAll()
        {
            var data = SqlSugarHelper.Instance.Find<EventLog>((o) => true);
            return JsonConvert.SerializeObject(data);
        }

    }
}
