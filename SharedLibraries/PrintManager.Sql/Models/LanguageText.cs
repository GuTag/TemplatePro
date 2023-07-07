using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Sql.Models
{
    public class LanguageText
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [SugarColumn(IsNullable = true)]

        public string Index { get; set; }

        [SugarColumn(IsNullable = true)]
        public string Language_zh { get; set; }

        [SugarColumn(IsNullable = true)]
        public string Language_cn { get; set; }

        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public DateTime AddTime { get; set; }
    }
}
