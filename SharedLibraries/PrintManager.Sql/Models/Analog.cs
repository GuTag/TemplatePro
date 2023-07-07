using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Sql.Models
{
    public class Analog
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [SugarColumn(IsNullable = true)]
        public string ClientName { get; set; }
        [SugarColumn(IsNullable = true)]
        public string NodeType { get; set; }
        [SugarColumn(IsNullable = true)]
        public string NodeTypeView { get; set; }
        [SugarColumn(IsNullable = true)]
        public string NodeAdr { get; set; }
        [SugarColumn(IsNullable = true)]
        public int NodeValHH { get; set; }
        [SugarColumn(IsNullable = true)]
        public int NodeValH { get; set; }
        [SugarColumn(IsNullable = true)]
        public int NodeValLL { get; set; }
        [SugarColumn(IsNullable = true)]
        public int NodeValL { get; set; }
        [SugarColumn(IsNullable = true)]
        public string NodeLanguageHH { get; set; }
        [SugarColumn(IsNullable = true)]
        public string NodeLanguageH { get; set; }
        [SugarColumn(IsNullable = true)]
        public string NodeLanguageLL { get; set; }
        [SugarColumn(IsNullable = true)]
        public string NodeLanguageL { get; set; }
        [SugarColumn(IsNullable = true)]
        public string NodeUnit { get; set; }
        [SugarColumn(IsNullable = true)]
        public string NodeDes { get; set; }
        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public DateTime AddTime { get; set; }
    }
}
