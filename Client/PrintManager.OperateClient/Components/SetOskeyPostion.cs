using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PrintManager.OperateClient.Components
{
    //public class SetOskeyPostion
    //{
    //    // 申明要使用的dll和api
    //    [DllImport("User32.dll", EntryPoint = "FindWindow")]
    //    public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
    //    [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "MoveWindow")]
    //    public static extern bool MoveWindow(System.IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);


    //    [DllImport("user32.dll")]
    //    static extern bool SetForegroundWindow(IntPtr hWnd);


    //    public static void ShowKeyBord()
    //    {
    //        //打开软键盘
    //        try
    //        {
    //            if (!System.IO.File.Exists(Environment.SystemDirectory + "\\osk.exe"))
    //            {
    //                MessageBox.Show("软件盘可执行文件不存在！");
    //                return;
    //            }


    //            var softKey = System.Diagnostics.Process.Start("C:\\Windows\\System32\\osk.exe");
    //            // 上面的语句在打开软键盘后，系统还没用立刻把软键盘的窗口创建出来了。所以下面的代码用循环来查询窗口是否创建，只有创建了窗口
    //            // FindWindow才能找到窗口句柄，才可以移动窗口的位置和设置窗口的大小。这里是关键。
    //            IntPtr intptr = IntPtr.Zero;
    //            while (IntPtr.Zero == intptr)
    //            {
    //                System.Threading.Thread.Sleep(100);
    //                intptr = FindWindow(null, "屏幕键盘");
    //            }


    //            // 获取屏幕尺寸
    //            var iActulaWidth = SystemParameters.PrimaryScreenWidth;
    //            var iActulaHeight = SystemParameters.PrimaryScreenHeight;


    //            // 设置软键盘的显示位置，底部居中
    //            var posX = (iActulaWidth - 1000) / 2;
    //            var posY = (iActulaHeight - 300);


    //            //设定键盘显示位置
    //            MoveWindow(intptr, (int)posX, (int)posY, 1000, 300, true);


    //            //设置软键盘到前端显示
    //            SetForegroundWindow(intptr);
    //        }
    //        catch (System.Exception ex)
    //        {
    //            MessageBox.Show(ex.Message);
    //        }
    //    }
    //}
}
