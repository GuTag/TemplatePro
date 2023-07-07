using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PrintManager.Sql.Models
{
    [SugarIndex("unique_username", nameof(UserName), OrderByType.Desc, true)]
    public class User
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }



    }
}
