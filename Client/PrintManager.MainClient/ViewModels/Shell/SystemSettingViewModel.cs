using PrintManager.Shared.Utils;
using PrintManager.Shared;
using PrintManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintManager.MainClient.Components;

namespace PrintManager.MainClient.ViewModels.Shell
{
    public class SystemSettingViewModel : ViewModelBase
    {
        public SystemSettingViewModel()
        {

            ReadConfig();
        }

        #region 属性
        public string ConnectionString { get => _connectionString; set => Set(ref _connectionString, value); }
        private string _connectionString;
        public string LoadlIPAdr { get => _loadlIPAdr; set => Set(ref _loadlIPAdr, value); }
        private string _loadlIPAdr;
        public bool AutoConnectDB { get => _autoConnectDB; set => Set(ref _autoConnectDB, value); }
        private bool _autoConnectDB;
        public string ActualLanguage { get => _actualLanguage; set => Set(ref _actualLanguage, value); }
        private string _actualLanguage;
        public string ClientName { get => _clientName; set => Set(ref _clientName, value); }
        private string _clientName;

        public string OPCAdr { get => _oPCAdr; set => Set(ref _oPCAdr, value); }
        private string _oPCAdr;

        public bool AutoConnectClient { get => _autoConnectClient; set => Set(ref _autoConnectClient, value); }
        private bool _autoConnectClient;

        public string ServerAdr { get => _serverAdr; set => Set(ref _serverAdr, value); }
        private string _serverAdr;

        public string RefreshTime { get => _refreshTime; set => Set(ref _refreshTime, value); }
        private string _refreshTime;
        #endregion

        #region 变量
        #endregion

        #region 方法

        public bool StringToBool(string str)
        {
            bool temp = false;
            if (str == null) return false;

            if (str.Equals("TRUE"))
            {
                temp = true;
            }
            else
            {
                temp = false;
            }

            return temp;
        }
        public string BoolToString(bool b)
        {
            string str = string.Empty;
            if (b)
            {
                str = "TRUE";
            }
            else
            {
                str = "FALSE";
            }

            return str;
        }

        public void ReadConfig()
        {
            ConnectionString = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "ConnectionString");
            LoadlIPAdr = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "LocalIPAdr");
            //ActualLanguage = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "ActualLanguage");
            ClientName = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "ClientName");
            OPCAdr = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "OPCAdr");
            ServerAdr = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "ServerAdr");
            RefreshTime = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "RefreshTime");

            AutoConnectDB = StringToBool(IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "AutoConnectDB"));
            AutoConnectClient = StringToBool(IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "AutoConnectClient"));
        }
        private void WriteConfig()
        {
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "ConnectionString", ConnectionString);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "LocalIPAdr", LoadlIPAdr);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "AutoConnectDB", BoolToString(AutoConnectDB));
            //IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "ActualLanguage", ActualLanguage);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "ClientName", ClientName);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "OPCAdr", OPCAdr);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "AutoConnectClient", BoolToString( AutoConnectClient));
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "ServerAdr", ServerAdr);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "RefreshTime", RefreshTime);

        }
        #endregion

        #region 事件
        #endregion

        #region 命令

        public void onChangeLanguageToZh()
        {

        }

        public void onChangeLanguageToCn()
        {

        }

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
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "ConnectionString", "Server=localhost;Database=db_client;Trusted_Connection=True");
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "LocalIPAdr", "127.0.0.1:5500");
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "OPCAdr", "opc.tcp://192.168.0.1:4840");
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "ClientName", "PLC_1");
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "AutoConnectDB", "TRUE");
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "AutoConnectClient", "TRUE");
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "ServerAdr", "192.168.0.10:8080");
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "RefreshTime", "1000");
                //IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "ActualLanguage", "中文");

                ReadConfig();
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
            if (IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "ConnectionString") == ConnectionString &
                IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "LocalIPAdr") == LoadlIPAdr &
               StringToBool( IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "AutoConnectDB")) == AutoConnectDB &
                //IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "ActualLanguage") == ActualLanguage &
                IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "ClientName") == ClientName &
                IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "OPCAdr") == OPCAdr &
                StringToBool(IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "AutoConnectClient")) == AutoConnectClient &
                IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "ServerAdr") == ServerAdr &
                IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "RefreshTime") == RefreshTime
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
