using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Sql.BLL
{
    public class ProductOrderLogBLL
    {
        public static bool AddOfUpdata(ProductOrderLog product)
        {
            //MO
            var data = SqlSugarHelper.Instance.Find<ProductOrderLog>((o) => o.MO == product.MO).FirstOrDefault();
            if (data != null)
            {
                data.ComplatedNum = product.ComplatedNum;
                var updata = SqlSugarHelper.Instance.db.Updateable(data).ExecuteCommand();
                return updata > 0;
            }
            var result = SqlSugarHelper.Instance.db.Insertable(product).ExecuteCommand();
            return result > 0;
        }

        public static List<ProductOrderLog> GetToAll()
        {
            return SqlSugarHelper.Instance.Find<ProductOrderLog>((o) => true);
        }
        public static List<ProductOrderLog> GetToDayAll()
        {
            return SqlSugarHelper.Instance.Find<ProductOrderLog>((o) => o.AddTime >= DateTime.Now.AddDays(-1) && o.AddTime <= DateTime.Now);
        }

        public static List<ProductOrderLog> GetToLastAndDayAll()
        {
            return SqlSugarHelper.Instance.Find<ProductOrderLog>((o) => o.AddTime >= DateTime.Now.AddDays(-2) && o.AddTime <= DateTime.Now);
        }

        public static List<ProductOrderLog> GetToLastWeekAll()
        {
            return SqlSugarHelper.Instance.Find<ProductOrderLog>((o) => o.AddTime >= DateTime.Now.AddDays(-7) && o.AddTime <= DateTime.Now);
        }

        public static List<ProductOrderLog> GetToLastMonthAll()
        {
            return SqlSugarHelper.Instance.Find<ProductOrderLog>((o) => o.AddTime >= DateTime.Now.AddDays(-30) && o.AddTime <= DateTime.Now);
        }

        public static List<ProductOrderLog> GetPage(int pageIndex, int pageSize, ref int totalCount, DateTime starttime, DateTime endtime,string line,string type, string searchText)
        {

            var exp = Expressionable.Create<ProductOrderLog>()
                          .And(it => it.AddTime.Date >= starttime.Date && it.AddTime.Date <= endtime.Date)
                          .AndIF(line != "",it => it.Line.Contains(line))
                          .AndIF(type != "",it => it.ProdType.Contains(type))
                          .AndIF(!string.IsNullOrEmpty(searchText), it => it.MO.Contains(searchText) || it.Desc.Contains(searchText))
                          .ToExpression();//拼接表达式

            var data = SqlSugarHelper.Instance.db.Queryable<ProductOrderLog>().Where(exp).ToPageList(pageIndex, pageSize, ref totalCount);
            return data;
        }


    }
}
