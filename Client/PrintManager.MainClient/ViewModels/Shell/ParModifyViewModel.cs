using Caliburn.Micro;
using Microsoft.Win32;
using Newtonsoft.Json;
using Opc.Ua;
using PrintManager.MainClient.Components;
using PrintManager.MainClient.Models;
using PrintManager.MainClient.Views;
using PrintManager.Shared;
using PrintManager.Shared.Enums;
using PrintManager.Shared.Helpers;
using PrintManager.Shared.Utils;
using PrintManager.Sql.BLL;
using PrintManager.Sql.Models;
using PrintManager.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static PrintManager.UI.Controls.PagePicker;

namespace PrintManager.MainClient.ViewModels.Shell
{
    public class ParModifyViewModel : ViewModelBase
    {
        public ParModifyViewModel()
        {
            PageData = new PageModel();
            PageData.PageChangeEvent += PageChangeEvent;
            UpdateProductOrderList();
        }


        public LogViewModel LogViewModel { get; } = new LogViewModel();

        #region 属性数据
        public PageModel PageData { get => _pageData; set => Set(ref _pageData, value); }
        private PageModel _pageData;

        public DateTime StartDateTime { get => _startDateTime; set => Set(ref _startDateTime, value); }
        private DateTime _startDateTime = DateTime.Now;

        public DateTime EndDateTime { get => _endDateTime; set => Set(ref _endDateTime, value); }
        private DateTime _endDateTime = DateTime.Now;

       
        //public int StatusComboxIndex { get => _statusComboxIndex; set => Set(ref _statusComboxIndex, value); }
        //private int _statusComboxIndex = 0;
        public int TypeComboxIndex { get => _typeComboxIndex; set => Set(ref _typeComboxIndex, value); }
        private int _typeComboxIndex = 0;

        public string ActualValue { get => _actualValue; set => Set(ref _actualValue, value); }
        private string _actualValue;
        public string NodeDes { get => _nodeDes; set => Set(ref _nodeDes, value); }
        private string _nodeDes;

        public string SearchText { get => _searchText; set => Set(ref _searchText, value); }
        private string _searchText;
        public ParModify SelectedOrder { get => _selectedOrder; set => Set(ref _selectedOrder, value); }
        private ParModify _selectedOrder;
        public ObservableCollection<ParModify> ProductOrderList { get => _productOrderList; set => Set(ref _productOrderList, value); }
        private ObservableCollection<ParModify> _productOrderList = new ObservableCollection<ParModify>();



        #endregion

        #region 变量
        private int totalCount = 0;
        public bool IsAdd { get; set; } = false;
        public bool IsUpdate { get; set; } = false;
        #endregion

        #region 方法



        //分页查询
        public   void UpdateProductOrderList()
        {
            Task.Run(() =>
            {
                var data = ParModifyBLL.GetPage(PageData.Page, PageData.PageSize, ref totalCount, SearchText);
                var datas = JsonConvert.DeserializeObject<List<ParModify>>(data);
                var datalist = new List<ParModify>();
                foreach (var item in datas)
                {
                    datalist.Add(item);
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (datalist != null)
                    {
                        ProductOrderList = new ObservableCollection<ParModify>(datalist);
                    }
                    PageData.Totals = totalCount;
                });
            });
        }

        private void ShowParameterView()
        {
            string _tag = "参数修改";
            dynamic settings = new ExpandoObject();
            settings.Height = 170;
            settings.Width = 500;
            settings.SizeToContent = SizeToContent.Manual;
            settings.Topmost = false;
            //settings.Owner = Application.Current.MainWindow;
            settings.Title = _tag;
            settings.Owner = null;
            var window = new ParUpdateTemp4ViewModel();
            window.ShowParView(SelectedOrder);
            WindowManager.ShowWindow(window, null, settings);
        }


        #endregion

        #region 事件

        private void PageChangeEvent()
        {
            UpdateProductOrderList();
        }

        #endregion

        #region 命令

        public void onDownloadCommand()
        {
            if (GlobalData.IsConnectClient)
            {
                LogEventMessage($"数据开始下载到客户端!");

                List<ParModify> parModifies = new List<ParModify>();
                int count = 0;
                int size = 1000;
                string[] tags = new string[size];
                object[] values = new object[size];


                //get all data
                parModifies = ParModifyBLL.GetAll();

                foreach (var item in parModifies)
                {
                    if (item.NodeAdr != null || item.ActualValue != null)
                    {
                        tags[count] = (TaskUtil.PLCNodeTransferToOPCNode(item.NodeAdr));
                        values[count] = (Convert.ToInt16(item.ActualValue));
                        count++;
                    }
                }

                //download all data
                if (parModifies == null) return;

                //string[] test1 = new string[2];
                //object[] test2 = new object[2];

                //test1[0] = "ns=3;s=\"parSetting\".\"par1\"";
                //test2[0] = (int)100;
                //test1[1] = "ns=3;s=\"parSetting\".\"par2\"";
                //test2[1] = 200;

                //GlobalData.m_OpcUaClient.WriteNodes(test1, test2);


                if (GlobalData.m_OpcUaClient.WriteNodes(tags, values) || true)
                {
                    WindowManagerExtension.ShowMessageDialog(WindowManager, "下载成功! ");
                    LogEventMessage($"下载成功!");
                }
                else
                {
                    WindowManagerExtension.ShowMessageDialog(WindowManager, "下载失败请检查数据! ");
                    LogEventMessage($"下载失败请检查数据!");
                }

            }
            else
            {
                WindowManagerExtension.ShowMessageDialog(WindowManager, "请先连接客户端! ");
            }
        }
        public void onSearchButtonClick()
        {
            PageData.Page = 1;
            UpdateProductOrderList();
        }

        public void onImportExcelCommand()
        {

            //手动解析Excel，获取数据
            var dialog = new OpenFileDialog
            {
                Title = "选择Excel文件",
                Filter = "Excel文件|*.xlsx;*.xls",

            };
            if ((bool)dialog.ShowDialog())
            {
                string filepath = dialog.FileName;

                var message = $"手动导入数据源：{filepath}";
                LogEventMessage(message);

                Task.Run(() =>
                {
                    try
                    {
                        TaskUtil.RequestExcelDataOfParModify(filepath);
                        LogEventMessage($"手动导入数据源{filepath}：成功");
                        UpdateProductOrderList();
                    }
                    catch (Exception e)
                    {
                        WindowManagerExtension.ShowMessageDialog(WindowManager, e);
                        LogEventMessage($"手动导入数据源{filepath}异常：{e.Message}");
                    }
                });         
            }
           
        }


        public void onInputTextSearch(ActionExecutionContext context)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                PageData.Page = 1;
                UpdateProductOrderList();
            }
        }

        public void onProductOrderDoubleClick(object obj)
        {

            if (GlobalData.Instance.IsLogin)
            {
                IsUpdate = true;
                if (SelectedOrder != null)
                {
                    string _tag = "参数修改";
                    bool isExistWindow = false;
                    foreach (Window w in Application.Current.Windows)
                    {
                        if (_tag.Equals(w.Tag?.ToString()))
                        {
                            w.Activate();
                            isExistWindow = true;
                            break;
                        }

                    }
                    if (!isExistWindow)
                    {
                        ShowParameterView();
                    }
                }
            }
            else
            {
                WindowManagerExtension.ShowMessageDialog(WindowManager, "请登录! ");
            }
        }


        public void onProductOrderChange(ParModify par)
        {
            if (par == null) return;
            Console.WriteLine(par.NodeAdr);

            ActualValue = par.ActualValue;
            NodeDes = par.NodeDes;

        }

        public void onUpdateItemCommand()
        {
            if (SelectedOrder != null) 
            {
                ParModifyBLL.UpdateOfNodeAdr(new ParModify
                {
                    ActualValue = ActualValue,
                    NodeDes = NodeDes,
                    NodeAdr = SelectedOrder.NodeAdr,
                    ClientName = SelectedOrder.ClientName,
                    AddTime = SelectedOrder.AddTime,
                    Index  = SelectedOrder.Index,
                }); ;
                UpdateProductOrderList();
            }
        }


        public void onInsertCommand()
        {
            IsUpdate = false;
            ShowParameterView();
            UpdateProductOrderList();
        }

        public void onDeleteCommand()
        {

                if (SelectedOrder != null)
                {
                    if (WindowManagerExtension.ShowAckDialog(WindowManager, "删除确认", "是否删除该数据！") == true)
                    {
                        ProductOrderBLL.Delete(SelectedOrder.NodeAdr);
                        UpdateProductOrderList();
                    }
                 }
                else
                {
                    WindowManagerExtension.ShowMessageDialog(WindowManager, "请选择一条数据！");
                }
          
        }
        #endregion
    }
}
