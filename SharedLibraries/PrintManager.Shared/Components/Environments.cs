using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Shared
{
    public class Environments
    {
        public static string AppDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(_appDataPath))
                {
                    _appDataPath = Environment.ExpandEnvironmentVariables(Fields.AppDataPath);
                }
                if (!Directory.Exists(_appDataPath))
                {
                    Directory.CreateDirectory(_appDataPath);
                }
                return _appDataPath;
            }
        }
        private static string _appDataPath;

        //日志路径
        public static string LogFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_logFilePath))
                {
                    _logFilePath = Path.Combine(AppDataPath, Fields.LogFileName);
                }
                return _logFilePath;
            }
        }
        private static string _logFilePath;

        //标签模板路径(folder)
        public static string PrintFolderPath
        {
            get
            {
                if (string.IsNullOrEmpty(_printFilePath))
                {
                    _printFilePath = Path.Combine(AppDataPath, Fields.PrintFolderName);
                    if (!Directory.Exists(_printFilePath))
                    {
                        Directory.CreateDirectory(_printFilePath);
                    }
                }
                return _printFilePath;
            }
        }
        private static string _printFilePath;

        //配置文件路径
        public static string ConfigFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_configFilePath))
                {
                    _configFilePath = Path.Combine(AppDataPath, Fields.ConfigFileName);
                }
                return _configFilePath;
            }
        }
        private static string _configFilePath;
    }
}
