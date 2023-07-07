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
    [SugarIndex("unique_code", nameof(Code), OrderByType.Desc, true)]
    public class Customer
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Code { get; set; }

        [SugarColumn(IsNullable = true)]
        public string Name { get; set; }

        [SugarColumn(IsOnlyIgnoreUpdate=true)]
        public DateTime AddTime { get; set; } = DateTime.Now;


    }
}
