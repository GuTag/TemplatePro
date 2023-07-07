using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace PrintManager.Shared.TCP
{
    public class TCPClient
    {
        #region 私有变量
        private TcpClient m_client;
        private TouchSocketConfig m_config;
        private string m_IP = "127.0.0.1:5500";
        private bool m_connecting;
        private bool m_disconnect;
        private bool m_reconnect;
        private int reconnect_interval;
        private DispatcherTimer reconnect_timer;
        //private Task m_taskConnect;
        #endregion

        #region 公有变量
        public EventHandler<TCPEventArgs> UpdateStatusEvent;
        public Action<TCPRecviceMsg> RecviceMsg;
        #endregion


        #region 单列模式

        private static TCPClient instance = null;
        private static readonly object padlock = new object();
        public static TCPClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TCPClient();
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new TCPClient();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        private TCPClient()
        {
            m_client = new TcpClient();
            m_client.Connected += Client_Connected;
            m_client.Disconnected += Client_Disconnected;
            m_client.Received += Client_Received;

            

            reconnect_timer = new DispatcherTimer();
            
            reconnect_timer.Tick += ReConnect_Event;
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
                TcpRole = TCPRole.Client,
                IP = $"{m_client.IP}:{m_client.Port}",
                Running = m_client.Online,
                Title = title,
                Message = String.Join(",", msgs),
            };
            UpdateStatusEvent?.Invoke(this, e);
            //DebugUtil.WriteLine($"{title}:{String.Join(",", msgs)}");

        }

        /// <summary>
        /// 启动连接服务器
        /// </summary>
        public void Connect(string ip= null, int interval = 10)
        {
            if (!string.IsNullOrEmpty(ip))
            {
                m_IP = ip;
            }
            m_config = new TouchSocketConfig()
                .SetRemoteIPHost(new IPHost(m_IP))
                .UsePlugin()
                .SetDataHandlingAdapter(() => { return new CustomDataHandlingAdapter(); })
                .SetBufferLength(1024 * 10);

            if (m_client.Online)
            {
                UpdateStatus($"与服务器连接:已经连接", "请勿重复启动"); return;
            }

            if (m_connecting)
            {
                UpdateStatus($"与服务器连接:连接中", "请勿重复启动"); return;
            }
            m_connecting = true;
            this.reconnect_interval = interval;
            if(interval > 0)
            {
                m_reconnect = true;
                reconnect_timer.Interval = TimeSpan.FromSeconds(reconnect_interval);
            }

            //var ip = IniUtil.IniReadvalue(PathUtil.ConfigPath, "General", "IP");
            //var port = IniUtil.IniReadvalue(PathUtil.ConfigPath, "General", "Port");
            //SetIP(ip, int.Parse(port));
            UpdateStatus($"与服务器{m_IP}连接:连接中");
            m_client.Setup(m_config).ConnectAsync().ContinueWith(TaskComplated);

            //if (m_taskConnect != null && m_taskConnect.Status == TaskStatus.RanToCompletion)
            //{
            //    UpdateStatus($"与服务器连接:连接中，请勿重复启动");
            //    return;
            //}
            //m_taskConnect = Task.Run(ConnectAsync).ContinueWith(TaskComplated);
            //UpdateStatus($"与服务器连接:连接中");

        }

        public void DisConnect()
        {
            if (m_connecting)
            {
                m_disconnect = true;
                UpdateStatus($"连接动作完成后关闭");
            }

            if(m_client.Online)
            {
                m_client.Close();
                UpdateStatus("已经断开链接");
            }
        }

        /// <summary>
        /// 设置服务器IP地址与端口
        /// </summary>
        /// <param name="IP"></param>
        public void SetIP(string IP, int Port)
        {
            if (!TouchSocketUtility.IsIPv4(IP))
            {
                UpdateStatus("IP地址格式不正确");
                return;
            }

            if (m_client.Online)
            {
                UpdateStatus("客户端已经启动，请先停止");
                return;
            }

            m_IP = $"{IP}:{Port}";
            m_config.SetRemoteIPHost(new IPHost(m_IP));
        }

        /// <summary>
        /// 客户端发送数据
        /// </summary>
        /// <param name="requestInfo"></param>
        public void Send(RequestInfo requestInfo)
        {
            if (m_client.CanSend)
            {
                m_client.Send(requestInfo.BuildAsBytes());
            }
            else
            {
                UpdateStatus("没有连接服务器，不能发送"); return;
            }
        }

        //异步连接服务器
        private void ConnectAsync()
        {
            
        }

        //连接任务完成时动作
        private void TaskComplated(Task t)
        {
            m_connecting = false;
            if (!m_client.Online)
            {
                UpdateStatus($"与服务器{m_IP}连接:连接超时，请检查服务器是否启动");
                if (m_reconnect)
                {
                    reconnect_timer.Start();
                }
            }
            if(m_client.Online && m_disconnect)
            {
                DisConnect();
            }
            else if (m_client.Online)
            {
                reconnect_timer.Stop();
            }
            
            m_disconnect = false;
        }

        //客户端接受数据
        private void Client_Received(TcpClient client, ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            //从服务器接受信息
            if (requestInfo is RequestInfo)//如果使用了适配器
            {

                var request = requestInfo as RequestInfo;
                var e = new TCPRecviceMsg()
                {
                    IP = $"{client.IP}:{client.Port}",
                    Message = request,
                    DateTime = DateTime.Now,
                };
                RecviceMsg?.Invoke(e);
            }
            else
            {
                string messsage = Encoding.UTF8.GetString(byteBlock.Buffer, 0, byteBlock.Len);
                UpdateStatus("接收客户端数据", $"没有使用适配器,{messsage}");
            }
        }

        //与服务器断开连接
        private void Client_Disconnected(ITcpClientBase client, ClientDisconnectedEventArgs e)
        {
            UpdateStatus($"与服务器断{m_IP}开连接:{e.Manual},{e.Message}");
            if (m_reconnect)
            {
                reconnect_timer.Start();
            }
        }

        //服务连接完成
        private void Client_Connected(ITcpClient client, MsgEventArgs e)
        {
            UpdateStatus($"服务器{m_IP}连接：{e.Message}");
        }

        //重新连接事件
        private void ReConnect_Event(object sender, EventArgs e)
        {
            this.Connect("", reconnect_interval);
            UpdateStatus($"正在重新连接服务器{m_IP}");
        }

    }
}
