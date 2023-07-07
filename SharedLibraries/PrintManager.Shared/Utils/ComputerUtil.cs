using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Shared.Utils
{
    public class ComputerUtil
    {
        public static IPHostEntry ME
        {
            get=>Dns.GetHostEntry(Dns.GetHostName());
        }
        public static IPAddress[] GetIPAddress()
        {
            return ME.AddressList;
        }
    }
}
