using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Sockets;

namespace PrintManager.Shared.TCP
{
    /// <summary>
    /// tcpip状态更新消息
    /// </summary>
    public class TCPRecviceMsg
    {
        /// <summary>
        /// IPHost
        /// </summary>
        public string IP { get; set; } 


        /// <summary>
        /// 文本
        /// </summary>
        public RequestInfo Message { get; set; }
        public DateTime DateTime { get; set; }
    }

}
