using Caliburn.Micro;
using Microsoft.Win32;
using Newtonsoft.Json;
using PrintManager.MainClient.Components;
using PrintManager.MainClient.Models;
using PrintManager.Sql.BLL;
using PrintManager.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PrintManager.UI.Controls.PagePicker;
using System.Windows.Input;
using System.Windows;
using System.Dynamic;

namespace PrintManager.MainClient.ViewModels.Shell
{
    public class LanguageViewModel : ViewModelBase
    {
        public LanguageViewModel()
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
        public LanguageModel SelectedOrder { get => _selectedOrder; set => Set(ref _selectedOrder, value); }
        private LanguageModel _selectedOrder;
        public ObservableCollection<LanguageModel> ProductOrderList { get => _productOrderList; set => Set(ref _productOrderList, value); }
        private ObservableCollection<LanguageModel> _productOrderList = new ObservableCollection<LanguageModel>();

        #endregion

        #region 变量
        private int totalCount = 0;
        public bool IsAdd { get; set; } = false;
        public bool IsUpdate { get; set; } = false;
        #endregion

        #region 方法
        //分页查询
        private void UpdateProductOrderList()
        {
            Task.Run(() =>
            {

                var data = LanguageTextBLL.GetPage(PageData.Page, PageData.PageSize, ref totalCount, StartDateTime, EndDateTime, SearchText);
                var datas = JsonConvert.DeserializeObject<List<LanguageModel>>(data);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (datas != null)
                    {
                        ProductOrderList = new ObservableCollection<LanguageModel>(datas);
                    }
                    PageData.Totals = totalCount;
                });
            });
        }

        #endregion

        #region 事件

        private void PageChangeEvent()
        {
            UpdateProductOrderList();
        }
        private void ShowParameterView(bool IsUpdate)
        {
            string _tag = "参数设置";
            dynamic settings = new ExpandoObject();
            settings.Height = 250;
            settings.Width = 700;
            settings.SizeToContent = SizeToContent.Manual;
            settings.Topmost = false;
            //settings.Owner = Application.Current.MainWindow;
            settings.Title = _tag;
            settings.Owner = null;
            var window = new ParUpdateTemp3ViewModel();
            window.ShowParView(SelectedOrder, IsUpdate);
            WindowManager.ShowWindow(window, null, settings);
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
                        TaskUtil.RequestExcelLanguageTextData(filepath);
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

        public void onImportPDFCommand()
        {

            //手动解析Excel，获取数据
            var dialog = new OpenFileDialog
            {
                Title = "选择PDF文件",
                Filter = "PDF文件|*.pdf;",

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
                        TaskUtil.RequestPDFData(filepath);
                        LogEventMessage($"手动导入数据源{filepath}：成功");
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
            IsUpdate = true;
            if (GlobalData.Instance.IsLogin)
            {
                if (SelectedOrder != null)
                {
                    ShowParameterView(IsUpdate);
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
        }

        public void onDeleteCommand()
        {

            if (SelectedOrder != null)
            {
                if (WindowManagerExtension.ShowAckDialog(WindowManager, "删除确认", "是否删除该数据！") == true)
                {
                    LanguageTextBLL.Delete(SelectedOrder.Index);
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
