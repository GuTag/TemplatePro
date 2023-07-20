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
    //[SugarIndex("unique_Desc", nameof(Desc), OrderByType.Desc, true)]
    //[SugarIndex("unique_MO", nameof(MO), OrderByType.Desc, true)]
    //[SugarIndex("unique_SOItem", nameof(SOItem), OrderByType.Desc, true)]
    public class ParModify
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(IsNullable = true)]
        public string Index { get; set; }

        [SugarColumn(IsNullable = true)]
        public string NodeAdr { get; set; }
        [SugarColumn(IsNullable = true)]
        public string ClientName { get; set; }
        [SugarColumn(IsNullable = true)]
        public string NodeDes { get; set; }
        [SugarColumn(IsNullable = true)]
        public string ActualValue { get; set; }
        public DateTime UpdateTime { get; set; }

        [SugarColumn(IsOnlyIgnoreUpdate=true)]
        public DateTime AddTime { get; set; } 


    }
}
