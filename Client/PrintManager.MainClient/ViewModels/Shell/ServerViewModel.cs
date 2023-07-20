using Caliburn.Micro;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PrintManager.MainClient.Components;
using PrintManager.MainClient.Models;
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
using System.Dynamic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PrintManager.MainClient.ViewModels.Shell
{
    public class ServerViewModel : ViewModelBase
    {
        public ServerViewModel()
        {
            
            UpdateItemSource();
            //UpdateItemSourceDiagram();
            ChartTimerConfig();
        }





        #region 属性

        public int TotalOnlineClient { get => _totalOnlineClient; set => Set(ref _totalOnlineClient, value); }
        private int _totalOnlineClient;
        public int TotalBoolNbr { get => _totalBoolNbr; set => Set(ref _totalBoolNbr, value); }
        private int _totalBoolNbr;
        public int TotalAnalogNbr { get => _totalAnalogNbr; set => Set(ref _totalAnalogNbr, value); }
        private int _totalAnalogNbr;
        public int TotalLanguageNbr { get => _totalLanguageNbr; set => Set(ref _totalLanguageNbr, value); }
        private int _totalLanguageNbr;

        public ObservableCollection<PieSerise> ItemsSourcePie { get => _itemsSourcePie; set => Set(ref _itemsSourcePie, value); }
        private ObservableCollection<PieSerise> _itemsSourcePie;

        public ObservableCollection<DiagramSerise> ItemsSourceDiagram { get => _itemsSourceDiagram; set => Set(ref _itemsSourceDiagram, value); }
        private ObservableCollection<DiagramSerise> _itemsSourceDiagram;

        public ObservableCollection<ChartLogModel> ItemSouceLine { get => _itemSouceLine; set => Set(ref _itemSouceLine, value); }
        private ObservableCollection<ChartLogModel> _itemSouceLine = new ObservableCollection<ChartLogModel>();
        public ObservableCollection<OnlineDevItemLog> ItemSouceLog { get => _itemSouceLog; set => Set(ref _itemSouceLog, value); }
        private ObservableCollection<OnlineDevItemLog> _itemSouceLog = new ObservableCollection<OnlineDevItemLog>();

        
        #endregion

        #region 变量
        private System.Timers.Timer ReferChart = new System.Timers.Timer();

        #endregion

        #region 方法
        public void AddLog(string line, string mo)
        {
            var lineName = "";
            var item = JsonConvert.DeserializeObject<ProductOrderModel>(ProductOrderBLL.GetItemOfMO(mo));
            if (item == null) return;

            var ConfigClent1 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "ClientIP", "1");
            var ConfigClent2 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "ClientIP", "2");
            var ConfigClent3 = IniUtil.IniReadvalue(Environments.ConfigFilePath, "ClientIP", "3");

            if (line.StartsWith(ConfigClent1))
            {
                lineName = "一线";
            }
            else if (line.StartsWith(ConfigClent2))
            {
                lineName = "二线";
            }
            else if (line.StartsWith(ConfigClent3))
            {
                lineName = "三线";
            }

            var _prodType = "";
            switch (item.ProductOrderType)
            {
                case OrderType.F89:
                    _prodType = "F89";
                    break;
                case OrderType.RPX:
                    _prodType = "RPX";
                    break;
                case OrderType.Bundle:
                    if (item.ItemNo.StartsWith("RPX"))
                    {
                        _prodType = "RPX";
                    }
                    else
                    {
                        _prodType = "F89";
                    }        
                    break;
                default:
                    break;
            }

            var model = new ProductOrderLog() {
                AddTime = DateTime.Now,
                ComplatedNum = item.ComplatedNum,
                RequestNum = item.RequestNum,
                Line = lineName,
                ProdType = _prodType,
                Desc = item.Desc,
                ItemNo = item.ItemNo,                   
                MO = mo, 
                Client = line,
                IsOK = true 
            };

            // 时间、需求数量、生产数量、生产线、MO、状态  ...
            ProductOrderLogBLL.AddOfUpdata(model);


        }
        public void UpdateItemSource()
        {
            Task.Run(() =>
            {
                try
                {
                    List<PieSerise> pieSerises = new List<PieSerise>();
                    List<DiagramSerise> diagramSerises = new List<DiagramSerise>();
                    int TotalNbr = TotalBoolNbr + TotalAnalogNbr + TotalLanguageNbr;

                    pieSerises.Add(new PieSerise()
                    {
                        Title = "离散量",
                        Num = TotalBoolNbr,
                        //Percentage = (TotalBoolNbr / TotalNbr) * 100,
                        Percentage = 0.5,
                        PieColor = "#FFC000"
                    }); 
                    pieSerises.Add(new PieSerise()
                    {
                        Title = "模拟量",
                        Num = TotalAnalogNbr,
                        //Percentage = (TotalAnalogNbr / TotalNbr) * 100,
                        Percentage = 0.4,
                        PieColor = "#FFB000"
                    });
                    pieSerises.Add(new PieSerise()
                    {
                        Title = "文本",
                        Num = TotalLanguageNbr,
                        //Percentage = (TotalLanguageNbr / TotalNbr) * 100,
                        Percentage = 0.1,
                        PieColor = "#FFA000"
                    });

                    diagramSerises.Add(new DiagramSerise() {
                        Title = "离散量",
                        RequestNum = TotalBoolNbr,
                        DiagramColor = "#FFCC00"
                    });
                    diagramSerises.Add(new DiagramSerise()
                    {
                        Title = "模拟量",
                        RequestNum = TotalAnalogNbr,
                        DiagramColor = "#FFBB00"
                    });
                    diagramSerises.Add(new DiagramSerise()
                    {
                        Title = "文本",
                        RequestNum = TotalLanguageNbr,
                        DiagramColor = "#FFAA00"
                    });

                    App.Current.Dispatcher.Invoke(new System.Action(() =>
                    {
                        ItemsSourceDiagram = new ObservableCollection<DiagramSerise>(diagramSerises);
                        ItemsSourcePie = new ObservableCollection<PieSerise>(pieSerises);
                    }));

                    //生产记录操作
                    Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        if (GlobalData.m_OpcUaClient.Connected)
                        {
                            ItemSouceLog.Clear();
                            ItemSouceLog.Add(new OnlineDevItemLog()
                            {
                                ClientName = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "ClientName"),
                                TotalNodeNbr = TotalAnalogNbr + TotalBoolNbr,
                                IsStatus = GlobalData.m_OpcUaClient.Connected,
                                OPCAdr = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "OPCAdr"),
                                IssueWebAdr = IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "ServerAdr"),
                            });
                        }
                    }));
                }
                catch (Exception e)
                {
                    Logger.Error(e, "数据统计查询错误");
                }
            });
        }

        public void UpdateItemSourceLine(int[] productNum) 
        {
            if (productNum.Length != 4) return;
            List<ChartLogModel> list = new List<ChartLogModel>();
            for (int i = 1; i <= 3; i++)
            {
                var (iphost, mo) = LineUtil.GetLineMO(i);
                if (!string.IsNullOrEmpty(iphost))
                {
                    var model = new ChartLogModel()
                    {
                        Line = $"{i}线",
                    };

                    if (!string.IsNullOrEmpty(mo))
                    {
                        model.ProducNum = productNum[i];
                        var item = JsonConvert.DeserializeObject<ProductOrderModel>(ProductOrderBLL.GetItemOfMO(mo));
                        if (item != null)
                        {
                            model.MO = mo;
                            model.RequestNum = item.RequestNum;
                            model.ComplatedNum = item.ComplatedNum;
                            model.IsOK = item.IsOK;
                        }

                    }
                    list.Add(model);
                }
            }

            Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                ItemSouceLine = new ObservableCollection<ChartLogModel>(list);
            }));
        }
        #endregion

        #region 事件
        private void ChartTimerConfig()
        {
            ReferChart.Interval = int.Parse(IniUtil.IniReadvalue(Environments.ConfigFilePath, "Config", "RefreshTime"));
            ReferChart.Enabled = true;
            ReferChart.AutoReset = true;
            ReferChart.Start();
            ReferChart.Elapsed += ReferChart_Elapsed;
        }

        private void ReferChart_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateTopLine();
            UpdateItemSource();
        }

        public void UpdateTopLine()
        {

            //Bool Nbr
            TotalBoolNbr = ProductOrderBLL.GetToAll();

            //Analog Nbr
            TotalAnalogNbr = AnalogBLL.GetToAll();

            //Language Nbr
            TotalLanguageNbr = LanguageTextBLL.GetToAll();

            //Online client
            TotalOnlineClient = TotalBoolNbr + TotalAnalogNbr;

        }
        #endregion

        #region 命令

        #endregion
    }
}
