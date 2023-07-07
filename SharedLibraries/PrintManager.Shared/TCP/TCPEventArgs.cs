using PrintManager.Shared.Attributes;
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
    public class TCPEventArgs
    {
        /// <summary>
        /// 类型
        /// </summary>
        public TCPRole TcpRole { get; set; }

        /// <summary>
        /// IPHost
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Running
        /// </summary>
        public bool Running { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 文本
        /// </summary>
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{RemarkExtension.GetRemark(TcpRole)} {IP} {Title}:{Message}";
        }
    }

    public enum TCPRole
    {
        [Remark("服务器")]
        Server,
        [Remark("客户端")]
        Client
    }
}
