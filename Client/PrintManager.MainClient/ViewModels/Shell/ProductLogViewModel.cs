using Caliburn.Micro;
using Microsoft.Win32;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using PrintManager.MainClient.Components;
using PrintManager.MainClient.Models;
using PrintManager.MainClient.Views;
using PrintManager.Shared;
using PrintManager.Shared.Enums;
using PrintManager.Shared.Helpers;
using PrintManager.Shared.Utils;
using PrintManager.Sql.BLL;
using PrintManager.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing.Drawing2D;
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
    public class ProductLogViewModel : ViewModelBase
    {
        public ProductLogViewModel()
        {
            PageData = new PageModel();
            PageData.PageChangeEvent += PageChangeEvent;
        }


        #region 属性数据

        public LogViewModel LogViewModel { get; } = new LogViewModel();
        public PageModel PageData { get => _pageData; set => Set(ref _pageData, value); }
        private PageModel _pageData;

        public DateTime StartDateTime { get => _startDateTime; set => Set(ref _startDateTime, value); }
        private DateTime _startDateTime = DateTime.Now;

        public DateTime EndDateTime { get => _endDateTime; set => Set(ref _endDateTime, value); }
        private DateTime _endDateTime = DateTime.Now;

       
        public int LineComboxIndex { get => _lineComboxIndex; set => Set(ref _lineComboxIndex, value); }
        private int _lineComboxIndex = 0;
        public int TypeComboxIndex { get => _typeComboxIndex; set => Set(ref _typeComboxIndex, value); }
        private int _typeComboxIndex = 0;

        public string SearchText { get => _searchText; set => Set(ref _searchText, value); }
        private string _searchText;
        public ProductOrderLog SelectedOrder { get => _selectedOrder; set => Set(ref _selectedOrder, value); }
        private ProductOrderLog _selectedOrder;
        public ObservableCollection<ProductOrderLog> ProductOrderList { get => _productOrderList; set => Set(ref _productOrderList, value); }
        private ObservableCollection<ProductOrderLog> _productOrderList = new ObservableCollection<ProductOrderLog>();

        #endregion

        #region 变量
        private int totalCount = 0;
        List<ProductOrderLog> data = new List<ProductOrderLog>();
        #endregion

        #region 方法
        //分页查询
        private void UpdateProductOrderList()
        {
            Task.Run(() => 
            {
                var _line = "";
                var _type = "";
                switch (LineComboxIndex)
                {
                    case 1:
                        _line = "一线";
                        break;
                    case 2:
                        _line = "二线";
                        break;
                    case 3:
                        _line = "三线";
                        break;
                    default:
                        _line = "";
                        break;
                }
                switch (TypeComboxIndex)
                {
                    case 1:
                        _type = "F89";
                        break;
                    case 2:
                        _type = "RPX";
                        break;
                    default:
                        _type = "";
                        break;
                }
                data = ProductOrderLogBLL.GetPage(PageData.Page, PageData.PageSize, ref totalCount, StartDateTime, EndDateTime, _line, _type, SearchText);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (data != null)
                    {
                        ProductOrderList = new ObservableCollection<ProductOrderLog>(data);
                    }
                    PageData.Totals = totalCount;
                });
            });
            //Task.Run(() =>
            //{
            //    var data = ProductOrderBLL.GetPage(PageData.Page, PageData.PageSize, ref totalCount, StartDateTime, EndDateTime, LineComboxIndex, TypeComboxIndex, SearchText);
            //    var datas = JsonConvert.DeserializeObject<List<ProductOrderLog>>(data);
            //    Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        if (datas != null)
            //        {
            //            ProductOrderList = new ObservableCollection<ProductOrderLog>(datas);
            //        }
            //        PageData.Totals = totalCount;
            //    });
            //});
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

        /// <summary>
        /// 导出当前查询数据Excel 
        /// </summary>
        public void onExportItem()
        {
            if (data.Count <= 0)
            {
                WindowManagerExtension.ShowAckDialog(WindowManager, "导出确认", "无导出数据,请筛选数据!");
                return;
            }
            //手动解析Excel，获取数据
            var dialog = new SaveFileDialog
            {
                Title = "请选择路径",
                Filter = "Excel文件|*.xlsx",
                FileName = DateTime.Now.ToString("yyyy-MM-dd"),
            };
            if ((bool)dialog.ShowDialog())
            {
                string filepath = dialog.FileName;
                var message = $"手动导出数据源：{filepath}";
                LogEventMessage(message);
                Task.Run(() =>
                {
                    try
                    {
                        TaskUtil.GenerateAttachmentDataExport(data, filepath);
                        LogEventMessage($"手动导出数据源{filepath}：成功");
                        LogViewModel.AddLog(LogType.Info, "系统", $"手动导出数据源{filepath}：成功");
                    }
                    catch (Exception e)
                    {
                        WindowManagerExtension.ShowMessageDialog(WindowManager, e);
                        LogEventMessage($"手动导出数据源{filepath}异常：{e.Message}");
                        LogViewModel.AddLog(LogType.Error, "系统", $"手动导出数据源{filepath}异常：{e.Message}");
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

        #endregion
    }
}
