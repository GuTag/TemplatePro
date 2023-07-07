using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TouchSocket.Sockets;

namespace PrintManager.Shared.SerialPorts
{
    public class RS485
    {
        #region 公共变量
        public EventHandler<bool[]> ReiceveData;
        public EventHandler<string> UpdateStatusEvent;
        public SerialPort comport;//串口
        public bool IsConnect = false;
        #endregion

        #region 私有变量
        //private Timer TimerConnect;//定时检查连接
        #endregion

        public RS485(string portName, int baudRate, int dataBits)
        {
            var parity = Parity.None;
            var stopBits = StopBits.One;

            comport = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            comport.WriteTimeout = 100;
            //收到返回数据的事件处理绑定函数
            comport.DataReceived += SerialPort_DataReceived;
            comport.ErrorReceived += SerialPort_ErrorReceived;

            //TimerConnect.Enabled = true;
            //TimerConnect.Interval = 1000;//1s
            //TimerConnect.AutoReset = true;
            //TimerConnect.Elapsed += new ElapsedEventHandler(ConnectCheck);
            //TimerConnect.Start();
        }


        #region 事件
        private void ConnectCheck(object sender, ElapsedEventArgs e)
        {
            var data = "";
            comport.Write(data);
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //16进制发送数据还原
            int i_InNum;//输入缓冲区中字节数
            i_InNum = comport.BytesToRead;
            if(i_InNum >= 15)
            {
                Byte[] ReceivedData = new Byte[i_InNum];//创建接收字节数组
                comport.Read(ReceivedData, 0, i_InNum);//读取接收的数据
                bool[] inputBools = new bool[8];

                for (int i = 0; i < inputBools.Length; i++)
                {
                    inputBools[i] = ReceivedData[i + 4] > 0;
                }
                comport.DiscardInBuffer();//丢弃接收缓冲区数据
                ReiceveData?.Invoke(this, inputBools);
            }
        }


        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            UpdateStatus(e.ToString());
        }
        #endregion

        #region 方法
        private void UpdateStatus(string message)
        {
            UpdateStatusEvent?.Invoke(this, message);
        }
        public bool SendData(string data)
        {
            if (!comport.IsOpen)
            {
                return false;
            }
            comport.Write(data);
            return true;
        }

        //打开串口
        public bool Open()
        {
            try
            {
                if (comport.IsOpen)
                {
                    Close();
                }

                comport.Open();

                if (comport.IsOpen)
                {
                    return true;
                }
                else
                {
                    UpdateStatus("端口打开失败");
                    return false;
                }
            }
            catch (Exception e)
            {
                UpdateStatus($"端口打开失败:{e.Message}");
            }
            return false;
        }

        public void Close()
        {
            comport.Close();
        }
        #endregion

        #region 属性
        //串口状态
        public bool IsOpen
        {
            get
            {
                return comport.IsOpen;
            }
        }
        #endregion

    }
}
