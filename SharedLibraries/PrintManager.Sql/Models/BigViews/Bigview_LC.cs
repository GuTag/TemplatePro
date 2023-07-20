using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Sql.Models.BigViews
{
    public class Bigview_LC
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(IsNullable = true)]
        public int CC01 { get; set; }

        [SugarColumn(IsNullable = true)]
        public int CC02 { get; set; }
        [SugarColumn(IsNullable = true)]
        public int CH01 { get; set; }
        [SugarColumn(IsNullable = true)]
        public int CH02 { get; set; }
        [SugarColumn(IsNullable = true)]
        public int ALV { get; set; }
        [SugarColumn(IsNullable = true)]
        public int SW01 { get; set; }
        [SugarColumn(IsNullable = true)]
        public int SW11 { get; set; }
        [SugarColumn(IsNullable = true)]
        public int M002 { get; set; }

        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public DateTime AddTime { get; set; }
    }
}
