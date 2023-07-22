using Caliburn.Micro;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PrintManager.MainClient.Components;
using PrintManager.MainClient.Conntrols;
using PrintManager.MainClient.Models;
using PrintManager.MainClient.Models.Extension;
using PrintManager.MainClient.ViewModels.Controls;
using PrintManager.Shared;
using PrintManager.Shared.Enums;
using PrintManager.Shared.Utils;
using PrintManager.Sql;
using PrintManager.Sql.BLL;
using PrintManager.Sql.Models;
using PrintManager.UI;
using PrintManager.UI.Controls;
using PrintManager.UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Dynamic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace PrintManager.MainClient.ViewModels.Shell
{
    public class SystemLanguageViewModel : ViewModelBase
    {
        public SystemLanguageViewModel()
        {
        }



        #region 属性


        #endregion

        #region 变量


        #endregion

        #region 方法

        #endregion

        #region 事件

        public void onChangeLanguageCommand(string language)
        {
            if (WindowManagerExtension.ShowAckDialog(WindowManager, "语言切换确认", "语言切换确认，加载语言前系统将重新启动！") == true)
            {
                IniUtil.IniWritevalue(Environments.ConfigFilePath, "Config", "Language", language);

                //软件重启
                Application.Current.Shutdown();
                System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }             
        }

        #endregion

        #region 命令

        #endregion
    }
}
