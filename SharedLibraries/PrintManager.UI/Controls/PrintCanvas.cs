using Caliburn.Micro;
using Panuon.WPF;
using PrintManager.Shared;
using PrintManager.Shared.Entity;
using PrintManager.Shared.Enums;
using PrintManager.Shared.Helpers;
using PrintManager.Shared.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing.Common;
using ZXing;
using static PrintManager.Shared.Entity.OrderEntity;
using ZXing.QrCode;
using PrintManager.UI.Helpers;

namespace PrintManager.UI.Controls
{
    public class PrintCanvas : Canvas
    {
        public double ZoomPercent { get { return (double)GetValue(ZoomPercentProperty); } set { SetValue(ZoomPercentProperty, value); } }
        public static readonly DependencyProperty ZoomPercentProperty = DependencyProperty.Register("ZoomPercent", typeof(double), typeof(PrintCanvas), new PropertyMetadata(3.0));


        #region 变量
        public CanvasData PrintData { get; set; }= new CanvasData();

        public string Filepath;
       
        #endregion

        #region 构造函数
        /// <summary>
        /// 没有数据的标签模板
        /// </summary>
        /// <param name="filepath"></param>
        public PrintCanvas(string filepath)
        {
            Init(filepath);
            PrintData.PrintTemplate = OrderHelpers.GetPrintTemplate(filepath);
            UpdateView();
        }
        /// <summary>
        /// 有详细数据的标签模板
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="itemNo"></param>
        /// <param name="mo"></param>
        /// <param name="complateNum"></param>
        /// <param name="soItm"></param>
        /// <param name="mltNo"></param>
        public PrintCanvas(string filepath, string itemNo, string mo, int complateNum, string soItm, string mltNo, OrderType orderType, string desc, string newItemNo)
        {
            Init(filepath);

            PrintData.PrintTemplate = OrderHelpers.GetPrintTemplate(filepath);
            PrintData.OrderDetail = new OrderDetail(PrintData.PrintTemplate.Type, itemNo, mo, complateNum, soItm, mltNo, orderType,desc,newItemNo);
            UpdateView();
        }

        public PrintCanvas(CanvasData printData, double ZoomPercent = 2.0)
        {
            this.ZoomPercent= ZoomPercent;
            UpdateCanvasView(printData);
        }
        #endregion

        #region 内部Visual操作函数

        private List<Visual> visuals = new List<Visual>();
        DrawingVisual ContentVisual = null;

        //获取Visual的个数
        protected override int VisualChildrenCount
        {
            get { return visuals.Count; }
        }

        //获取Visual
        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        //添加Visual
        public void AddVisual(Visual visual)
        {
            visuals.Add(visual);
            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }

        //删除Visual
        public void RemoveVisual(Visual visual)
        {
            if (visual != null)
            {
                visuals.Remove(visual);
                base.RemoveVisualChild(visual);
                base.RemoveLogicalChild(visual);
            }
        }

        //删除所有Visual
        public void RemoveAllVisual()
        {
            this.RemoveVisual(ContentVisual);
            this.InvalidateVisual();
        }

        //命中测试
        public DrawingVisual GetVisual(Point point)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, point);
            return hitResult.VisualHit as DrawingVisual;
        }

        #endregion


        #region 公有方法


        public void UpdateCanvasView() 
        { 
            if(!string.IsNullOrEmpty(Filepath))
            {
                PrintData.PrintTemplate = OrderHelpers.GetPrintTemplate(Filepath);
                UpdateView();
            }
        }

        public void UpdateCanvasView(CanvasData printData)
        {
            PrintData = printData;
            UpdateView();
        }

        #endregion

        #region 私有方法
        private void Init(string filepath)
        {
            this.Filepath = filepath;
            this.MouseWheel+= MouseWheel_Event;
        }

        private void UpdateView()
        {
            if (PrintData.PrintTemplate == null || PrintData.PrintTemplate?.ControlItems == null || PrintData.PrintTemplate?.ControlItems?.Count == 0) return;

            this.Height = PixelUtil.mmToPixel(PrintData.PrintTemplate.Height, ZoomPercent);
            this.Width = PixelUtil.mmToPixel(PrintData.PrintTemplate.Width, ZoomPercent);
            this.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(PrintData.PrintTemplate.Background));

            this.Children.Clear();
            this.RemoveVisual(ContentVisual);
            ContentVisual = new DrawingVisual();
            DrawingContext dc = ContentVisual.RenderOpen();
            foreach (var control in PrintData.PrintTemplate.ControlItems)
            {
                try
                {
                    switch (control.ControlType)
                    {
                        case ControlType.Text:
                            this.DrawingText(this, dc, control, ZoomPercent);
                            break;
                        case ControlType.Image:
                            this.DrawingImage(dc, control, ZoomPercent);
                            break;
                        case ControlType.BarCode:
                            CanvasHelpers.DrawingBarCode(dc, control, ZoomPercent);
                            break;
                        case ControlType.QRCode:
                            CanvasHelpers.DrawingQRCode(dc, control, ZoomPercent);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    ;
                }
            }
            dc.Close();
            this.AddVisual(ContentVisual);
        }

        private void DrawingText(UIElement parent, DrawingContext dc, ControlItem control, double ZoomPercent)
        {
            var _lang = new System.Globalization.CultureInfo("en-us", false);
            var _font = new Typeface(FontHelpers.GetFontFamily(control.FontFamily), FontHelpers.GetFontStyle(control.FontStyle), FontHelpers.GetFontWeight(control.FontWeight), FontStretches.Normal);
            SolidColorBrush _foreground = Brushes.Black;
            string _text = control.DisplayName;
            if (control.IsAssociation)
            {
                //_foreground = CanvasHelpers.StringConvertBrush("#ED4BE1");
                if (PrintData.OrderDetail != null)
                {
                    //如果不为空，怎展示数据
                    string varValue = (string)PrintData.OrderDetail.GetType().GetProperty(control.VarName).GetValue(PrintData.OrderDetail, null);
                    _text = varValue;
                }
                else
                {
                    _text = control.DisplayName;
                }
            }

            FormattedText formattedText = new FormattedText(_text, _lang, FlowDirection.LeftToRight, _font, control.FontSize * ZoomPercent, _foreground, VisualTreeHelper.GetDpi(parent).PixelsPerDip);
            var contorlWidth = PixelUtil.mmToPixel(control.Width, ZoomPercent);
            
            while (formattedText.Width > contorlWidth)
            {
                control.FontSize -= 0.1;
                formattedText.SetFontSize(control.FontSize * ZoomPercent);
            }
            var rect = CanvasHelpers.GetContorlZoomPercent(control, ZoomPercent);
            dc.PushTransform(new ScaleTransform(control.WidthFactor, 1));// { CenterX = text.Width / 2 });
            //dc.DrawText(formattedText, new Point(rect.X / control.WidthFactor, rect.Y));
            //if (PrintData.PrintTemplate.Type.Equals("RPX"))
            //{
            //    var geometry = formattedText.BuildGeometry(new Point(rect.X / control.WidthFactor, rect.Y));
            //    dc.DrawGeometry(_foreground, new Pen(_foreground, 0.5), geometry);
            //    //if (control.FontWeight == "Bold")
            //    //{
            //    //    dc.DrawGeometry(_foreground, new Pen(_foreground, 0.5), geometry);
            //    //}
            //    //else
            //    //{
            //    //    dc.DrawGeometry(_foreground, new Pen(_foreground, 1), geometry);
            //    //}
            //}
            //else
            //{
                if (control.FontWeight == "Bold")
                {
                    dc.DrawText(formattedText, new Point(rect.X / control.WidthFactor, rect.Y));
                }
                else
                {
                    var geometry = formattedText.BuildGeometry(new Point(rect.X / control.WidthFactor, rect.Y));
                    dc.DrawGeometry(_foreground, new Pen(_foreground, 0.5), geometry);
                }
            //}
            dc.Pop();
        }

        public void DrawingImage(DrawingContext dc, ControlItem control, double ZoomPercent)
        {
            if (!string.IsNullOrEmpty(control.ImageData))
            {
                double width, height;
                var image = ImageHelpers.StringConvertBitmapImage(control.ImageData);
                width = image.Width * ZoomPercent;
                height = image.Height * ZoomPercent;
                if (control.Width != 0 && control.Height != 0)
                {
                    width = PixelUtil.mmToPixel(control.Width, ZoomPercent);
                    height = PixelUtil.mmToPixel(control.Height, ZoomPercent);
                }
                control.Width = PixelUtil.PixelTomm(width, ZoomPercent);
                control.Height = PixelUtil.PixelTomm(height, ZoomPercent);

                var rect = CanvasHelpers.GetContorlZoomPercent(control, ZoomPercent);
                dc.DrawImage(image, rect);
                
            }
        }
        #endregion

        #region 事件
        private void MouseWheel_Event(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                ZoomPercent = ZoomPercent + 0.05;
            }
            else
            {
                ZoomPercent = ZoomPercent - 0.05;
            }
            if(ZoomPercent < 0.5)ZoomPercent= 0.5;
            if(ZoomPercent > 5)ZoomPercent= 5;
            
            this.UpdateView();
        }
        #endregion




    }
}
