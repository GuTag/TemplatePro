using Newtonsoft.Json;
using PrintManager.MainClient.Components;
using PrintManager.MainClient.Models;
using PrintManager.Sql.BLL;
using PrintManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.MainClient.ViewModels.Shell
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel()
        {
            if(GlobalData.Instance.User != null)
            {
                UserName = GlobalData.Instance.User.UserName;
                Password = GlobalData.Instance.User.Password;
            }
            
        }

        #region 属性

        public string UserName { get => _username; set => Set(ref _username, value); }
        private string _username;
        public string Password { get => _password; set => Set(ref _password, value); }
        private string _password;

        public string Hint { get => _hint; set => Set(ref _hint, value); }
        private string _hint;

        #endregion

        #region 事件与命令

        public void onLoginButton()
        {
            try
            {
                Hint = "";
                if (string.IsNullOrEmpty(UserName))
                {
                    Hint = "用户名不能为空";
                    return;
                }
                if (string.IsNullOrEmpty(Password))
                {
                    Hint = "密码不能为空";
                    return;
                }
                var data = UserBLL.Login(UserName, Password);
                if (data != null)
                {
                    GlobalData.Instance.User = JsonConvert.DeserializeObject<UserModel>(data);
                    GlobalData.Instance.IsLogin = true;
                    TryClose(true);
                }
                else
                {
                    Hint = "用户名或者密码不正确";
                }
            }
            catch (Exception ex)
            {
                Hint = ex.Message;
            }
            
        }
        public void onLogoutButton()
        {
            GlobalData.Instance.IsLogin = false;
            TryClose(false);
        }
        public void onCancelButton()
        {
            TryClose(false);
        }
        #endregion
    }
}
