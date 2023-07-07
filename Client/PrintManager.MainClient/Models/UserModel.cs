using Panuon.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.MainClient.Models
{
    public  class UserModel : NotifyPropertyChangedBase
    {
        public string UserName { get => _username; set => Set(ref _username, value); }
        private string _username;
        public string Password { get => _password; set => Set(ref _password, value); }
        private string _password;

        
    }
}
