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
    public class HistoryLog
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public int LogType { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }

        [SugarColumn(IsOnlyIgnoreUpdate=true)]
        public DateTime AddTime { get; set; } = DateTime.Now;


    }
}
