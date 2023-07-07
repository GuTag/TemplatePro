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
    public class ProductOrder
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [SugarColumn(IsNullable = true)]
        public bool IsOK { get; set; }

        [SugarColumn(IsNullable = true)]
        public string ClientName { get; set; }
        [SugarColumn(IsNullable = true)]
        public string NodeType { get; set; }
        [SugarColumn(IsNullable = true)]
        public string NodeTypeView { get; set; }
        [SugarColumn(IsNullable = true)]
        public string NodeAdr { get; set; }
        [SugarColumn(IsNullable = true)]
        public string NodeDes { get; set; }
        [SugarColumn(IsNullable = true)]
        public string NodeIndexLang { get; set; }
        
        [SugarColumn(IsNullable = true)]
        public bool IsPrinting { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Desc { get; set; }
        [SugarColumn(IsNullable = true)]
        public string ItemNo { get; set; }
        [SugarColumn(IsNullable = true)]
        public string MO { get; set; }
        [SugarColumn(IsNullable = true)]
        public string SOItem { get; set; }

        [SugarColumn(IsNullable = true)]
        public string MtlNo { get; set; }

        [SugarColumn(IsNullable = true)]
        public string NewItemNO { get; set; }

        [SugarColumn(IsNullable = true)]
        public string CustomerCode { get; set; }

        [SugarColumn(IsNullable =true)]
        public string CPQCode { get; set; }
        [SugarColumn(IsNullable = true)]
        public int ProductOrderType { get; set; }
        [SugarColumn(IsNullable = true)]
        public int RequestNum { get; set; }
        [SugarColumn(IsNullable = true)]
        public int ComplatedNum { get; set; }
        public DateTime UpdateTime { get; set; }

        [SugarColumn(IsOnlyIgnoreUpdate=true)]
        public DateTime AddTime { get; set; } 


    }
}
