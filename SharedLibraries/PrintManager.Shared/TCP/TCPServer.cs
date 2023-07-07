using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace PrintManager.Shared.TCP
{
    public class TCPServer
    {
        #region 私有变量
        private TcpService m_service;
        private TouchSocketConfig m_config;
        //private bool m_running;//服务器已经启动
        public string m_IP = "127.0.0.1:5500";
        #endregion

        #region 公有变量
        public EventHandler<TCPEventArgs> UpdateStatusEvent;
        public Action<TCPRecviceMsg> RecviceMsg;
        public bool IsRunning => m_service.ServerState == ServerState.Running;

        public Dictionary<string, string> ClientDictionary = new Dictionary<string, string>();
        #endregion


        #region 单列模式

        private static TCPServer instance = null;
        private static readonly object padlock = new object();
        public static TCPServer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TCPServer();
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new TCPServer();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        private TCPServer()
        {
            m_service = new TcpService();
            m_service.Connecting += Server_Connecting;
            m_service.Connected += Server_Connected;
            m_service.Disconnected += Server_Disconnected;
            m_service.Received += Server_Received;



           
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="status"></param>
        /// <param name="msg"></param>
        private void UpdateStatus(string title, params string[] msgs)
        {
            var e = new TCPEventArgs()
            {
                TcpRole = TCPRole.Server,
                IP = m_IP,
                Running = IsRunning,
                Title = title,
                Message = String.Join(",", msgs),
            };
            UpdateStatusEvent?.Invoke(this, e);
        }

        /// <summary>
        /// 服务器启动
        /// </summary>
        public void Start()
        {
            try
            {
                m_config = new TouchSocketConfig()
               .SetListenIPHosts(new IPHost[] { new IPHost(m_IP) })
               .SetMaxCount(10000)
               .SetDataHandlingAdapter(() => { return new CustomDataHandlingAdapter(); })
               .SetThreadCount(10)
               .SetKeepAliveValue(new KeepAliveValue())
               .ConfigurePlugins(a =>
               {
                   //a.Add(new CheckClearPlugin() { Duration = new TimeSpan(1200)}); //添加插件
               })
               .ConfigureContainer(a =>
               {
                   //a.SetSingletonLogger<ConsoleLogger>(); //添加一个日志注入
               });

                if (m_service.ServerState == ServerState.Running)
                {
                    UpdateStatus("服务器启动", "已经启动");
                    return;
                }
                //var ip = IniUtil.IniReadvalue(PathUtil.ConfigPath, "General", "IP");
                //var port = IniUtil.IniReadvalue(PathUtil.ConfigPath, "General", "Port");
                //SetIP(ip, int.Parse(port));
                m_service.Setup(m_config).Start();
                UpdateStatus("服务器启动", "成功");
            }
            catch (Exception ex)
            {
                var ips = m_config.GetValue(TouchSocketConfigExtension.ListenIPHostsProperty);
                UpdateStatus("服务器启动", ex.Message);
            }
            
        }

        /// <summary>
        /// 服务器停止
        /// </summary>
        public void Stop()
        {
            m_service.Stop();
            UpdateStatus("服务器停止");
        }

        /// <summary>
        /// 设置服务器启动IP和端口
        /// </summary>
        /// <param name="IP"></param>
        public void SetIP(string IP, int Port)
        {
            if (!TouchSocketUtility.IsIPv4(IP))
            {
                UpdateStatus("IP地址格式不正确");
                return;
            }

            if (m_service.ServerState == ServerState.Running)
            {
                UpdateStatus("服务器已经启动，请先停止");
                return ;
            }
            
            m_IP = $"{IP}:{Port}";
            m_config.SetListenIPHosts(new IPHost[] { new IPHost(m_IP) });
        }

        public void SetIP(string IP)
        {
            if (!string.IsNullOrEmpty(IP))
            {
                m_IP = IP;
            }
        }

        public void SetIPS(IPHost[] ips)
        {
            m_config.SetListenIPHosts(ips);
        }

        /// <summary>
        /// 获取所有客户端实例
        /// </summary>
        /// <returns>Clients SocketClient[]</returns>
        public SocketClient[] GetClients()
        {
            if (!ServerIsRunning()) return null;
            return m_service.GetClients();
        }

        /// <summary>
        /// 获取所有客户端IP
        /// </summary>
        /// <returns>IPS string[]</returns>
        public string[] GetClientIPs()
        {
            if (!ServerIsRunning()) return null;
            return m_service.GetClients().Select(c=>c.IP + ":" + c.Port).ToArray();
        }

        /// <summary>
        /// 获取所有客户端ID
        /// </summary>
        /// <returns>IPS string[]</returns>
        public string[] GetClientIDs()
        {
            if (!ServerIsRunning()) return null;
            return m_service.GetClients().Select(c => c.ID).ToArray();
        }

        /// <summary>
        /// 获取客户端ID
        /// </summary>
        /// <returns>IPS string[]</returns>
        public string GetClientID(string IPHost)
        {
            var iphost = IPHost.Split(':');
            if (iphost.Length == 2)
            {
                return GetClientID(iphost[0], iphost[1]);
            }
            return null;
        }

        /// <summary>
        /// 获取客户端ID
        /// </summary>
        /// <returns>IPS string[]</returns>
        public string GetClientID(string IP, string Port)
        {
            if (!ServerIsRunning()) return null;
            foreach (var client in m_service.GetClients())
            {
                if(client.IP == IP && client.Port.ToString() == Port)
                {
                    return client.ID;
                }
            }
            return null;
        }

        /// <summary>
        /// 发送数据给多个客户端
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <param name="IDs"></param>
        public void SendClients(RequestInfo requestInfo, string[] clientIDs)
        {
            if (!ServerIsRunning()) return;
            foreach (var clientID in clientIDs)
            {
                m_service.Send(clientID, requestInfo.BuildAsBytes());
            }
        }

        /// <summary>
        /// 发送数据给单个客户端
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <param name="ID"></param>
        public void SendClient(RequestInfo requestInfo, string clientID)
        {
            if(!ServerIsRunning())return;
            m_service.Send(clientID, requestInfo.BuildAsBytes());
        }

        //判断服务是否启动
        public bool ServerIsRunning()
        {
            if (IsRunning) return true;
            UpdateStatus("服务没有启动");
            return false;
        }
        //接受客户端数据
        private void Server_Received(SocketClient client, ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            //从客户端接受信息
            if (requestInfo is RequestInfo)//如果使用了适配器
            {
                var request = requestInfo as RequestInfo;
                RecviceMsg?.Invoke(new TCPRecviceMsg()
                {
                    IP = $"{client.IP}:{client.Port}",
                    Message = request
                }) ;
            }
            else
            {
                string messsage = Encoding.UTF8.GetString(byteBlock.Buffer, 0, byteBlock.Len);
                UpdateStatus("接收客户端数据", "没有使用适配器", $"客户端{client.IP}:{client.Port}：{messsage}");
            }
        }
        //客户端断开连接
        private void Server_Disconnected(SocketClient client, ClientDisconnectedEventArgs e)
        {
            UpdateStatus("断开连接", $"客户端断开连接：{client.IP}:{client.Port}", e.Message);
            List<string> list = new List<string>();
            foreach (var key in ClientDictionary.Keys)
            {
                if (key.Contains(client.IP))
                {
                    list.Add(key);
                }
            }
            foreach (var item in list)
            {
                ClientDictionary.Remove(item);
            }
        }
        //客户端连接完成
        private void Server_Connected(SocketClient client, TouchSocketEventArgs e)
        {
            UpdateStatus("连接完成", $"客户端连接：{client.IP}:{client.Port}");
            ClientDictionary.Add($"{client.IP}:{client.Port}", "");
        }
        //客户端正在连接
        private void Server_Connecting(SocketClient client, ClientOperationEventArgs e)
        {
            UpdateStatus("正在连接", $"客户端连接：{client.IP}:{client.Port}");
        }

        
    }
}
