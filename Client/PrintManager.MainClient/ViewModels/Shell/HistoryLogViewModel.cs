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
    public class HistoryLogViewModel : ViewModelBase
    {
        public HistoryLogViewModel()
        {
            PageData = new PageModel();
            PageData.PageChangeEvent += PageChangeEvent;
            UpdateProductOrderList();
        }

        
        #region 属性数据
        public PageModel PageData { get => _pageData; set => Set(ref _pageData, value); }
        private PageModel _pageData;

        public DateTime StartDateTime { get => _startDateTime; set => Set(ref _startDateTime, value); }
        private DateTime _startDateTime = DateTime.Now;

        public DateTime EndDateTime { get => _endDateTime; set => Set(ref _endDateTime, value); }
        private DateTime _endDateTime = DateTime.Now;

        public int TypeComboxIndex { get => _typeComboxIndex; set => Set(ref _typeComboxIndex, value); }
        private int _typeComboxIndex = 0;
        public string SearchText { get => _searchText; set => Set(ref _searchText, value); }
        private string _searchText;
        public List<HistoryLogModel> HistoryLogList { get => _historyLogList; set => Set(ref _historyLogList, value); }
        private List<HistoryLogModel> _historyLogList = new List<HistoryLogModel>();

        #endregion

        #region 变量
        private int totalCount = 0;
        #endregion

        #region 方法
        //分页查询
        private void UpdateProductOrderList()
        {
            Task.Run(() =>
            {
                var data = HistoryLogBLL.GetPage(PageData.Page, PageData.PageSize, ref totalCount, StartDateTime, EndDateTime, TypeComboxIndex, SearchText);
                var datas = JsonConvert.DeserializeObject<List<HistoryLogModel>>(data);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (datas != null)
                    {
                        HistoryLogList = datas;
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

        #endregion

        #region 命令
        public void SearchCommand()
        {
            PageData.Page = 1;
            UpdateProductOrderList();
        }
        #endregion
    }
}
