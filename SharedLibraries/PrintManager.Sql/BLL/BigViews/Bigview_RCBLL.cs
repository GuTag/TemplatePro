﻿using Newtonsoft.Json;
using PrintManager.Sql.Models;
using PrintManager.Sql.Models.BigViews;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Sql.BLL.BigViews
{
    public class Bigview_RCBLL
    {
        public static bool Add(string stringItem)
        {
            var item = JsonConvert.DeserializeObject<Bigview_RC>(stringItem);
            item.AddTime = DateTime.Now;
            var result = SqlSugarHelper.Instance.db.Insertable(item).ExecuteCommand();
            return result > 0;
        }

        public static bool Add(Bigview_RC item)
        {
            item.AddTime = DateTime.Now;
            var result = SqlSugarHelper.Instance.db.Insertable(item).ExecuteCommand();
            return result > 0;
        }
        public static bool AddList(string stringItems)
        {
            var items = JsonConvert.DeserializeObject<List<Bigview_RC>>(stringItems);
            var itemList = new List<Bigview_RC>();
            foreach (var item in items)
            {
                 itemList.Add(item);
            }
            var result = SqlSugarHelper.Instance.db.Insertable(itemList).ExecuteCommand();
            return result > 0;
        }
        public static bool AddList(List<Bigview_RC> items)
        {
            var itemList = new List<Bigview_RC>();
            foreach (var item in items)
            {
                    itemList.Add(item);

            }
            var result = SqlSugarHelper.Instance.db.Insertable(itemList).ExecuteCommand();
            return result > 0;
        }


    }
}
