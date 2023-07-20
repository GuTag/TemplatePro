using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Sql.Models.BigViews
{
    public class Bigview_RC
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(IsNullable = true)]
        public int Waster { get; set; }

        [SugarColumn(IsNullable = true)]
        public int WasterSolvent { get; set; }

        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public DateTime AddTime { get; set; }
    }
}
