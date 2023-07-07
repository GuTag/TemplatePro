using Caliburn.Micro;
using Newtonsoft.Json;
using Panuon.WPF;
using Panuon.WPF.UI;
using PrintManager.OperateClient.Components;
using PrintManager.OperateClient.Models;
using PrintManager.OperateClient.ViewModels.Dialog;
using PrintManager.Shared;
using PrintManager.Shared.Enums;
using PrintManager.Shared.Helpers;
using PrintManager.Shared.TCP;
using PrintManager.Shared.Utils;
using PrintManager.UI;
using PrintManager.UI.Controls;
using PrintManager.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static PrintManager.Shared.Entity.OrderEntity;

namespace PrintManager.OperateClient.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        #region 变量
        public ManualViewModel ManualViewModel { get; }
        private System.Timers.Timer RequestCurrentTimer = new System.Timers.Timer();
        //public LabelPrinter Printer { get; }

        #endregion
        public ShellViewModel()
        {
            TCPClient.Instance.UpdateStatusEvent += TCPClient_UpdateStatusEvent;
            TCPClient.Instance.RecviceMsg += TCPClient_RecviceMsg;
            TCPClient.Instance.Connect(IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "IP"));
            ManualViewModel = new ManualViewModel();

            //刷新当前时间
            RequestCurrentTimer.Enabled = true;
            RequestCurrentTimer.Interval = 1000;//1s
            RequestCurrentTimer.AutoReset = true;
            RequestCurrentTimer.Start();
            RequestCurrentTimer.Elapsed += RequestCurrentTimer_Elapsed;
        }


        #region 属性
        public bool IsConnecting { get => _isConnecting; set => Set(ref _isConnecting, value); }
        private bool _isConnecting;

        public bool IsManual { get => _isManual; set
            {
                Set(ref _isManual, value);
                if (value && PrintData != null)
                {
                    IsEnblePrint = true;
                }
            }
        }
        private bool _isManual = false;

        public HintModel Hint { get => _hint; set => Set(ref _hint, value); }
        private HintModel _hint = new HintModel();

        //ScanMO
        public string ScanMO { get => _scanMO; set => Set(ref _scanMO, value); }
        private string _scanMO;



        public string CurrentScanMO { get => _currentScanMO; set => Set(ref _currentScanMO, value); }
        private string _currentScanMO;
        public PrintModel PrintData { get => _printData;
            set
            {
                Set(ref _printData, value);
                if(value != null)
                {
                    IsEnblePrint = value.RequestNum > value.ComplatedNum;
                }
                else
                {
                    IsEnblePrint = false;
                }
            }
        }
        private PrintModel _printData = new PrintModel();

        public PrintCanvas PrintPreviewCanvas1 { get => _printPreviewCanvas1; set => Set(ref _printPreviewCanvas1, value); }
        private PrintCanvas _printPreviewCanvas1;

        public PrintCanvas PrintPreviewCanvas2 { get => _printPreviewCanvas2; set => Set(ref _printPreviewCanvas2, value); }
        private PrintCanvas _printPreviewCanvas2;

        public bool IsPrintOK1 { get => _isPrintOK1; set => Set(ref _isPrintOK1, value); }
        private bool _isPrintOK1;

        public bool IsPrintOK2 { get => _isPrintOK2; set => Set(ref _isPrintOK2, value); }
        private bool _isPrintOK2;

        public bool IsEnblePrint { get => _isEnblePrint; set => Set(ref _isEnblePrint, value); }
        private bool _isEnblePrint;

        public string CurrentTime { get => _currentTime; set => Set(ref _currentTime, value); }
        private string _currentTime;

        #endregion

        #region 变量

        #endregion

        #region 方法
        /// <summary>
        /// 打印标签
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns></returns>
        private bool PrinterLabel(PrintCanvas canvas)
        {
            LabelPrinter printer = new LabelPrinter();
            string[] devices = null;
            try
            {
                Hint.Clean();
                int language = 6;//BLA
                int portType = 3;//USB
                
                string portInfo = IniUtil.IniReadvalue(Environments.ConfigFilePath, canvas.PrintData.PrintTemplate.Type, "USB");
                devices = printer.DiscoverPrinter(portType);
                if (devices == null || devices.Length == 0 || !devices.Contains(portInfo))
                {
                    Hint.Update(LogType.Error,"打印机连接错误：无法找到设备");
                    return false;
                }
                int r = printer.ConnectPrinter(portType, portInfo, language);
                if (r != 0)
                {
                    Hint.Update(LogType.Error, "打印机连接错误：错误代码" + r);
                    return false;
                }
               
                int x = 5, y = 25;
                var width = PixelUtil.mmToPixel_Print(canvas.PrintData.PrintTemplate.Width);
                var height = PixelUtil.mmToPixel_Print(canvas.PrintData.PrintTemplate.Height);


                var offsetx = IniUtil.IniReadvalue(Environments.ConfigFilePath, canvas.PrintData.PrintTemplate.Type, "PrintX");
                var offsety = IniUtil.IniReadvalue(Environments.ConfigFilePath, canvas.PrintData.PrintTemplate.Type, "PrintY");
                if (!string.IsNullOrEmpty(offsetx))
                {
                    x = Convert.ToInt32(offsetx);
                }
                if (!string.IsNullOrEmpty(offsety))
                {
                    y = Convert.ToInt32(offsety);
                }
                int offset = height;//出纸距离
                var offset_string = IniUtil.IniReadvalue(Environments.ConfigFilePath, canvas.PrintData.PrintTemplate.Type, "Offset");
                if (!string.IsNullOrEmpty(offset_string))
                {
                    offset = Convert.ToInt32(offset_string);
                }
                offset += y;
                printer.SetPrintMode(3, offset);
                
                r = printer.SetLabelSize(width, height + y);
                if (r != 0)
                {
                    Hint.Update(LogType.Error, "打印机设置打印纸张错误：错误代码" + r);
                    return false;
                }
                //生成canvas image，打印完成后删除
                //var iamgepath = Path.Combine(Environments.PrintFolderPath, "print.png");
                var targetpath = Path.Combine(Environments.PrintFolderPath, PrintData.MO + ".png");

                using (Stream stream = File.Create(targetpath))
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(CanvasHelpers.ConvertToBitmapSource(canvas)));
                    encoder.Save(stream);
                }

                printer.PrintImageFile(x, y, targetpath);


                r = printer.PrintLabel(1, 1);
                if (r != 0)
                {
                    Hint.Update(LogType.Error, "打印错误：错误代码" + r);
                    return false;
                }
                //生成canvas image，打印完成后删除
                File.Delete(targetpath);

                Hint.Update(LogType.OK, $"打印完成:{PrintData.MO}");
                return true;
            }
            catch (Exception e)
            {
                Hint.Update(LogType.Error, $"打印异常:{e.Message}");
                return false;
            }
            finally
            {
                if(devices != null)
                {
                    printer.DisconnectPrinter();
                }
            }
            

        }

        /// <summary>
        /// 打印动作
        /// </summary>
        private void PrintAction()
        {
            //PrintData.ComplatedNum += 1;
            ////发送数据
            //Communication.TcpSendData_Print(PrintData);
            //Clean();

            if (PrintData == null || !IsEnblePrint || !IsConnecting) return;
            dynamic settings = new ExpandoObject();
            settings.ResizeMode = ResizeMode.NoResize;
            settings.WindowStyle = WindowStyle.None;
            settings.AllowsTransparency = true;
            settings.ShowInTaskBar = false;
            settings.Height = 200;
            settings.Width = 800;
            settings.SizeToContent = SizeToContent.Manual;
            settings.Background = new SolidColorBrush(Colors.Transparent);
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            WindowManager.ShowDialog(new PrintAckViewModel(PrintData.Desc), null, settings);
            //执行打印
            if (!IsPrintOK1 && PrintPreviewCanvas1 != null)
            {
                IsPrintOK1 = PrinterLabel(PrintPreviewCanvas1);
            }

            if (!IsPrintOK2 && PrintPreviewCanvas2 != null)
            {
                IsPrintOK2 = PrinterLabel(PrintPreviewCanvas2);
            }
            //打印成功则+1
            if (IsPrintOK1 && IsPrintOK2)
            {
                if (!IsManual)
                {
                    PrintData.ComplatedNum += 1;
                    //发送数据
                    Communication.TcpSendData_Print(PrintData);
                    //Clean();
                }
            }
            else
            {
                //打印错误上报
                IsEnblePrint = true;
            }

        }

        /// <summary>
        /// 发送MO码
        /// </summary>
        /// <param name="mo"></param>
        private void SendMO(string mo)
        {
            //没有打印成功，则不能打印下一个
            if(PrintData != null)
            {
                if(!IsPrintOK1 || !IsPrintOK2)
                {
                    return;
                }
            }
            Communication.TcpSendData_MO(mo);
            Clean();
        }

        /// <summary>
        /// 清除客户端数据
        /// </summary>
        private void Clean()
        {
            ManualViewModel.MO = "";
            PrintData = null;
            IsEnblePrint = false;
            PrintPreviewCanvas1 = null;
            PrintPreviewCanvas2 = null;
            IsPrintOK1 = false;
            IsPrintOK2 = false;
            Hint.Clean();
        }
        #endregion

        #region 事件
        /// <summary>
        /// 显示当前时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RequestCurrentTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void TCPClient_RecviceMsg(TCPRecviceMsg e)
        {
            if (e.Message.ID == "MO")
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<Dictionary<object, object>>(e.Message.Data);
                    if ("OK".Equals(data.GetValueOrDefault("Status").ToString()))
                    {
                        var Order = JsonConvert.DeserializeObject <PrintModel>(data.GetValueOrDefault("Order").ToString());
                        var PrintDatas = JsonConvert.DeserializeObject<List<CanvasData>>(data.GetValueOrDefault("PrintDatas").ToString());

                        Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
                        {
                            Clean();

                            PrintData = Order;
                            if (PrintDatas != null && PrintDatas.Count == 2)
                            {
                                PrintPreviewCanvas1 = new PrintCanvas(PrintDatas[0], 1.5);
                                PrintPreviewCanvas2 = new PrintCanvas(PrintDatas[1], 1.5);
                            }
                            else
                            {
                                Hint.Update(LogType.Error, $"{Order.MO}:服务端模板数量不正确");
                            }

                            if (Order.IsOK && PrintPreviewCanvas1 != null)
                            {
                                PrintAction();
                            }
                            else
                            {
                                Hint.Update(LogType.Warning, $"{Order.MO}:等待该产品检测合格,请合格后打印");
                            }
                        }));
                    }
                    else
                    {
                        Hint.Update(LogType.Warning, $"{ScanMO}:MO码不正确,无法查询该MO码产品");
                    }
                }
                catch (Exception ex)
                {
                    Hint.Update(LogType.Error, $"数据接受异常：{ex}");
                }
            }else if (e.Message.ID == "TEST")
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<Dictionary<object, object>>(e.Message.Data);
                    if ("OK".Equals(data.GetValueOrDefault("Status").ToString()))
                    {
                        var PrintDatas = JsonConvert.DeserializeObject<List<CanvasData>>(data.GetValueOrDefault("PrintDatas").ToString());

                        Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
                        {
                            Hint.Clean();
                            PrintData = new PrintModel();
                            if (PrintDatas != null && PrintDatas.Count == 1)
                            {
                                PrintPreviewCanvas1 = new PrintCanvas(PrintDatas[0], 1.5);
                                IsEnblePrint = true;
                            }
                            else if (PrintDatas != null && PrintDatas.Count == 2)
                            {
                                PrintPreviewCanvas1 = new PrintCanvas(PrintDatas[0], 1.5);
                                PrintPreviewCanvas2 = new PrintCanvas(PrintDatas[1], 1.5);
                            }
                            if (PrintPreviewCanvas1 != null)
                            {
                                IsEnblePrint = true;
                                PrintAction();
                            }
                        }));
                    }
                    else
                    {
                        Hint.Update(LogType.Warning, $"{ScanMO}:MO码不正确,无法查询该MO码产品");
                    }
                    
                }
                catch (Exception ex)
                {
                    Hint.Update(LogType.Error, $"数据接受异常：{ex}");
                }
            }
            else if (e.Message.ID == "MANUAL")
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<Dictionary<object, object>>(e.Message.Data);
                    if ("OK".Equals(data.GetValueOrDefault("Status").ToString()))
                    {
                        var Order = JsonConvert.DeserializeObject<PrintModel>(data.GetValueOrDefault("Order").ToString());
                        var PrintDatas = JsonConvert.DeserializeObject<List<CanvasData>>(data.GetValueOrDefault("PrintDatas").ToString());

                        Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
                        {
                            Hint.Clean();
                            PrintData = Order;
                            if (PrintDatas != null && PrintDatas.Count == 2)
                            {
                                PrintPreviewCanvas1 = new PrintCanvas(PrintDatas[0], 1.5);
                                PrintPreviewCanvas2 = new PrintCanvas(PrintDatas[1], 1.5);
                                IsEnblePrint = true;
                            }
                            else
                            {
                                Hint.Update(LogType.Error, $"{Order.MO}:服务端模板数量不正确");
                            }
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Hint.Update(LogType.Error, $"数据接受异常：{ex}");
                }
            }
        }

        private void TCPClient_UpdateStatusEvent(object sender, TCPEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                //if(IsConnecting == false && e.Running == true && PrintData!= null 
                //&& !string.IsNullOrEmpty(PrintData.MO) && !string.IsNullOrEmpty(PrintData.Desc))
                //{
                //    //重写连接时，需要更新服务端数据
                //    var data = new RequestInfo("Print", JsonConvert.SerializeObject(PrintData));
                //    TCPClient.Instance.Send(data);
                //}
                IsConnecting = e.Running;
                if (!IsConnecting)
                {
                    Clean();
                }
                Hint.Update(LogType.Info, $"TCP:{e}");
            }));
        }


        #endregion

        #region 命令
        public void PrintSettingsCommand()
        {
            try
            {
                Process kbpr = System.Diagnostics.Process.Start("osk.exe"); // 打开系统键盘
                dynamic settings = new ExpandoObject();
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "打印设置";
                settings.ShowInTaskBar = false;
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                WindowManager.ShowDialog(new SystemSettingViewModel(), null, settings);
                if (!kbpr.HasExited)
                {
                    kbpr.Kill();
                }
            }
            catch (Exception ex)
            {
                Hint.Update(LogType.Error, $"打开屏幕键盘失败！{ex}");
            }
           
        }
        public void PrintTestCommand()
        {
            if (PrintData != null) return;
            dynamic settings = new ExpandoObject();
            settings.ResizeMode = ResizeMode.NoResize;
            //settings.WindowStyle = WindowStyle.None;
            //settings.AllowsTransparency = true;
            settings.Title = "打印测试";
            settings.ShowInTaskBar = false;
            //settings.Height = 200;
            //settings.Width = 800;
            //settings.SizeToContent = SizeToContent.Manual;
            //settings.Background = new SolidColorBrush(Colors.Transparent);
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            WindowManager.ShowDialog(new PrintTestViewModel(), null, settings);
            Clean();
        }
        public void onManualButtonClick()
        {
            if(IsConnecting && PrintData == null)
            {
                try
                {
                    Process kbpr = System.Diagnostics.Process.Start("osk.exe"); // 打开系统键盘
                    if (WindowManager.ShowDialog(ManualViewModel) == true)
                    {
                        //SendMO(ManualViewModel.MO);
                        dynamic data = new ExpandoObject();
                        data.MO = ManualViewModel.MO;
                        data.Index = ManualViewModel.Index - 1;
                        Communication.TcpSendData_Manual(data);
                        Clean();
                    }
                    if (!kbpr.HasExited)
                    {
                        kbpr.Kill();
                    }
                }
                catch (Exception ex)
                {
                    Hint.Update(LogType.Error, $"打开屏幕键盘失败！{ex}");
                }

            }
            else
            {
                Hint.Update(LogType.Info, "请先完成订单或者清除数据");
                IsManual = false;
            }
        }

        public void onManualPrintCommand()
        {
            if (IsEnblePrint && IsConnecting && IsManual)
            {
                PrintAction();
            }
            else
            {
                Hint.Update(LogType.Info, "请在手动模式下打印");
            }
            
        }

        public void CleanDataCommand()
        {
            if(WindowManagerExtension.ShowAckDialog(WindowManager, "清除数据","是否清除当前订单数据？请除后需重新请求数据打印。") == true)
            {
                Clean();
                Communication.TcpSendData_Clean("Clean");
            }

        }

        public void onScanMOCommand(ActionExecutionContext context)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                try
                {
                    if (!string.IsNullOrEmpty(CurrentScanMO))
                    {
                        ScanMO = CurrentScanMO;
                        CurrentScanMO = "";
                        SendMO(ScanMO);
                    }
                }
                catch (Exception ex)
                {
                    Hint.Update(LogType.Error, $"发送异常：{ex}");
                }
            }
        }

        public void ProductCommand()
        {
            if (IsConnecting && PrintData != null)
            {
                dynamic data = new ExpandoObject();
                data.MO = PrintData.MO;
                Communication.TcpSendData_ProductOK(data);
            }
            else
            {
                Hint.Update(LogType.Info, "当前MO码为空，操作失败！");
            }
        }
        #endregion


    }
}
