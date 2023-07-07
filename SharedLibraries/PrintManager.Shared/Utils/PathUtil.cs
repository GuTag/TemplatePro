using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Shared.Utils
{
    public class PathUtil
    {
        /// <summary>
        /// 根路径
        /// </summary>
        public static string GetLocalPath
        {
            get  //AppDomain.CurrentDomain.BaseDirectory;
            { 
                string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string data_path = Path.Combine(local, "PrintManager");
                if (!Directory.Exists(data_path))
                {
                    Directory.CreateDirectory(data_path);
                }
                return data_path;
            }
        }

        /// <summary>
        /// 根路径
        /// </summary>
        public static string GetBasePath
        {
            get 
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static string ConfigPath
        {
            get
            {
                string[] sections = { "General" };

                var _folder = Path.Combine(GetBasePath,"Config");
                if (!Directory.Exists(_folder))
                {
                    Directory.CreateDirectory(_folder);
                }
                var _path = Path.Combine(_folder, "Config.ini");
                if (!File.Exists(_path))
                {
                    File.Create(_path).Close();
                    StreamWriter fs = System.IO.File.AppendText(_path);
                    foreach (string section in sections)
                    {
                        fs.WriteLine($"[{section}]");
                    }
                    fs.Close();

                    IniUtil.IniWritevalue(_path, "General", "IP", "127.0.0.1");
                    IniUtil.IniWritevalue(_path, "General", "Port", "5500");
                }
                return _path;
            }
        }

        /// <summary>
        ///标签文件夹路径
        /// </summary>
        public static string LablePath
        {
            get
            {
                var _path = Path.Combine(GetLocalPath, "Lable");
                if (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                }
                return _path;
            }
        }
    }
}
