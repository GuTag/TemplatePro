using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Shared.Utils
{
    /// <summary>
    /// IniFiles 的摘要说明。
    /// 示例文件路径：C:\file.ini
    /// [Server]            //[*] 表示缓存区
    /// name=localhost      //name 表示主键，localhost 表示值
    /// </summary>
    public class IniUtil
    {

        [DllImport("kernel32")] //返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(byte[] section, byte[] key, byte[] val, string filePath);
        [DllImport("kernel32")] //返回取得字符串缓冲区的长度
        private static extern int GetPrivateProfileString(byte[] section, byte[] key, byte[] def, byte[] retVal, int size, string filePath);


        /// <summary>
        /// 写Ini文件
        /// 调用示例：ini.IniWritevalue("Server","name","localhost");
        /// </summary>
        /// <param name="Section">[缓冲区]</param>
        /// <param name="Key">键</param>
        /// <param name="value">值</param>
        public static void IniWritevalue(string path,string Section, string Key, object value)
        {
            string encodingName = "UTF-8";
            if (value == null) value = "";
            WritePrivateProfileString(getBytes(Section, encodingName), getBytes(Key, encodingName), getBytes(value.ToString(), encodingName), path);
        }

        /// <summary>
        /// 读Ini文件
        /// 调用示例：ini.IniWritevalue("Server","name");
        /// </summary>
        /// <param name="Section">[缓冲区]</param>
        /// <param name="Key">键</param>
        /// <returns>值</returns>
        public static string IniReadvalue(string path, string Section, string Key)
        {
            byte[] buffer = new byte[1024];
            string encodingName = "UTF-8";
            int count = GetPrivateProfileString(getBytes(Section, encodingName), getBytes(Key, encodingName), getBytes("", encodingName), buffer, 1024, path);
            return Encoding.GetEncoding(encodingName).GetString(buffer, 0, count).Trim();
        }

        public static byte[] getBytes(string s, string encodingName)
        {
            return null == s? null:Encoding.GetEncoding(encodingName).GetBytes(s);
        }

    }
}
