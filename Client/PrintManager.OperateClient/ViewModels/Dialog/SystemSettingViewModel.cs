using PrintManager.Shared.Utils;
using PrintManager.Shared;
using PrintManager.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintManager.OperateClient.Components;

namespace PrintManager.OperateClient.ViewModels.Dialog
{
    public class SystemSettingViewModel : ViewModelBase
    {
        public SystemSettingViewModel()
        {
            ReadConfig();
            F89_PrintX = string.IsNullOrEmpty(x1)? 5 : Convert.ToInt32(x1);
            F89_PrintY = string.IsNullOrEmpty(y1)? 35 : Convert.ToInt32(y1);
            F89_Offset = string.IsNullOrEmpty(offset1)? 600 : Convert.ToInt32(offset1);

            RPX_PrintX = string.IsNullOrEmpty(x2) ? 5 : Convert.ToInt32(x2);
            RPX_PrintY = string.IsNullOrEmpty(y2) ? 35 : Convert.ToInt32(y2);
            RPX_Offset = string.IsNullOrEmpty(offset2) ? 420 : Convert.ToInt32(offset2);

            Bundle_PrintX = string.IsNullOrEmpty(x3) ? 5 : Convert.ToInt32(x3);
            Bundle_PrintY = string.IsNullOrEmpty(y3) ? 35 : Convert.ToInt32(y3);
            Bundle_Offset = string.IsNullOrEmpty(offset3) ? 240 : Convert.ToInt32(offset3);
        }

        #region 属性
        public int F89_PrintX { get => _F89_PrintX; set => Set(ref _F89_PrintX, value); }
        private int _F89_PrintX;
        public int F89_PrintY { get => _F89_PrintY; set => Set(ref _F89_PrintY, value); }
        private int _F89_PrintY;
        public int F89_Offset { get => _F89_Offset; set => Set(ref _F89_Offset, value); }
        private int _F89_Offset;

        public int RPX_PrintX { get => _RPX_PrintX; set => Set(ref _RPX_PrintX, value); }
        private int _RPX_PrintX;
        public int RPX_PrintY { get => _RPX_PrintY; set => Set(ref _RPX_PrintY, value); }
        private int _RPX_PrintY;
        public int RPX_Offset { get => _RPX_Offset; set => Set(ref _RPX_Offset, value); }
        private int _RPX_Offset;

        public int Bundle_PrintX { get => _Bundle_PrintX; set => Set(ref _Bundle_PrintX, value); }
        private int _Bundle_PrintX;
        public int Bundle_PrintY { get => _Bundle_PrintY; set => Set(ref _Bundle_PrintY, value); }
        private int _Bundle_PrintY;
        public int Bundle_Offset { get => _Bundle_Offset; set => Set(ref _Bundle_Offset, value); }
        private int _Bundle_Offset;
        #endregion

        #region 变量
        string x1, x2, x3;
        string y1, y2, y3;
        string offset1, offset2,offset3;
        #endregion

        #region 方法
        private void ReadConfig()
        {
            x1 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "F89", "PrintX");
            y1 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "F89", "PrintY");
            offset1 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "F89", "Offset");

            x2 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "RPX", "PrintX");
            y2 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "RPX", "PrintY");
            offset2 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "RPX", "Offset");

            x3 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Bundle", "PrintX");
            y3 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Bundle", "PrintY");
            offset3 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Bundle", "Offset");
        }
        private void WriteConfig()
        {
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "F89", "PrintX", F89_PrintX);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "F89", "PrintY", F89_PrintY);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "F89", "Offset", F89_Offset);

            IniUtil.IniWritevalue(Environments.ConfigFilePath, "RPX", "PrintX", RPX_PrintX);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "RPX", "PrintY", RPX_PrintY);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "RPX", "Offset", RPX_Offset);

            IniUtil.IniWritevalue(Environments.ConfigFilePath, "F89RPX", "PrintX", Bundle_PrintX);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "F89RPX", "PrintY", Bundle_PrintY);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "F89RPX", "Offset", Bundle_Offset);

            IniUtil.IniWritevalue(Environments.ConfigFilePath, "Bundle", "PrintX", Bundle_PrintX);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "Bundle", "PrintY", Bundle_PrintY);
            IniUtil.IniWritevalue(Environments.ConfigFilePath, "Bundle", "Offset", Bundle_Offset);
        }
        #endregion

        #region 事件
        #endregion

        #region 命令

        public void onSaveConfigCommand()
        {
            WriteConfig();
            ReadConfig();
            TryClose(true);
        }


        

        public void onCancelCommand()
        {
            TryClose(false);
        }

        #endregion

        #region 重写方法
        public override void CanClose(Action<bool> callback)
        {
            if (x1 == F89_PrintX.ToString() && y1 == F89_PrintY.ToString() && offset1== F89_Offset.ToString() &&
                x2 == RPX_PrintX.ToString() && y2 == RPX_PrintY.ToString() && offset2 == RPX_Offset.ToString() &&
                x3 == Bundle_PrintX.ToString() && y3 == Bundle_PrintY.ToString() && offset3 == Bundle_Offset.ToString())
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
