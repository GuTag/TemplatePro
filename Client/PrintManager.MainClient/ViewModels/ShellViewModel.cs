using Panuon.WPF;
using PrintManager.UI;
using PrintManager.MainClient.ViewModels.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PrintManager.MainClient.Models;
using System.Windows.Media;
using PrintManager.Shared.TCP;
using System.Windows;
using PrintManager.Shared.Attributes;
using System.IO;
using PrintManager.Shared;
using PrintManager.Sql;
using PrintManager.Sql.BLL;
using Newtonsoft.Json;
using PrintManager.Shared.Utils;
using PrintManager.MainClient.Components;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using System.Dynamic;
using System.Timers;
using System.Threading;
using PrintManager.Shared.SerialPorts;
using Caliburn.Micro;
using PrintManager.Shared.Helpers;
using static PrintManager.Shared.Entity.OrderEntity;
using PrintManager.Shared.Enums;
using PrintManager.MainClient.ViewModels.Dialog;
using NPOI.SS.Formula.Functions;
using OpcUaHelper;
using Opc.Ua;
using PrintManager.Sql.Models;
using System.Drawing.Design;
using Opc.Ua.Client;

namespace PrintManager.MainClient.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        #region ViewModels 数据模板
        public HomeViewModel HomeViewModel { get; }
        public LogViewModel LogViewModel { get; } = new LogViewModel();
        public ProductViewModel ProductViewModel { get; } = new ProductViewModel();
        public LanguageViewModel LanguageViewModel { get; } = new LanguageViewModel();
        public AnalogViewModel AnalogViewModel { get; } = new AnalogViewModel();
        public PrintViewModel PrintViewModel { get; } = new PrintViewModel();
        public ProductLogViewModel ProductLogViewModel { get; } = new ProductLogViewModel();


        #endregion

        #region 变量
        private System.Timers.Timer RequestExcelFileTimer = new System.Timers.Timer();
        private System.Timers.Timer RequestExcelCPQTimer = new System.Timers.Timer();
        private System.Timers.Timer RequestExcelProductTimer = new System.Timers.Timer();
        //private System.Timers.Timer SerialPortTimer = new System.Timers.Timer();
        private RS485 SerialPortRS485;

        private bool[] SerialPortRS485InputData = new bool[8];
        private int[] ProductOKNum= new int[4];

        private static int _lock = 0;
        private static List<string> LineMOList = new List<string>();

        private OpcUaClient m_OpcUaClient = new OpcUaClient();
        private List<String> MonitorAnalogNodeTags = new List<String>();
        private List<String> MonitorBoolNodeTags = new List<String>();
        #endregion

        #region 构造函数
        public ShellViewModel()
        {
            Initialize();

            LogViewModel.LogEvent += ViewLogEvent;
            PrintViewModel.LogEvent += ViewLogEvent;
            ProductViewModel.LogEvent += ViewLogEvent;
            ProductLogViewModel.LogEvent += ViewLogEvent;
            GlobalData.Instance.LoginStatusChanged += LoginStatusChanged;

            //扫描标签模板文件夹下的模板内容
            //GetPrintTempletes();

            //Home Page Loading
            HomeViewModel = new HomeViewModel();

            ////启动定时任务获取数据
            ////获取公盘打印数据
            //RequestExcelFileTimer.Enabled = true;
            //RequestExcelFileTimer.Interval = 3600000;//60分钟
            //RequestExcelFileTimer.AutoReset = true;
            //RequestExcelFileTimer.Start();
            //RequestExcelFileTimer.Elapsed += new ElapsedEventHandler(RequestExcelFile_Event);

            ////获取公盘客户数据
            //RequestExcelCPQTimer.Enabled = true;
            //RequestExcelCPQTimer.Interval = 3600000;//60分钟
            //RequestExcelCPQTimer.AutoReset = true;
            //RequestExcelCPQTimer.Start();
            //RequestExcelCPQTimer.Elapsed += new ElapsedEventHandler(RequestExcelCPQ_Event);

            ////导出报表
            //RequestExcelProductTimer.Enabled = true;
            //RequestExcelProductTimer.Interval = 600000;//10分钟
            //RequestExcelProductTimer.AutoReset = true;
            //RequestExcelProductTimer.Start();
            //RequestExcelProductTimer.Elapsed += new ElapsedEventHandler(RequestExcelProductTimer_Event);


            //RequestExcelCPQ();
            //RequestExcelFile();
            //RequestPDFFile();


        }

        

         
        /// <summary>
        /// 初始化
        /// </summary>
        private  void Initialize()
        {
            Task.Run(() =>
            {

                // database 
                var AutoConnectionDB = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "AutoConnectDB");
                if (AutoConnectionDB != null && AutoConnectionDB.ToUpper().Equals("TRUE"))
                {
                    try
                    {
                        var ConnectionString = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "ConnectionString");
                        ServerIP = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "LocalIPAdr");
                        //检查数据库
                        SqlSugarHelper.Instance.SetConnectString(ConnectionString);
                        try
                        {
                            if (!SqlSugarHelper.Instance.IsConnection)
                            {
                                SqlSugarHelper.Instance.CreateDatabase();
                            }
                            SqlSugarHelper.Instance.CreateTable();
                            LogViewModel.AddLog(LogType.Info, "系统", "数据库连接完成");
                            IsConnectDB = true;
                        }
                        catch (Exception e)
                        {
                            LogViewModel.AddLog(LogType.Error, "系统", $"数据库连接失败:{e.Message}");
                            WindowManagerExtension.ShowMessageDialog(WindowManager, e.Message);
                            IsConnectDB = false;
                        }
                    }
                    catch (Exception e)
                    {
                        LogViewModel.AddLog(LogType.Error, "系统", $"程序启动错误:{e.Message}");
                        Logger.Error(e, "程序启动错误");
                    }
                }

                //opc ua
                var AutoConnectionClicent = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "AutoConnectClient");
                var OPCAdr = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "OPCAdr");
                if (AutoConnectionClicent != null && AutoConnectionClicent.ToUpper().Equals("TRUE"))
                {
                    try
                    {
                        if (OPCAdr != null && OPCAdr.Length > 15)
                        {
                            m_OpcUaClient.UserIdentity = new UserIdentity(new AnonymousIdentityToken());
                            m_OpcUaClient.ConnectServer(OPCAdr);
                            if (m_OpcUaClient.Connected)
                            {
                                LogViewModel.AddLog(LogType.Info, "系统", "OPC UA 连接完成");
                                IsConnectClient = true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogViewModel.AddLog(LogType.Error, "系统", $"OPC UA连接错误:{e.Message}");
                        Logger.Error(e, "OPC UA连接错误");
                        IsConnectClient = false;
                    }
                }
            });

            SubOPCNode();
        }

        #endregion

        #region 属性
        public int SelectedMainMenuIndex { get => _selectedMainMenuIndex; set => Set(ref _selectedMainMenuIndex, value); }
        private int _selectedMainMenuIndex = 0;

        public int SelectedPrintMenuIndex { get => _selectedPrintMenuIndex; set => Set(ref _selectedPrintMenuIndex, value); }
        private int _selectedPrintMenuIndex = -1;

        public int SelectedViewIndex { get => _selectedViewIndex; set => Set(ref _selectedViewIndex, value); }
        private int _selectedViewIndex;
        public ObservableCollection<PrintItemModel> PrintItemModels { get => _printItemModels; set => Set(ref _printItemModels, value); }
        private ObservableCollection<PrintItemModel> _printItemModels = new ObservableCollection<PrintItemModel>();

        public bool IsConnectDB { get => _isConnectDB; set => Set(ref _isConnectDB, value); }
        private bool _isConnectDB;
        public bool IsConnectClient { get => _isConnectClient; set => Set(ref _isConnectClient, value); }
        private bool _isConnectClient;

        public bool IsStartOPCSub { get => _isStartOPCSub; set => Set(ref _isStartOPCSub, value); }
        private bool _isStartOPCSub;
        
        public string ServerIP { get => _serverIP; set => Set(ref _serverIP, value); }
        private string _serverIP;
        #endregion

        #region 方法

        private void SubOPCNode()
        {
            //Analog
            var AnalogNode = AnalogBLL.GetAll();
            foreach (var item in AnalogNode)
            {
                if (item.NodeAdr != null)
                {
                    MonitorAnalogNodeTags.Add(item.NodeAdr);
                }
            };
            //Console.WriteLine(MonitorAnalogNodeTags);

            //Bool
            var BoolNode = ProductOrderBLL.GetAll();
            foreach (var item in BoolNode)
            {
                if (item.NodeAdr != null)
                {
                    MonitorBoolNodeTags.Add(item.NodeAdr);
                }
            }
            //Console.WriteLine(MonitorBoolNodeTags);

            //Subject each node
            m_OpcUaClient.AddSubscription("A", MonitorAnalogNodeTags.ToArray(), Multi_SubCallbackA);
            m_OpcUaClient.AddSubscription("B", MonitorBoolNodeTags.ToArray(), Multi_SubCallbackB);
        }

        private void Multi_SubCallbackA(string arg1, MonitoredItem item, MonitoredItemNotificationEventArgs args)
        {

        }

        private void Multi_SubCallbackB(string arg1, MonitoredItem item, MonitoredItemNotificationEventArgs args)
        {

        }



        /// <summary>
        /// 导出DB 数据 到 Environments.AppDataPath, "F89_ProductData" 
        /// </summary>
        private void ExportDBToExcel()
        {
            var directory = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Chart", "ChartExcelSavePath");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            try
            {
                TaskUtil.ExportDBToExcel("0");
                LogViewModel.AddLog(LogType.Info,"系统",$"导出当日报表!");
            }
            catch (Exception e)
            {
                LogViewModel.AddLog(LogType.Warning,"系统",$"导出当日报表错误:{e.Message}");
            }
            
        }
        /// <summary>
        /// 获取公盘打印数据
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void RequestExcelFile()
        {
            Task.Run(() =>
            {
                LogViewModel.AddLog(LogType.Info,"系统","开始获取公盘Excel数据");
                string today = DateTime.Now.Date.ToString("yyyyMMdd");
                string yesterday = DateTime.Now.AddDays(-1).Date.ToString("yyyyMMdd");
                string filename = $"ZSDR01N_{today}.xls";
                string filepath = Path.Combine("Z:\\SAP", filename);
                string localFilepath = Path.Combine(Environments.AppDataPath, "Local",filename);
                try
                {
                    if (!Directory.Exists(Path.Combine(Environments.AppDataPath, "Local")))
                    {
                        Directory.CreateDirectory(Path.Combine(Environments.AppDataPath, "Local"));
                    }
                    //删除昨日数据
                    File.Delete(Path.Combine(Environments.AppDataPath, $"ZSDR01N_{yesterday}.xls"));
                    File.Delete(Path.Combine(Environments.AppDataPath, "Local", $"ZSDR01N_{yesterday}.xls"));

                    //复制文件
                    if (File.Exists(filepath))
                    {
                        var result = FileUtil.CopyToFile(filepath, Environments.AppDataPath);
                        if (result)
                        {
                            LogViewModel.AddLog(LogType.Info,"系统",$"获取公盘Excel数据成功:{filepath}");
                            TaskUtil.TxtToExcel(Path.Combine(Environments.AppDataPath, filename), localFilepath);
                        }
                        else
                        {
                            LogViewModel.AddLog(LogType.Warning,"系统",$"获取公盘Excel数据失败:{filepath}");
                        }
                        if (File.Exists(Path.Combine(Environments.AppDataPath, filename)))
                        {
                            LogViewModel.AddLog(LogType.Info,"系统",$"开始转换:{filepath}");
                            TaskUtil.TxtToExcel(Path.Combine(Environments.AppDataPath, filename), localFilepath);
                        }
                    }
                    if (File.Exists(localFilepath))
                    {
                        LogViewModel.AddLog(LogType.Info,"系统",$"开始解析:{filepath}");
                        TaskUtil.RequestExcelData(localFilepath);
                        LogViewModel.AddLog(LogType.Info,"系统",$"解析成功:{filepath}");
                    }
                }
                catch (Exception ex)
                {
                    LogViewModel.AddLog(LogType.Error,"系统",$"自动导入Excel数据源{filepath}异常：{ex.Message}");
                }

            });
        }

        private void RequestPDFFile()
        {
            //Z:\部门文件夹_MRP\MO picker跟踪\MO picker\2023年\20230308
            Task.Run(() =>
            {
                LogViewModel.AddLog(LogType.Info,"系统","开始获取公盘PDF数据");
                string today = DateTime.Now.Date.ToString("yyyyMMdd");
                string yesterday = DateTime.Now.AddDays(-1).Date.ToString("yyyyMMdd");
                string fileDirectory = Path.Combine("Z:\\部门文件夹_MRP\\MO picker跟踪\\MO picker", $"{DateTime.Now.Date.ToString("yyyy")}年", today);
                string localDirectory = Path.Combine(Environments.AppDataPath, "Local", today);
                string yesterdayDirectory = Path.Combine(Environments.AppDataPath, "Local", yesterday);
                try
                {
                    //删除昨日数据
                    if(Directory.Exists(yesterdayDirectory)) 
                    {
                        DirectoryInfo subdir = new DirectoryInfo(yesterdayDirectory);
                        subdir.Delete(true);
                    }


                    //复制文件
                    if (Directory.Exists(fileDirectory))
                    {
                        var result = FileUtil.CopyDir(fileDirectory, localDirectory);
                        if (result)
                        {
                            LogViewModel.AddLog(LogType.Info,"系统",$"获取公盘PDF数据成功:{fileDirectory}");
                        }
                        else
                        {
                            LogViewModel.AddLog(LogType.Warning,"系统",$"获取公盘PDF数据失败:{fileDirectory}");
                        }
                        GlobalData.Instance.MOList.Clear();
                        string[] fileList = Directory.GetFiles(localDirectory, "*.pdf");
                        foreach (var file in fileList)
                        {
                            LogViewModel.AddLog(LogType.Info,"系统",$"开始解析PDF:{file}");
                            TaskUtil.RequestPDFData(file);
                        }
                        LogViewModel.AddLog(LogType.Info,"系统",$"解析PDF成功:{localDirectory}");
                    }
                }
                catch (Exception ex)
                {
                    LogViewModel.AddLog(LogType.Error,"系统",$"自动导入今日PDF数据源{fileDirectory}异常：{ex.Message}");
                }

            });
        }

        /// <summary>
        /// 获取公盘CPQ数据
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void RequestExcelCPQ()
        {
            Task.Run(() =>
            {
                string filename = Fields. CloudFileName;
                string filepath = Path.Combine(Fields.CloudFilePath, filename);
                try
                {
                    if (File.Exists(filepath))
                    {
                        var result = FileUtil.CopyToFile(filepath, Environments.AppDataPath);
                        if (result)
                        {
                            LogViewModel.AddLog(LogType.Info,"系统",$"获取公盘CPQ数据成功:{filepath}");
                            TaskUtil.RequestExcelCPQ(Path.Combine(Environments.AppDataPath, filename));
                        }
                        else
                        {
                            LogViewModel.AddLog(LogType.Warning,"系统",$"获取公盘CPQ数据失败:{filepath}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogViewModel.AddLog(LogType.Error,"系统",$"自动导入CPQ数据源{filepath}异常：{ex.Message}");
                }
                
            });
            
        }
        /// <summary>
        /// 获取打印模板
        /// </summary>
        private void GetPrintTempletes()
        {
            PrintItemModels.Clear();
            DirectoryInfo folder = new DirectoryInfo(Environments.PrintFolderPath);
            foreach (FileInfo file in folder.GetFiles("*.pm"))
            {
                var name = file.Name.Split('.')[0];
                PrintItemModels.Add(new PrintItemModel() { DisplayName = name, AccentBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500")), });
            }
        }

        /// <summary>
        /// 根据客户端MO返回数据
        /// </summary>
        /// <param name="ipHost"></param>
        /// <param name="mo"></param>
        private void SendDataOfMO(string ipHost, string mo)
        {
            TCPServer.Instance.ClientDictionary[ipHost] = "";
            string message;
            dynamic response = new ExpandoObject();
            response.Status = "Error";

            var (item, PrintDatas) = Communication.GetMO(mo);
            if (item != null && PrintDatas != null)
            {
                TCPServer.Instance.ClientDictionary[ipHost] = mo;
                response.Status = "OK";
                response.Order = item;


                //System.Diagnostics.Debug.WriteLine($"1 {DateTime.Now.ToString("HH:mm:ss.ffff")}:{item.MO},{item.ComplatedNum},{String.Join(",", LineMOList)},{item.IsOK}");
                if (LineMOList.Contains(mo) || item.ComplatedNum == item.RequestNum)
                {
                    PrintDatas.Clear();

                    //System.Diagnostics.Debug.WriteLine($"2 {DateTime.Now.ToString("HH:mm:ss.ffff")}:{item.MO},{item.ComplatedNum},{String.Join(",", LineMOList)},{item.IsOK}");
                }
                response.PrintDatas = PrintDatas;
                //System.Diagnostics.Debug.WriteLine(PrintDatas.Count);

                if (item.IsOK)
                {
                    message = $"接收MO查询数据:{mo} 成功，产品合格，执行打印";
                    if (!LineMOList.Contains(mo))
                    {
                        LineMOList.Add(mo);
                    }
                    //IsPrintingLine[line] = true;
                    //ProductOrderBLL.UpdateProductPrinting(mo, true);
                }
                else
                {
                    message = $"接收MO查询数据:{mo} 成功，等待产品合格";
                    if (LineMOList.Contains(mo))
                    {
                        LineMOList.Remove(mo);
                    }
                    //IsPrintingLine[line] = false;
                    //ProductOrderBLL.UpdateProductPrinting(mo, false);
                }
            }
            else
            {
                message = $"接收MO查询数据:{mo} 失败";
            }
            TCPServer.Instance.SendClient(new RequestInfo("MO", JsonConvert.SerializeObject(response)), TCPServer.Instance.GetClientID(ipHost));
            LogViewModel.AddLog(LogType.Info, LineUtil.GetLineName(ipHost), message);
        }

        /// <summary>
        /// 产品合格
        /// </summary>
        /// <param name="line"></param>
        private void ProductOKAndSendPrint(int line)
        {
            //分为三个点位，对应不同IP地址
            try
            {

                string mo = "";
                string ipHost = "";
                switch (line)
                {
                    case 1:
                        (ipHost, mo) = LineUtil.GetLineMO(line);
                        break;
                    case 2:
                        (ipHost, mo) = LineUtil.GetLineMO(line);
                        break;
                    case 3:
                        (ipHost, mo) = LineUtil.GetLineMO(line);
                        break;
                }
                if (!string.IsNullOrEmpty(mo))
                {
                    ProductOrderBLL.UpdateProductOK(mo);
                    SendDataOfMO(ipHost, mo);
                }
                else
                {
                    LogViewModel.AddLog(LogType.Warning, $"{line}线", $"产品的合格信号无法找到对应MO码:{mo}");
                }
            }
            catch (Exception e)
            {
                LogViewModel.AddLog(LogType.Warning, $"{line}线", $"产品的合格信号异常，{e}");
            }

        }

        
        #endregion

        #region 事件
        /// <summary>
        /// 串口接受数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="inputs"></param>
        private void SerialPortRS485_ReiceveData(object sender, bool[] inputs)
        {
            //如果上一次合格信号存在，则不计数。
            //反之，则计数1次
            //当有两次合格信号时，则产品合格，清除次数
            if(inputs == null || inputs.Length != 8) return;
            bool[] result = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                if (SerialPortRS485InputData[i])
                {
                    result[i] = false;
                }
                else
                {
                    result[i] = inputs[i];
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (result[i])
                {
                    LogViewModel.AddLog(LogType.Info,"系统",$"接收合格信号，产品检测机构{i + 1},");
                    ProductOKNum[i]++;
                    if (ProductOKNum[i] >= 2)
                    {
                        ProductOKNum[i] = 0;
                        if (i == 0 || i == 1)
                        {
                            ProductOKAndSendPrint(1);
                        }
                        else
                        {
                            ProductOKAndSendPrint(i);
                        }

                    }


                }
            }
            SerialPortRS485InputData = inputs;
        }

        /// <summary>
        /// 串口状态更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPortRS485_UpdateStatusEvent(object sender, string e)
        {
            LogViewModel.AddLog(LogType.Warning,"系统",e);
        }

        /// <summary>
        /// 定时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void RequestExcelFile_Event(object sender, ElapsedEventArgs e)
        {
            RequestExcelFile();
            RequestPDFFile();
        }

        /// <summary>
        /// 定时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void RequestExcelCPQ_Event(object sender, ElapsedEventArgs e)
        {
            RequestExcelCPQ();
        }
        /// <summary>
        /// 定时导出报表 18:00 - 18:10
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RequestExcelProductTimer_Event(object sender, ElapsedEventArgs e)
        {
            if ((DateTime.Now.TimeOfDay >= Convert.ToDateTime("16:50:00").TimeOfDay && 
                DateTime.Now.TimeOfDay <  Convert.ToDateTime("17:00:00").TimeOfDay) || 
                (DateTime.Now.TimeOfDay >= Convert.ToDateTime("20:15:00").TimeOfDay &&
                DateTime.Now.TimeOfDay < Convert.ToDateTime("20:25:00").TimeOfDay))
            {
                ExportDBToExcel();
            }
        }
        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewLogEvent(object sender, object logModel)
        {
            if(logModel is HistoryLogModel)
            {
                LogViewModel.AddLog(logModel as HistoryLogModel);
            }
            else
            {
                LogViewModel.AddLog(LogType.Info, "系统", logModel.ToString());
            }

        }

        private void LoginStatusChanged()
        {
            if (GlobalData.Instance.IsLogin)
            {
                LogViewModel.AddLog(LogType.Info,"系统",$"用户{GlobalData.Instance.User.UserName}登录成功");
            }
            else
            {
                LogViewModel.AddLog(LogType.Info,"系统",$"用户{GlobalData.Instance.User.UserName}退出登录");
            }
        }

        /// <summary>
        /// 服务接收数据事件
        /// </summary>
        /// <param name="e"></param>
        private void TCPServer_RecviceMsg(TCPRecviceMsg e)
        {
            //解析瘦客户端数据
            //判断类型
            //根据MO码查询数据库中订单
            //查询结果返回
            dynamic response = new ExpandoObject();
            switch (e.Message.ID)
            {
                case "MO":
                    //通过MO码获取数据
                    var mo = JsonConvert.DeserializeObject<string>(e.Message.Data);
                    SendDataOfMO(e.IP, mo);
                    HomeViewModel.UpdateItemSourceLine(ProductOKNum);
                    break;
                case "PRINT":
                    var order = JsonConvert.DeserializeObject<ProductOrderModel>(e.Message.Data);
                    //打印记录更新
                    string message;
                    if (order != null)
                    {
                        ProductOrderBLL.UpdateComplatedNum(e.Message.Data);
                        LogViewModel.AddLog(LogType.Info, LineUtil.GetLineName(e.IP), $"接收打印记录数据{e.IP}:{e.Message.ID}，{order.MO} 打印1次，当前完成次数{order.ComplatedNum}");
                        HomeViewModel.AddLog(e.IP, order.MO);
                        string PrintMO = TCPServer.Instance.ClientDictionary[e.IP];
                        if (LineMOList.Contains(PrintMO))
                        {
                            LineMOList.Remove(PrintMO);

                        }
                        if (order.RequestNum > order.ComplatedNum)
                        {
                            SendDataOfMO(e.IP, order.MO);
                        }
                        else
                        {
                            LogViewModel.AddLog(LogType.Info, LineUtil.GetLineName(e.IP), $"接收打印记录数据 {order.MO} 订单打印完成");
                        }
                        if (SelectedViewIndex == 0)
                        {
                            HomeViewModel.UpdateItemSource();
                        }
                    }
                    else
                    {
                        LogViewModel.AddLog(LogType.Error, LineUtil.GetLineName(e.IP), $"接收打印记录数据{e.IP}:{e.Message.ID}，数据错误");
                    }
                    break;
                case "TEST":
                    //通过MO码获取数据
                    var type = JsonConvert.DeserializeObject<string>(e.Message.Data);
                    DirectoryInfo folder = new DirectoryInfo(Environments.PrintFolderPath);
                    var PrintDatas = new List<CanvasData>();
                    foreach (FileInfo file in folder.GetFiles("*.pm"))
                    {
                        var template = OrderHelpers.GetPrintTemplate(file.FullName);
                        if (template.Type == type)
                        {
                            PrintDatas.Add(OrderHelpers.GetPrintData(file.FullName, "89D0140406P14N2M00F07S17", "8010105272", 1, "3030015792/100", "1000067869",OrderType.F89, "自动阀 for KTM", ""));
                            break;
                        }
                    }
                    response.Status = "OK";
                    response.PrintDatas = PrintDatas;
                    TCPServer.Instance.SendClient(new RequestInfo("TEST", JsonConvert.SerializeObject(response)), TCPServer.Instance.GetClientID(e.IP));
                    LogViewModel.AddLog(LogType.Info, LineUtil.GetLineName(e.IP), $"打印测试:{e.Message.ID}，{type}");
                    break;
                case "MANUAL":
                    
                    //通过MO码获取数据
                    var manualData = JsonConvert.DeserializeObject<Dictionary<object, object>>(e.Message.Data);
                    var manualMO = JsonConvert.DeserializeObject<string>(manualData.GetValueOrDefault("MO").ToString());
                    var manualIndex = JsonConvert.DeserializeObject<int>(manualData.GetValueOrDefault("Index").ToString());

                    var (item, ManualPrintDatas) = Communication.GetManulMO(manualMO, manualIndex);
                    response.Status = "Error";
                    if (item != null && ManualPrintDatas != null && manualIndex < item.RequestNum)
                    {
                        //for (int i = 0; i < ProductOKNum.Length; i++)
                        //{
                        //    ProductOKNum[i] = 0;
                        //}
                        item.ComplatedNum = manualIndex;
                        TCPServer.Instance.ClientDictionary[e.IP] = manualMO;
                        response.Status = "OK";
                        response.Order = item;
                        response.PrintDatas = ManualPrintDatas;
                        message = $"手动打印MO查询数据{e.IP}:{manualMO} 成功，执行打印";
                    }
                    else
                    {
                        message = $"手动打印MO查询数据{e.IP}:{manualMO} 失败";
                    }
                    TCPServer.Instance.SendClient(new RequestInfo("MANUAL", JsonConvert.SerializeObject(response)), TCPServer.Instance.GetClientID(e.IP));
                    LogViewModel.AddLog(LogType.Info, LineUtil.GetLineName(e.IP), $"手动打印:{e.Message.ID}，{message}");
                    break;
                case "CLEAN":
                    string cleanMO = TCPServer.Instance.ClientDictionary[e.IP];
                    if (LineMOList.Contains(cleanMO))
                    {
                        LineMOList.Remove(cleanMO);

                    }
                    TCPServer.Instance.ClientDictionary[e.IP] = "";
                    break;
                case "PRODUCTOK":
                    var ip = e.IP;
                    var OKData = JsonConvert.DeserializeObject<Dictionary<object, object>>(e.Message.Data);
                    var OKMO = JsonConvert.DeserializeObject<string>(OKData.GetValueOrDefault("MO").ToString());
                    TCPServer.Instance.ClientDictionary[e.IP] = OKMO;

                    var ConfigClent1 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "ClientIP", "1");
                    var ConfigClent2 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "ClientIP", "2");
                    var ConfigClent3 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "ClientIP", "3");
                    if (ip.StartsWith(ConfigClent1))
                    {
                        ManualProductOKCommand(1);
                    }
                    else if (ip.StartsWith(ConfigClent2))
                    {
                        ManualProductOKCommand(3);
                    }
                    else if (ip.StartsWith(ConfigClent3))
                    {
                        ManualProductOKCommand(4);
                    }
                    LogViewModel.AddLog(LogType.Info,LineUtil.GetLineName(e.IP),$"来自客户端的手动产品合格信号:{e.Message.ID}:{OKMO}");
                    break;
                default:
                    break;
            }
            HomeViewModel.UpdateItemSourceLine(ProductOKNum);
        }

        /// <summary>
        /// 服务状态更新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TCPServer_UpdateStatusEvent(object sender, TCPEventArgs e)
        {
            //Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
            //{
            //    LogViewModel.AddLog(LogType.Info, LineUtil.GetLineName(e.IP), e.ToString());
            //    ServerRunning = e.Running;
            //    HomeViewModel.UpdateItemSourceLine(ProductOKNum);
            //}));

            //try
            //{
            //    if (LineMOList.Count > 0)
            //    {
            //        var molist = TCPServer.Instance.ClientDictionary.Values.ToList();
            //        for (int i = LineMOList.Count - 1; i >= 0; i--)
            //        {
            //            if (!molist.Contains(LineMOList[i]))
            //            {
            //                LineMOList.RemoveAt(i);
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogViewModel.AddLog(LogType.Error, LineUtil.GetLineName(e.IP), $"连接异常{ex.Message}");
            //}
        }


        private void CheckUsedManager_TimeToFinishedEvent(object sender, EventArgs e)
        {
            CheckUsedManager.TimeToFinishedEvent -= new EventHandler(CheckUsedManager_TimeToFinishedEvent);
            GlobalData.Instance.IsLogin = false;
        }
        #endregion

        #region 命令
        /// <summary>
        /// 手动产品合格
        /// 1,2 =》 1线 
        /// 3 =》 2线
        /// 4 =》 3线
        /// </summary>
        /// <param name="line"></param>
        public void ManualProductOKCommand(int index)
        {
            //手动合格时清除合格计数
            if (index > 0 && index <= 4)
            {
                ProductOKNum[index - 1] = 0;

                if (index == 1 || index == 2)
                {
                    ProductOKAndSendPrint(1);
                }
                else
                {
                    ProductOKAndSendPrint(index - 1);
                }
            }
        }

        /// <summary>
        /// 菜单选择改变
        /// </summary>
        public void OnMainMenuSelectionChanged()
        {
            if (SelectedMainMenuIndex != -1)
            {
                SelectedPrintMenuIndex = -1;
                SelectedViewIndex = SelectedMainMenuIndex;
                if(SelectedViewIndex == 0) 
                {
                    //HomeViewModel.UpdateItemSource();
                }
            }
        }

        /// <summary>
        /// 菜单选择改变
        /// </summary>
        public void OnPrintSelectionChanged()
        {
            if (SelectedPrintMenuIndex != -1)
            {
                SelectedMainMenuIndex = -1;
                SelectedViewIndex = 4;

                //点击模板时检查文件是否存在
                var filepath = Path.Combine(Environments.PrintFolderPath, PrintItemModels[SelectedPrintMenuIndex].DisplayName + ".pm");
                if (File.Exists(filepath))
                {
                    PrintViewModel.updatePrintView(filepath);
                }
                else
                {
                    PrintItemModels.Remove(PrintItemModels[SelectedPrintMenuIndex]);
                }
                
            }
        }

        /// <summary>
        /// 重新读取模板文件夹
        /// </summary>
        public void onRefreshPrintTemplete()
        {
            GetPrintTempletes();
            SystemUtil.GetRatio();
        }


        public void UserLoginCommand()
        {
            dynamic settings = new ExpandoObject();
            settings.Title = "设置";
            settings.ResizeMode = ResizeMode.NoResize;
            settings.ShowInTaskBar = false;
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var result = WindowManager.ShowDialog(new LoginViewModel(), null, settings);
            if (result)
            {
                CheckUsedManager.TimeToFinishedEvent += new EventHandler(CheckUsedManager_TimeToFinishedEvent);
            }
        }



        public void SettingViewCommand()
        {
            dynamic settings = new ExpandoObject();
            settings.Title = "设置";
            settings.ResizeMode = ResizeMode.NoResize;
            settings.ShowInTaskBar = false;
            //settings.Background = new SolidColorBrush(Colors.Transparent);
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            WindowManager.ShowDialog(new SystemSettingViewModel(), null, settings);
        }

        public void AboutCommand()
        {
            dynamic settings = new ExpandoObject();
            settings.Title = "关于智能标签管理软件";
            settings.ResizeMode = ResizeMode.NoResize;
            settings.ShowInTaskBar = false;
            //settings.Background = new SolidColorBrush(Colors.Transparent);
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            WindowManager.ShowDialog(new AboutViewModel(), null, settings);
        }
        #endregion

        #region 重写方法

        public override void CanClose(Action<bool> callback)
        {
            var isCanClose = false;
            if (IsConnectClient || IsConnectDB )
            {
                if (WindowManagerExtension.ShowAckDialog(WindowManager, "关闭程序确认", "关闭程序时，停止服务器功能，是否关闭？").Equals(true))
                {
                    isCanClose = true;
                }
            }
            else
            {
                isCanClose = true;
            }
            if (isCanClose)
            {
                LogViewModel.AddLog(LogType.Error,"系统","软件停止运行!!");
                Logger.Infomation("软件停止运行!!");
                base.CanClose(callback);
            }
            
        }
        #endregion
    }
}
