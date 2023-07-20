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
    public class ParModifyBLL
    {
        public static bool Add(string stringItem)
        {
            var item = JsonConvert.DeserializeObject<ParModify>(stringItem);
            item.AddTime = DateTime.Now;
            var result = SqlSugarHelper.Instance.db.Insertable(item).ExecuteCommand();
            return result > 0;
        }

        public static bool Add(ParModify item)
        {
            var isExit = false;
            var getItem = SqlSugarHelper.Instance.Find<ParModify>((o) => o.NodeAdr == item.NodeAdr).FirstOrDefault();
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
            var items = JsonConvert.DeserializeObject<List<ParModify>>(stringItems);
            var itemList = new List<ParModify>();
            foreach (var item in items)
            {
                var getItem = SqlSugarHelper.Instance.Find<ParModify>((o) => o.NodeAdr == item.NodeAdr).FirstOrDefault();
                if (getItem == null)
                {
                    itemList.Add(item);
                }
            }
            var result = SqlSugarHelper.Instance.db.Insertable(itemList).ExecuteCommand();
            return result > 0;
        }
        public static bool AddList(List<ParModify> items)
        {
            var itemList = new List<ParModify>();
            foreach (var item in items)
            {
                var getItem = SqlSugarHelper.Instance.Find<ParModify>((o) => o.NodeAdr == item.NodeAdr).FirstOrDefault();
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
            var result = SqlSugarHelper.Instance.db.Deleteable<ParModify>((o) => o.NodeAdr == NodeAdr).ExecuteCommand();
        }

        public static string GetPage(int pageIndex, int pageSize, ref int totalCount, string searchText)
        {
            var exp = Expressionable.Create<ParModify>()
                          .AndIF(!string.IsNullOrEmpty(searchText), it => it.NodeAdr.Contains(searchText))
                          .OrIF(!string.IsNullOrEmpty(searchText), it => it.NodeDes.Contains(searchText))
                          .ToExpression();//拼接表达式

            var data = SqlSugarHelper.Instance.db.Queryable<ParModify>().Where(exp).ToPageList(pageIndex, pageSize, ref totalCount);
            return JsonConvert.SerializeObject(data);
        }
        public static int GetToAll()
        {
            return SqlSugarHelper.Instance.Find<ParModify>((o) => true).Count;
        }

        public static List<ParModify> GetAll()
        {
            return SqlSugarHelper.Instance.Find<ParModify>((o) => true);
        }

        public static ParModify GetOfNodeAdr(string nodeAdr)
        {
            return SqlSugarHelper.Instance.Find<ParModify>((o) => o.NodeAdr.Equals(nodeAdr)).FirstOrDefault();
        }

        public static void UpdateOfNodeAdr(ParModify parModify)
        {
            var item = SqlSugarHelper.Instance.Find<ParModify>((o) => o.NodeAdr == parModify.NodeAdr).FirstOrDefault();

            if (item != null)
            {
                //item.ClientName = analog.ClientName;
                //item.NodeAdr = analog.NodeAdr;
                item.ActualValue = parModify.ActualValue;
                item.NodeDes = parModify.NodeDes;
                item.UpdateTime = DateTime.Now;
                var result = SqlSugarHelper.Instance.db.Updateable(item).ExecuteCommand();
            }
        }

    }
}
