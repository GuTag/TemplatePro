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


    }
}
