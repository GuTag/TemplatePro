using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Sql.BLL
{
     public  class ProductOrderLog
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string MO { get; set; }
        public string Client { get; set; }
        public string Line { get; set; }
        public string ProdType { get; set; }
        public string Desc { get; set; }
        public string ItemNo { get; set; }
        public int RequestNum { get; set; }
        public int ComplatedNum { get; set; }
        public bool IsOK { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Message { get; set; }
        [SugarColumn(IsNullable = true)]
        public int ProducNum { get; set; }
        public DateTime AddTime { get; set; }
    }
}
