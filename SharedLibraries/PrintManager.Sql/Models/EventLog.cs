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
    public class EventLog
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [SugarColumn(IsNullable = true)]
        public string ClientName { get; set; }
        [SugarColumn(IsNullable = true)]
        public string NodeAdr { get; set; }
        [SugarColumn(IsNullable = true)]
        public string ActualValue { get; set; }
        [SugarColumn(IsNullable = true)]
        public int LogType { get; set; }
        [SugarColumn(IsNullable = true)]
        public string NodeTypeView { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Message { get; set; }

        [SugarColumn(IsOnlyIgnoreUpdate=true)]
        public DateTime AddTime { get; set; } = DateTime.Now;


    }
}
