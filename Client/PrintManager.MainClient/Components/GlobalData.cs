using OpcUaHelper;
using Panuon.WPF;
using PrintManager.MainClient.Models;
using PrintManager.MainClient.ViewModels;
using PrintManager.MainClient.ViewModels.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PrintManager.MainClient.Components
{
    public class GlobalData : NotifyPropertyChangedBase
    {
        public Action LoginStatusChanged;
        public static GlobalData Instance { private set; get; } = new GlobalData();


        public UserModel User { get => _user; set => Set(ref _user, value); }
        private UserModel _user;

        public bool IsLogin { get => _isLogin; set { Set(ref _isLogin, value); LoginLogout(); } }
        private bool _isLogin;

        public List<string> MOList { get; set; } = new List<string>();
        private void LoginLogout()
        {
            LoginStatusChanged?.Invoke();
            if (!_isLogin)
            {
                User = null;
                foreach (Window w in Application.Current.Windows)
                {
                    if (w.DataContext is SystemSettingViewModel || w.DataContext is LabelEditViewModel)
                    {
                        w.Close();
                    }

                }
            }
        }

        public void AddToDayMOList(List<string> list)
        {
            MOList = MOList.Concat(list).ToList();
        }

        //OPC UA
        public static OpcUaClient m_OpcUaClient = new OpcUaClient();
        public static bool IsConnectClient;
        public static bool IsStartOPCSub;
        public static bool IsConnectDB;
        public static int TotalConnectClient;

        //List Item Options

        public bool IsAddItem { get => _isAddItem; set { Set(ref _isAddItem, value);  } }
        private bool _isAddItem;

        public bool IsUpdateItem { get => _isUpdateItem; set { Set(ref _isUpdateItem, value); } }
        private bool _isUpdateItem;

        //Language
        public static string Language;

    }
}
