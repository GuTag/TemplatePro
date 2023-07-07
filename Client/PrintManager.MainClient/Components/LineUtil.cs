using PrintManager.Shared.TCP;
using PrintManager.Shared.Utils;
using PrintManager.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.MainClient.Components
{
    public class LineUtil
    {
        /// <summary>
        /// 获取当前生产线上的MO订单
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static (string, string) GetLineMO(int line)
        {
            var ip = IniUtil.IniReadvalue(Environments.ConfigFilePath, "ClientIP", line.ToString());
            if (string.IsNullOrEmpty(ip)) return ("", "");
            foreach (string iphost in TCPServer.Instance.ClientDictionary.Keys)
            {
                if (iphost.Contains(ip))    
                {
                    var mo = TCPServer.Instance.ClientDictionary[iphost];
                    return (iphost, mo);
                }
            }
            return ("", "");
        }
        public static int GetLineNum(string ip)
        {
            var serverIP = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "IP");
            var ConfigClent1 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "ClientIP", "1");
            var ConfigClent2 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "ClientIP", "2");
            var ConfigClent3 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "ClientIP", "3");
            if (ip.StartsWith(serverIP))
            {
                return 0;
            }
            else if (ip.StartsWith(ConfigClent1))
            {
                return 1;
            }
            else if (ip.StartsWith(ConfigClent2))
            {
                return 2;
            }
            else if (ip.StartsWith(ConfigClent3))
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }

        public static string GetLineName(string ip)
        {
            var serverIP = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "IP");
            var ConfigClent1 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "ClientIP", "1");
            var ConfigClent2 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "ClientIP", "2");
            var ConfigClent3 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "ClientIP", "3");
            if (ip.StartsWith(serverIP))
            {
                return "系统";
            }
            else if(ip.StartsWith(ConfigClent1))
            {
                return "1线";
            }
            else if (ip.StartsWith(ConfigClent2))
            {
                return "2线";
            }
            else if (ip.StartsWith(ConfigClent3))
            {
                return "3线";
            }
            else
            {
                return "外部";
            }
        }
    }
}
