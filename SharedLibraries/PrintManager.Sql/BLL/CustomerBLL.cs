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
    public class CustomerBLL
    {
        public static bool Add(string code)
        {
            var item = new Customer() { Code = code };
            item.AddTime= DateTime.Now;
            var result = SqlSugarHelper.Instance.db.Insertable(item).ExecuteCommand();
            return result > 0;
        }

        //public static bool AddList(string stringItems)
        //{
        //    var items = JsonConvert.DeserializeObject<List<Customer>>(stringItems);
        //    var result = SqlSugarHelper.Instance.db.Insertable(items).ExecuteCommand();
        //    return result > 0;
        //}

        public static bool AddList(List<string> codes)
        {
            var itemList = new List<Customer>();
            foreach (var code in codes)
            {
                var getItem = SqlSugarHelper.Instance.Find<Customer>((o) => o.Code ==code).FirstOrDefault();
                if (getItem == null)
                {
                    itemList.Add(new Customer() { Code = code});
                }
            }
            var result = SqlSugarHelper.Instance.db.Insertable(itemList).ExecuteCommand();
            return result > 0;
        }

        public static bool Delete(string item)
        {
            throw new NotImplementedException();
        }

        

        public static string GetAll()
        {
            var data = SqlSugarHelper.Instance.Find<Customer>((o) => true);
            return JsonConvert.SerializeObject(data);
        }

        public static List<string> GetAllCodes()
        {
            var data = SqlSugarHelper.Instance.Find<Customer>((o) => true);
            return data.Select(o=>o.Code).ToList();
        }

    }
}
