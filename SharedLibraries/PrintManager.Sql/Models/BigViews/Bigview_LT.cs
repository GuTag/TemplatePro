using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Sql.Models.BigViews
{
    public class Bigview_LT
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(IsNullable = true)]
        public int EventNbr { get; set; }

        [SugarColumn(IsNullable = true)]
        public int FaultNbr { get; set; }

        [SugarColumn(IsNullable = true)]
        public int WarnNbr { get; set; }

        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public DateTime AddTime { get; set; }
    }
}
