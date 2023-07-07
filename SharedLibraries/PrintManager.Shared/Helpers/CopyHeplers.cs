using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Shared.Helpers
{
    public class CopyHeplers
    {
        public static T DeepCopyByBin<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                //序列化成流
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                //反序列化成对象
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }

        public static object Clone(object obj)
        {
            Type t = obj.GetType();
            PropertyInfo[] properties = t.GetProperties();
            object p = t.InvokeMember("", BindingFlags.CreateInstance, null, obj, null);
            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite)
                {
                    object value = property.GetValue(obj, null);
                    property.SetValue(p, value, null);
                }
            }
            return p;
        }
    }
}
