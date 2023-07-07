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
    public class UserBLL
    {
        public static string Login(string username, string password)
        {
            var data = SqlSugarHelper.Instance.Find<User>((o) => o.UserName == username && o.Password == password);
            if(data.Count> 0) 
            { 
                return JsonConvert.SerializeObject(data.FirstOrDefault());
            }
            else
            {
                return null;
            }
            
        }

        public static bool Add(string username, string password)
        {
            var item = new User() { UserName= username, Password = password };
            var result = SqlSugarHelper.Instance.db.Insertable<User>(item).ExecuteCommand();
            return result > 0;
        }
    }
}
