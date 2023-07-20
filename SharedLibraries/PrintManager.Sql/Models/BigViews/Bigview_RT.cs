using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Sql.Models.BigViews
{
    public class Bigview_RT
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(IsNullable = true)]
        public int TotalEnergy { get; set; }

        [SugarColumn(IsNullable = true)]
        public int TotalAir { get; set; }

        [SugarColumn(IsNullable = true)]
        public int TotalWater { get; set; }
        [SugarColumn(IsNullable = true)]
        public int TotalSolvent { get; set; }

        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public DateTime AddTime { get; set; }
    }
}
