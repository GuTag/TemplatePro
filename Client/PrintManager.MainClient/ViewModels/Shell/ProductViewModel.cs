using Caliburn.Micro;
using Microsoft.Win32;
using Newtonsoft.Json;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static PrintManager.UI.Controls.PagePicker;

namespace PrintManager.MainClient.ViewModels.Shell
{
    public class ProductViewModel : ViewModelBase
    {
        public ProductViewModel()
        {
            PageData = new PageModel();
            PageData.PageChangeEvent += PageChangeEvent;
        }

        
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

        public string SearchText { get => _searchText; set => Set(ref _searchText, value); }
        private string _searchText;
        public ProductOrderModel SelectedOrder { get => _selectedOrder; set => Set(ref _selectedOrder, value); }
        private ProductOrderModel _selectedOrder;
        public ObservableCollection<ProductOrderModel> ProductOrderList { get => _productOrderList; set => Set(ref _productOrderList, value); }
        private ObservableCollection<ProductOrderModel> _productOrderList = new ObservableCollection<ProductOrderModel>();



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

            string type = TaskUtil.GetItemNodeType(TypeComboxIndex);
            Task.Run(() =>
            {

                var data = ProductOrderBLL.GetPage(PageData.Page, PageData.PageSize, ref totalCount, StartDateTime, EndDateTime, type, SearchText);
                var datas = JsonConvert.DeserializeObject<List<ProductOrderModel>>(data);
                var datalist = new List<ProductOrderModel>();
                foreach (var item in datas)
                {

                    LanguageText language = LanguageTextBLL.Find(item.NodeIndexLang);
                    if (language != null)
                    {
                        item.NodeDes = language.Language_zh;
                    }
                    else
                    {
                        //LanguageText languageNull = LanguageTextBLL.Find("T01_E9999");
                        //item.NodeDes = languageNull.Language_zh;

                        item.NodeDes = "未找到文本";
                    }
                    datalist.Add(item);
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (datalist != null)
                    {
                        
                        ProductOrderList = new ObservableCollection<ProductOrderModel>(datalist);
                    }
                    PageData.Totals = totalCount;
                });
            });
        }

        private void ShowParameterView(bool IsUpdate)
        {
            string _tag = "参数设置";
            dynamic settings = new ExpandoObject();
            settings.Height = 300;
            settings.Width = 700;
            settings.SizeToContent = SizeToContent.Manual;
            settings.Topmost = false;
            //settings.Owner = Application.Current.MainWindow;
            settings.Title = _tag;
            settings.Owner = null;
            var window = new ParUpdateTemp1ViewModel();
            window.ShowParView(SelectedOrder, IsUpdate);
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
                        TaskUtil.RequestExcelData(filepath);
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

        public void onProductOrderDoubleClick()
        {
            //if (SelectedOrder != null)
            //{
            //    var fileList = OrderHelpers.GetPrintTemplateFilePath(SelectedOrder.ProductOrderType, SelectedOrder.ItemNo);
            //    foreach (var filepath in fileList)
            //    {
            //        if (!File.Exists(filepath)) continue;
            //        bool isExistWindow = false;
            //        string _tag = Path.GetFileName(filepath) + "-" + SelectedOrder.ItemNo.ToString();
            //        foreach (Window w in Application.Current.Windows)
            //        {
            //            if (_tag.Equals(w.Tag?.ToString()))
            //            {
            //                w.Activate();
            //                isExistWindow = true;
            //                break;
            //            }

            //        }
            //        if (!isExistWindow)
            //        {
            //            dynamic settings = new ExpandoObject();
            //            settings.Tag = Path.GetFileName(filepath) + "-" + SelectedOrder.ItemNo.ToString();
            //            settings.Height = 450;
            //            settings.Width = 800;
            //            settings.SizeToContent = SizeToContent.Manual;
            //            settings.Topmost = false;
            //            settings.Title = Path.GetFileName(filepath) + " - " + SelectedOrder.ItemNo;
            //            //settings.Owner = Application.Current.MainWindow;
            //            settings.Owner = null;
            //            var window = new PrintViewModel();
            //            window.ShowPrintView(filepath, SelectedOrder);
            //            WindowManager.ShowWindow(window, null, settings);
            //        }
            //    }
            //}
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
                        ShowParameterView(IsUpdate);
                    }
                }
            }
            else
            {
                WindowManagerExtension.ShowMessageDialog(WindowManager, "请登录! ");
            }
        }

        public void onInsertCommand()
        {
            IsUpdate = false;
            ShowParameterView(IsUpdate);
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
