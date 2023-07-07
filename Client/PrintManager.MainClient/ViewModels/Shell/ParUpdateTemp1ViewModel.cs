using PrintManager.MainClient.Components;
using PrintManager.Shared.Utils;
using PrintManager.Shared;
using PrintManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.MainClient.ViewModels.Shell
{
    public class ParUpdateTemp1ViewModel : ViewModelBase
    {
        public ParUpdateTemp1ViewModel()
        {
       
        }

        #region 属性
        public string ConfigSQLStr { get => _configSQLStr; set => Set(ref _configSQLStr, value); }
        private string _configSQLStr;
        public string ConfigIPAdr { get => _configIPAdr; set => Set(ref _configIPAdr, value); }
        private string _configIPAdr;
        public string ConfigClent1 { get => _configClent1; set => Set(ref _configClent1, value); }
        private string _configClent1;
        public string ConfigClent2 { get => _configClent2; set => Set(ref _configClent2, value); }
        private string _configClent2;
        public string ConfigClent3 { get => _configClent3; set => Set(ref _configClent3, value); }
        private string _configClent3;

        public string SerialPort { get => _serialPort; set => Set(ref _serialPort, value); }
        private string _serialPort;

        public string ChartReferTime { get => _chartReferTime; set => Set(ref _chartReferTime, value); }
        private string _chartReferTime;

        public string ChartDataRange { get => _chartDataRange; set => Set(ref _chartDataRange, value); }
        private string _chartDataRange;

        public string ChartExcelSavePath { get => _chartExcelSavePath; set => Set(ref _chartExcelSavePath, value); }
        private string _chartExcelSavePath;
        #endregion

        #region 变量
        #endregion

        #region 方法
        private void WriteConfig()
        {
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "ConnectionString", ConfigSQLStr);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "IP", ConfigIPAdr);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "SerialPort", SerialPort);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "ClientIP", "1", ConfigClent1);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "ClientIP", "2", ConfigClent2);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "ClientIP", "3", ConfigClent3);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "Chart", "ChartReferTime", ChartReferTime);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "Chart", "ChartDataRange", ChartDataRange);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "Chart", "ChartExcelSavePath", ChartExcelSavePath);
        }
        #endregion

        #region 事件
        #endregion

        #region 命令

        public void onSaveConfigCommand()
        {
            WriteConfig();
            TryClose(true);
        }


        public void onInitConfigCommand()
        {
            if (WindowManagerExtension.ShowAckDialog(WindowManager, "恢复默认参数确认", "是否恢复默认参数配置") == true)
            {
                //default paras
                //ConfigSQLStr = "Server=localhost;Database=db_printmanager;Trusted_Connection=True";
                //ConfigIPAdr = "0.0.0.0:5500";
                //ConfigClent1 = "192.168.0.101";
                //ConfigClent2 = "192.168.0.102";
                //ConfigClent3 = "192.168.0.103";
                //SerialPort = "COM3";
                //ChartReferTime = "0";
                //ChartDataRange = "0";
                //ChartExcelSavePath = "C:\\";
                //WriteConfig();
            }
        }

        public void onCancelCommand()
        {
            TryClose(false);
        }

        #endregion

        #region 重写方法
        public override void CanClose(Action<bool> callback)
        {
            if (IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "ConnectionString") == ConfigSQLStr &
                IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "IP") == ConfigIPAdr &
                IniUtil.IniReadvalue(Environments.ConfigFilePath, "ClientIP", "1") == ConfigClent1 &
                IniUtil.IniReadvalue(Environments.ConfigFilePath, "ClientIP", "2") == ConfigClent2 &
                IniUtil.IniReadvalue(Environments.ConfigFilePath, "ClientIP", "3") == ConfigClent3 &
                IniUtil.IniReadvalue(Environments.ConfigFilePath, "Chart", "ChartReferTime") == ChartReferTime &
                IniUtil.IniReadvalue(Environments.ConfigFilePath, "Chart", "ChartDataRange") == ChartDataRange &
                IniUtil.IniReadvalue(Environments.ConfigFilePath, "Chart", "ChartExcelSavePath") == ChartExcelSavePath
                )
            {

                base.CanClose(callback);

            }
            else
            {
                if (WindowManagerExtension.ShowAckDialog(WindowManager, "关闭窗口", "是否保存配置?") == true)
                {
                    try
                    {
                        WriteConfig();
                        base.CanClose(callback);
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    base.CanClose(callback);
                }
            }
        }
        #endregion
    }
}