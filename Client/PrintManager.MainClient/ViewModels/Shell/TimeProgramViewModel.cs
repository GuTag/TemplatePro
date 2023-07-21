using Caliburn.Micro;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PrintManager.MainClient.Components;
using PrintManager.MainClient.Conntrols;
using PrintManager.MainClient.Models;
using PrintManager.MainClient.Models.Extension;
using PrintManager.MainClient.ViewModels.Controls;
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
using System.Drawing.Printing;
using System.Dynamic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace PrintManager.MainClient.ViewModels.Shell
{
    public class TimeProgramViewModel : ViewModelBase
    {
        public TimeProgramViewModel()
        {
            EditCanvans.MouseDown += EditCanvans_MouseDown;

            List<TimeProgramOrder> order = TaskUtil.ReadOfTimeProgram();

            InitData();
        }



        #region 属性

        public EditCanvans EditCanvans { get => _editCanvans; set => Set(ref _editCanvans, value); }
        private EditCanvans _editCanvans = new EditCanvans();

        public List<TimeProgramTreeParent> ListViewOrderParent { get => _listViewOrderParent; set => Set(ref _listViewOrderParent, value); }
        private List<TimeProgramTreeParent> _listViewOrderParent = new List<TimeProgramTreeParent>();

        public List<TimeProgramTreeParent> SelectItem { get => _selectItem; set => Set(ref _selectItem, value); }
        private List<TimeProgramTreeParent> _selectItem = new List<TimeProgramTreeParent>();
        public TreeView TreeView1 { get => _treeView1; set => Set(ref _treeView1, value); }
        private TreeView _treeView1 = new TreeView();

        public string Title { get => _title; set => Set(ref _title, value); }
        private string _title;
        public int Time { get => _time; set => Set(ref _time, value); }
        private int _time;
        public string Description { get => _description; set => Set(ref _description, value); }
        private string _description;

        public List<TimeProgramOrder> TimeProgramOrder;

        #endregion

        #region 变量
        public const int sysRow = 32 + 1;
        public const int sysColum = 120 + 1;
        public bool[,] isActive = new bool[500, 500];
        //public int row = 10;
        //public int column = 20;
        public int row;
        public int column;
        public Rectangle[,] rectangles = new Rectangle[500, 500];
        public int rectangleHeight = 40;
        public int rectangleWidth = 50;
        public int lineTinckess = 1;

        #endregion

        #region 方法

        public void InitIsActuve()
        {
            for (int i = 0; i < 500; i++)
            {
                for (int ii = 0; ii < 500; ii++)
                {
                    isActive[i, ii] = false;
                }
            }
        }
        private void ShowLoadingView()
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
            var window = new LoadingViewModel();
            window.Loading();
            WindowManager.ShowWindow(window, null, settings);
        }
        private void InitData()
        {
            ListViewOrderParent = TaskUtil.GetListViewItem();
        }
        #endregion

        #region 事件


        public void onChangeListView(object obj)
        {
            TimeProgramTreeParent order = (TimeProgramTreeParent)obj;

            Loading(order.Index - 1);

        }

        public void onReferCanvans()
        {
            //drawing canva
            EditCanvans.Height = rectangleHeight * row;
            EditCanvans.Width = rectangleWidth * column;
            //EditCanvans.Height = 400;
            //EditCanvans.Width = 1000;
            EditCanvans.Background = new SolidColorBrush(Colors.White);

            //InitIsActuve();
        }

        /// <summary>
        /// 添加画布
        /// </summary>
        public void onReferAddCanvans(List<string> devices,int time)
        {
            //drawing line
            Line[] myrowdrawline = new Line[row];
            Line[] mycolumdrawline = new Line[column];
            TextBlock[] leftitem = new TextBlock[row];
            TextBlock[] topitem = new TextBlock[column];

            //drawing canvas line
            for (int i = 0; i < myrowdrawline.Length; i++)
            {
                //直线对象
                myrowdrawline[i] = new Line();
                myrowdrawline[i].Stroke = Brushes.Black;//外宽颜色，在直线里为线颜色
                myrowdrawline[i].StrokeThickness = lineTinckess;//线宽度
                myrowdrawline[i].X1 = EditCanvans.Width / column;
                myrowdrawline[i].Y1 = (EditCanvans.Height / row) * i;
                myrowdrawline[i].X2 = EditCanvans.Width;
                myrowdrawline[i].Y2 = (EditCanvans.Height / row) * i;

                if ((myrowdrawline[i].X1.Equals(0) || myrowdrawline[i].Y1.Equals(0)) == false)
                {
                    //将直线对象添加到canvas画布中
                    EditCanvans.AddVisual(myrowdrawline[i]);
                }


            }
            for (int i = 0; i < mycolumdrawline.Length; i++)
            {
                //直线对象
                mycolumdrawline[i] = new Line();
                mycolumdrawline[i].Stroke = Brushes.Black;//外宽颜色，在直线里为线颜色
                mycolumdrawline[i].StrokeThickness = lineTinckess;//线宽度
                mycolumdrawline[i].X1 = (EditCanvans.Width / column) * i;
                mycolumdrawline[i].Y1 = EditCanvans.Height / row;
                mycolumdrawline[i].X2 = (EditCanvans.Width / column) * i;
                mycolumdrawline[i].Y2 = EditCanvans.Height;

                if ((mycolumdrawline[i].X1.Equals(0) || mycolumdrawline[i].Y1.Equals(0)) == false)
                {
                    //将直线对象添加到canvas画布中
                    EditCanvans.AddVisual(mycolumdrawline[i]);
                }

            }

            //drawing canvans textblock
            for (int i = 0; i < row - 1; i++)
            {
                leftitem[i] = new TextBlock();
                leftitem[i].FontSize = 15;
                //leftitem[i].Text = $"item:{i}";
                leftitem[i].Text = devices.ToArray()[i];
                EditCanvans.SetLeft(leftitem[i], 0);
                EditCanvans.SetTop(leftitem[i], (40 * (i + 1)) + 10);
                EditCanvans.AddVisual(leftitem[i]);
            }
            for (int i = 0; i < column - 1; i++)
            {
                topitem[i] = new TextBlock();
                topitem[i].FontSize = 15;
                //topitem[i].Text = $"{i}.00";
                topitem[i].Text = $"{(i + 1) * (time/1000)}.0";
                EditCanvans.SetLeft(topitem[i], (50 * (i + 1)) + 10);
                EditCanvans.SetTop(topitem[i], 15);
                EditCanvans.AddVisual(topitem[i]);
            }

        }
        /// <summary>
        /// 移除画布
        /// </summary>
        public void onReferRemoveCanvans()
        {
            EditCanvans.DeletAllVisual();
            //InitIsActuve();
        }


        /// <summary>
        /// 点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditCanvans_MouseDown(object sender, MouseButtonEventArgs e)
        {

            /*
             * Height = 40
             * Width  = 50
             * 
             * rowHeight = 40
             * columWidth = 50
             * 
             * X  = 50
             * Y  = 40
             */

            int height = rectangleHeight;
            int width = rectangleWidth;
            int clickRow = 0;
            int clickColum = 0;
            const int margin = 1;


            //谁引用的转换成谁
            var c = sender as EditCanvans;
            Point point = e.GetPosition(c);
            Console.WriteLine($"X:{point.X} Y:{point.Y}");

            for (int i = 1; i < column; i++)
            {
                if (point.X >= (width * i))
                {
                    for (int j = 1; j < row; j++)
                    {
                        if (point.Y >= (height * j))
                        {
                            clickRow = j;
                            clickColum = i;
                        }
                    }
                }
            }

            Console.WriteLine($"{clickRow}:{clickColum}");


            rectangles[clickRow, clickColum] = new Rectangle();
            rectangles[clickRow, clickColum].Width = rectangleWidth - margin;
            rectangles[clickRow, clickColum].Height = rectangleHeight - margin;
            EditCanvans.SetLeft(rectangles[clickRow, clickColum], width * clickColum);
            EditCanvans.SetTop(rectangles[clickRow, clickColum], height * clickRow);
            EditCanvans.AddVisual(rectangles[clickRow, clickColum]);

            isActive[clickRow, clickColum] = !isActive[clickRow, clickColum];


            string test = string.Empty;
            for (int i = 0; i < 32; i++)
            {
                string str =  TaskUtil.BoolConvertStr(isActive[i, clickColum]);

                test += str;
            }

            Console.WriteLine(test);

            //0x0 排除掉
            if (clickRow == 0 || clickColum == 0) return;

            if (isActive[clickRow, clickColum])
            {
                rectangles[clickRow, clickColum].Fill = Brushes.Green;
            }
            else
            {
                rectangles[clickRow, clickColum].Fill = Brushes.White;
            }
        }


        public void Loading(int index)
        {
            InitIsActuve();
            onReferRemoveCanvans();

            List<TimeProgramOrder> orders = new List<TimeProgramOrder>();
            orders = TaskUtil.ReadOfTimeProgram();
            TimeProgramOrder = orders;

            Title = orders[index].Title;
            Time = orders[index].Time;
            Description = orders[index].Description;

            row = orders[index].Device.ToArray().Length + 1;
            column = orders[index].ControlWord.ToArray().Length + 1;

            onReferCanvans();
            onReferAddCanvans(orders[index].Device, orders[index].Time);
            LoadCavans(orders[index].ControlWord);

        }
        public void LoadCavans(List<string> bit)
        {
            int height = rectangleHeight;
            int width = rectangleWidth;
            const int margin = 1;
            int row = 0;
            int colum = 0;

            foreach (var items in bit)
            {
                int columIndex = bit.IndexOf(items);

                for (int rowIndex = 0; rowIndex < items.Length; rowIndex++)
                {
                    rectangles[columIndex, rowIndex] = new Rectangle();

                    rectangles[columIndex, rowIndex].Width = rectangleWidth - margin;
                    rectangles[columIndex, rowIndex].Height = rectangleHeight - margin;


                    if (items.Substring(rowIndex, 1).Contains("1"))
                    {
                        rectangles[columIndex, rowIndex].Fill = Brushes.Green; 
                        isActive[columIndex, rowIndex] = true;
                    }

                    if (items.Substring(rowIndex, 1).Contains("0")) 
                    {
                        rectangles[columIndex, rowIndex].Fill = Brushes.White;
                        isActive[columIndex, rowIndex] = false;
                    } 

                    EditCanvans.SetLeft(rectangles[columIndex, rowIndex], width * (columIndex+1));
                    EditCanvans.SetTop(rectangles[columIndex, rowIndex], height * (rowIndex+1));

                    EditCanvans.AddVisual(rectangles[columIndex, rowIndex]);
                }

            }

            Console.WriteLine(isActive);
        }

        public void Download()
        {
            Task.Run(() =>
            {
                WindowManagerExtension.ShowLoading(WindowManager);
            });

            Console.WriteLine(isActive);
            List<string> cw = new List<string>();
            List<string> tempCW = new List<string>();
            string tempString = string.Empty;

            for (int i = 1; i <= 120; i++)
            {
                tempString = string.Empty;
                for (int ii = 1; ii <= 32; ii++)
                {
                    tempString += TaskUtil.BoolConvertStr(isActive[ii, i]);
                }
                cw.Add(tempString);
            }

            Console.WriteLine(cw);


            List<string> _16Bit = new List<string>();
            foreach (var item in cw)
            {
                _16Bit.Add(TaskUtil._2ConvertTo16(item));
            }

            TimeProgramOrder timeProgramOrder = new TimeProgramOrder();

            timeProgramOrder.Title = Title;
            timeProgramOrder.Description = Description;
            timeProgramOrder.Time = Time;

            timeProgramOrder.Device = TimeProgramOrder[0].Device;
            timeProgramOrder.ControlWord = _16Bit;

            bool isOK = TaskUtil.WriteOfTimeProgram(timeProgramOrder);

            Console.WriteLine(_16Bit);

        }



        #endregion

        #region 命令

        #endregion
    }
}
