﻿using Newtonsoft.Json;
using Opc.Ua;
using PrintManager.Shared.Enums;
using PrintManager.Sql.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Sql.BLL
{
    public class LanguageTextBLL
    {
        public static bool Add(string stringItem)
        {
            var item = JsonConvert.DeserializeObject<LanguageText>(stringItem);
            item.AddTime = DateTime.Now;
            var result = SqlSugarHelper.Instance.db.Insertable(item).ExecuteCommand();
            return result > 0;
        }

        public static bool Add(LanguageText item)
        {
            var isExit = false;
            var getItem = SqlSugarHelper.Instance.Find<LanguageText>((o) => o.Index == item.Index).FirstOrDefault();
            if (getItem == null )
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
            var items = JsonConvert.DeserializeObject<List<LanguageText>>(stringItems);
            var itemList = new List<LanguageText>();
            foreach (var item in items)
            {
                var getItem = SqlSugarHelper.Instance.Find<LanguageText>((o) => o.Index == item.Index).FirstOrDefault();
                if (getItem == null)
                {
                    itemList.Add(item);
                }
            }
            var result = SqlSugarHelper.Instance.db.Insertable(itemList).ExecuteCommand();
            return result > 0;
        }
        public static bool AddList(List<LanguageText> items)
        {
            var itemList = new List<LanguageText>();
            foreach (var item in items)
            {
                var getItem = SqlSugarHelper.Instance.Find<LanguageText>((o) => o.Index == item.Index).FirstOrDefault();
                if (getItem == null)
                {
                    itemList.Add(item);
                }
            }
            var result = SqlSugarHelper.Instance.db.Insertable(itemList).ExecuteCommand();
            return result > 0;
        }

        public static void Delete(string item)
        {
            var result = SqlSugarHelper.Instance.db.Deleteable<LanguageText>((o) => o.Index == item).ExecuteCommand();
        }

        public static string GetPage(int pageIndex, int pageSize, ref int totalCount, DateTime starttime, DateTime endtime, string searchText)
        {
            var exp = Expressionable.Create<LanguageText>()
                          .And(it => it.AddTime.Date >= starttime.Date && it.AddTime.Date <= endtime.Date)
                          .AndIF(!string.IsNullOrEmpty(searchText), it => it.Index.Contains(searchText))
                          .ToExpression();//拼接表达式

            var data = SqlSugarHelper.Instance.db.Queryable<LanguageText>().Where(exp).ToPageList(pageIndex, pageSize, ref totalCount);
            return JsonConvert.SerializeObject(data);
        }

        public static LanguageText Find(string searchText)
        {
            var exp = Expressionable.Create<LanguageText>()

                          .AndIF(!string.IsNullOrEmpty(searchText), it => it.Index.Contains(searchText))
                          .ToExpression();//拼接表达式

            LanguageText data = SqlSugarHelper.Instance.db.Queryable<LanguageText>().Where(exp).First();
            return data;
        }
        public static int GetToAll()
        {
            return SqlSugarHelper.Instance.Find<LanguageText>((o) => true).Count;
        }

        public static string GetOfIndex(string Index, LanguageSet language)
        {
            var data = SqlSugarHelper.Instance.Find<LanguageText>((o) => o.Index == Index).FirstOrDefault();
            var str = "";
            if (data != null) { 
                if (language == LanguageSet.Language_cn) str = data.Language_cn;
                if (language == LanguageSet.Language_zh) str = data.Language_zh;
            }
            return str;
        }

        public static void UpdateOfIndex(LanguageText language)
        {
            var item = SqlSugarHelper.Instance.Find<LanguageText>((o) => o.Index == language.Index).FirstOrDefault();
            if (item != null)
            {
                item.Language_cn = language.Language_cn;
                item.Language_zh = language.Language_zh;
                var result = SqlSugarHelper.Instance.db.Updateable(item).ExecuteCommand();            
            }
        }
    }
}
