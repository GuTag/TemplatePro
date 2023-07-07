using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Shared.Attributes
{
    public class RemarkAttribute : Attribute
    {
        private readonly string remark;

        public RemarkAttribute(string remark)
        {
            this.remark = remark;
        }

        public string GetRemark()
        {
            return remark;
        }
    }

    public static class RemarkExtension
    {
        public static string GetRemark(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo field = type.GetField(value.ToString());
            if (field.IsDefined(typeof(RemarkAttribute), true))
            {
                RemarkAttribute attr = field.GetCustomAttribute<RemarkAttribute>();
                return attr.GetRemark();
            }
            return value.ToString();
        }
    }
}
