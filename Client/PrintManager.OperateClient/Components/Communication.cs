using Newtonsoft.Json;
using PrintManager.Shared.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.OperateClient.Components
{
    public class Communication
    {
        public static void TcpSendData_MO(object data)
        {
            TCPClient.Instance.Send(new RequestInfo("MO", JsonConvert.SerializeObject(data)));
        }

        public static void TcpSendData_Print(object data)
        {
            TCPClient.Instance.Send(new RequestInfo("PRINT", JsonConvert.SerializeObject(data)));
        }

        public static void TcpSendData_Manual(object data)
        {
            TCPClient.Instance.Send(new RequestInfo("MANUAL", JsonConvert.SerializeObject(data)));
        }

        public static void TcpSendData_Clean(object data)
        {
            TCPClient.Instance.Send(new RequestInfo("CLEAN", JsonConvert.SerializeObject(data)));
        }

        public static void TcpSendData_ProductOK(object data)
        {
            TCPClient.Instance.Send(new RequestInfo("PRODUCTOK",JsonConvert.SerializeObject(data)));
        }
    }
}
