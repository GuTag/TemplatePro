using Panuon.WPF.UI;
using Panuon.WPF;
using PrintManager.Shared.Entity;
using PrintManager.Shared.Enums;
using PrintManager.Shared.Utils;
using PrintManager.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PrintManager.UI.Controls
{
    public class EditCanvas : Canvas
    {
        //public event Action CanvasChanged { add { AddHandler(CanvasChangedEvent, value); } remove { RemoveHandler(CanvasChangedEvent, value); } }
        //public static readonly RoutedEvent CanvasChangedEvent = EventManager.RegisterRoutedEvent("CanvasChanged", RoutingStrategy.Bubble, typeof(Action), typeof(EditCanvas));
        
        public ControlItem SelectedControl { get { return (ControlItem)GetValue(SelectedControlProperty); } set { SetValue(SelectedControlProperty, value); } }
        public static readonly DependencyProperty SelectedControlProperty = DependencyProperty.Register("SelectedControl", typeof(ControlItem), typeof(EditCanvas), new PropertyMetadata(null, null));

        public double ZoomPercent { get { return (double)GetValue(ZoomPercentProperty); } set { SetValue(ZoomPercentProperty, value); } }
        public static readonly DependencyProperty ZoomPercentProperty = DependencyProperty.Register("ZoomPercent", typeof(double), typeof(EditCanvas), new PropertyMetadata(1.0, DataChangedCallback));

        public PrintTemplate PrintItem { get { return (PrintTemplate)GetValue(PrintItemProperty); } set { SetValue(PrintItemProperty, value); } }
        public static readonly DependencyProperty PrintItemProperty = DependencyProperty.Register("PrintItem", typeof(PrintTemplate), typeof(EditCanvas), new PropertyMetadata(null, DataChangedCallback));

        private static void DataChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var canvas = d as EditCanvas;
            if(canvas.PrintItem!= null)
            {
                if(canvas.PrintItem.UpdateViewEvent == null) canvas.PrintItem.UpdateViewEvent += canvas.UpdateContentVisual;
            }
            canvas.UpdateContentVisual();
        }

        #region 变量

        //public PrintTemplate PrintItem { get; set; }

        private Point MouseDownPoint;
        private bool IsMouseDown = false;
        //private bool IsCtrlKeyDown = false;
        #endregion

        #region 构造函数
        /// <summary>
        /// 一个空的模板画布
        /// </summary>
        public EditCanvas()
        {
            this.Focusable = true;
            this.Focus();
            this.MouseWheel += MouseWheel_Event;
            this.MouseMove += MouseMove_Event;
            this.MouseLeave += MouseLeave_Event;
            this.MouseLeftButtonDown += MouseLeftButtonDown_Event;
            this.MouseLeftButtonUp += MouseLeftButtonUp_Event;
            this.KeyDown += PreviewKeyDown_Event;

            this.UpdateContentVisual();
        }

        #endregion

        #region 内部Visual操作函数

        private List<FrameworkElement> ContentChildren = new List<FrameworkElement>();
        private List<Visual> visuals = new List<Visual>();
        DrawingVisual lineVisual = null;
        DrawingVisual ContentVisual = null;
        DrawingVisual MouseVisual = null;
        DrawingVisual DragControlVisual = null;
        ControlItem copyControl = null;

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
            this.RemoveVisual(lineVisual);
            this.RemoveVisual(ContentVisual);
            this.RemoveVisual(MouseVisual);
            this.RemoveVisual(DragControlVisual);
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

        /// <summary>
        /// 更新绘画界面
        /// </summary>
        public void UpdateContentVisual()
        {
            if (PrintItem == null) return;
//#if DEBUG
//            System.Diagnostics.Debug.WriteLine("画了一次");
//#endif
            this.Height = PixelUtil.mmToPixel(PrintItem.Height, ZoomPercent);
            this.Width = PixelUtil.mmToPixel(PrintItem.Width, ZoomPercent);
            this.Background = CanvasHelpers.StringConvertBrush(PrintItem.Background);

            this.RemoveVisual(ContentVisual);
            ContentVisual = new DrawingVisual();
            DrawingContext dc = ContentVisual.RenderOpen();

            foreach (var control in PrintItem.ControlItems)
            {
                switch (control.ControlType)
                {
                    case ControlType.Text:
                        CanvasHelpers.DrawingText(this, dc, control,ZoomPercent);
                        break;
                    case ControlType.Image:
                        CanvasHelpers.DrawingImage(dc, control,ZoomPercent);
                        break;
                    case ControlType.BarCode:
                        CanvasHelpers.DrawingBarCode(dc, control, ZoomPercent);
                        break;
                    case ControlType.QRCode:
                        CanvasHelpers.DrawingQRCode(dc, control, ZoomPercent);
                        break;
                    //case ControlType.Line:
                    //    //Test 
                    //    CanvasHelpers.DrawingLine(dc, control, ZoomPercent);
                    //    break;
                    default:
                        break;
                }
                DrawingSelectedControl(dc, control);
            }
            dc.Close();
            this.AddVisual(ContentVisual);
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// 画鼠标移动矩形坐标
        /// </summary>
        /// <param name="mousePoint"></param>
        private void DrawingMouseVisual(Rect rect)
        {
            //清楚之前的画笔
            this.RemoveVisual(MouseVisual);
            MouseVisual = new DrawingVisual();
            DrawingContext dc = MouseVisual.RenderOpen();

            DoubleCollection doubles = new DoubleCollection { 15, 15 };
            DashStyle dashstyle = new DashStyle();
            dashstyle.Dashes = doubles;
            dashstyle.Offset = 0;
            Pen penAxis = new Pen(Brushes.Black, 0.5) { DashStyle = dashstyle };
            penAxis.Freeze();
            dc.DrawRectangle(Brushes.Transparent, penAxis, rect);

            dc.Close();
            this.AddVisual(MouseVisual);
        }

        private void DrawingDragControl()
        {
            //清楚之前的画笔
            this.RemoveVisual(DragControlVisual);
            if(copyControl == null) { return; }
            DragControlVisual = new DrawingVisual();
            DrawingContext dc = DragControlVisual.RenderOpen();
            dc.PushOpacity(0.5);
            switch (copyControl.ControlType)
            {
                case ControlType.Text:
                    CanvasHelpers.DrawingText(this, dc, copyControl, ZoomPercent);
                    break;
                case ControlType.Image:
                    CanvasHelpers.DrawingImage(dc, copyControl, ZoomPercent);
                    break;
                case ControlType.BarCode:
                    CanvasHelpers.DrawingBarCode(dc, copyControl, ZoomPercent);
                    break;
                case ControlType.QRCode:
                    CanvasHelpers.DrawingQRCode(dc, copyControl, ZoomPercent);
                    break;
                //case ControlType.Line:
                //    CanvasHelpers.DrawingLine(dc, copyControl, ZoomPercent);
                //    break;
                default:
                    break;
            }
            DrawingSelectedControl(dc, copyControl);


            dc.Close();
            this.AddVisual(DragControlVisual);
        }


        /// <summary>
        /// 选中时画线条
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="control"></param>
        private void DrawingSelectedControl(DrawingContext dc, ControlItem control)
        {
            if(control.IsSelected) 
            {
                var _lang = new System.Globalization.CultureInfo("zh-CHS", false);
                var _font = new Typeface("微软雅黑");
                var rect = CanvasHelpers.GetContorlZoomPercent(control,ZoomPercent);
                DoubleCollection doubles = new DoubleCollection { 5, 5 };
                DashStyle dashstyle = new DashStyle();
                dashstyle.Dashes = doubles;
                dashstyle.Offset = 0;
                var textBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3535FF"));
                Pen penAxis = new Pen(textBrush, 0.5) { DashStyle = dashstyle };
                penAxis.Freeze();
                var colorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FFFFFF"));
                dc.DrawRectangle(colorBrush, penAxis, rect);
                //penAxis.Thickness = 0.5;
                dc.DrawLine(penAxis, new Point(rect.X, 0), new Point(rect.X, rect.Y));
                dc.DrawLine(penAxis, new Point(0, rect.Y + rect.Height), new Point(rect.X, rect.Y + rect.Height));

                FormattedText textY = new FormattedText(string.Format("{0:0.00}", control.PosX), _lang, FlowDirection.LeftToRight, _font, 6 * ZoomPercent, textBrush, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                FormattedText textX = new FormattedText(string.Format("{0:0.00}", control.PosY + control.Height), _lang, FlowDirection.LeftToRight, _font, 6 * ZoomPercent, textBrush, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                dc.DrawText(textY, new Point(rect.X + 1, rect.Y - textX.Height));
                dc.DrawText(textX, new Point(rect.X - textY.Width, rect.Y + rect.Height));
            }
        }

        /// <summary>
        /// 判断鼠标是否在矩形内部
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool RectContainsPoint(Rect rect, Point point)
        {
            if (point.X >= rect.X && point.X <= rect.X + rect.Width && point.Y >= rect.Y && point.Y <= rect.Y + rect.Height)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region 事件
        /// <summary>
        /// 鼠标滚轮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseWheel_Event(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                ZoomPercent += 0.05;
            }
            else
            {
                ZoomPercent -= 0.05;
            }
            if(ZoomPercent < 0.5)ZoomPercent= 0.5;
            if(ZoomPercent > 10)ZoomPercent= 10;
            this.UpdateContentVisual();
            this.DrawingDragControl();

        }

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseMove_Event(object sender, MouseEventArgs e)
        {
            var offsetPoint = e.GetPosition(this);
            this.Focus();
            if (IsMouseDown)
            {
                var rect = new Rect()
                {
                    X = MouseDownPoint.X < offsetPoint.X ? MouseDownPoint.X : offsetPoint.X,
                    Y = MouseDownPoint.Y < offsetPoint.Y ? MouseDownPoint.Y : offsetPoint.Y,
                    Width = Math.Abs(offsetPoint.X - MouseDownPoint.X),
                    Height = Math.Abs(offsetPoint.Y - MouseDownPoint.Y),
                };

                if (SelectedControl!= null)
                {
                    var selectRect = CanvasHelpers.GetContorlZoomPercent(SelectedControl,ZoomPercent);
                    if(RectContainsPoint(selectRect, offsetPoint))
                    {
                        if (copyControl == null) copyControl = SelectedControl;
                        
                        var offset = offsetPoint - copyControl.MousePoint;
                        copyControl.PosX = PixelUtil.PixelTomm(offset.X, ZoomPercent);
                        copyControl.PosY = PixelUtil.PixelTomm(offset.Y, ZoomPercent);
                        this.DrawingDragControl();

                    }
                    else
                    {
                        this.DrawingMouseVisual(rect);
                        foreach (var control in PrintItem.ControlItems)
                        {
                            var controlRect = CanvasHelpers.GetContorlZoomPercent(control,ZoomPercent);
                            if (rect.Contains(controlRect))
                            {
                                control.IsSelected = true;
                            }
                            else if (control != SelectedControl)
                            {
                                control.IsSelected = false;
                            }
                        }
                        this.UpdateContentVisual();
                    }

                }
                else
                {
                    this.DrawingMouseVisual(rect);

                    foreach (var control in PrintItem.ControlItems)
                    {
                        var controlRect = CanvasHelpers.GetContorlZoomPercent(control,ZoomPercent);
                        if (rect.Contains(controlRect))
                        {
                            control.IsSelected = true;
                            SelectedControl = control;
                        }
                    }
                    this.UpdateContentVisual();
                }
                
            }
        }

        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeave_Event(object sender, MouseEventArgs e)
        {
            IsMouseDown = false;
            copyControl = null;
            this.RemoveVisual(MouseVisual);
            this.RemoveVisual(DragControlVisual);
        }
        
        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeftButtonDown_Event(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown= true;
            this.Focus();
            MouseDownPoint= e.GetPosition(this);
            
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && SelectedControl != null)
            {
                for (int i = PrintItem.ControlItems.Count - 1; i >= 0; i--)
                {
                    var rect = CanvasHelpers.GetContorlZoomPercent(PrintItem.ControlItems[i],ZoomPercent);
                    if (RectContainsPoint(rect, MouseDownPoint))
                    {
                        PrintItem.ControlItems[i].IsSelected = true;
                        break;
                    }
                }
            }
            else
            {
                SelectedControl = null;
                string selectID = "";
                for (int i = PrintItem.ControlItems.Count - 1; i >= 0; i--)
                {
                    var rect = CanvasHelpers.GetContorlZoomPercent(PrintItem.ControlItems[i],ZoomPercent);
                    if (RectContainsPoint(rect, MouseDownPoint))
                    {
                        selectID = PrintItem.ControlItems[i].ID;
                        break;
                    }
                }
                foreach (var control in PrintItem.ControlItems)
                {
                    control.IsSelected = false;
                    if (control.ID.Equals(selectID))
                    {
                        control.IsSelected = true;
                        var currentPos = new Point(PixelUtil.mmToPixel(control.PosX, ZoomPercent), PixelUtil.mmToPixel(control.PosY, ZoomPercent));
                        control.MousePoint = (Point)(MouseDownPoint - currentPos);
                        SelectedControl = control;
                    }
                }
            }
            this.UpdateContentVisual();
        }

        /// <summary>
        /// 鼠标抬起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeftButtonUp_Event(object sender, MouseButtonEventArgs e)
        {
            //if (IsMouseDown)
            //{
            //    this.UpdateContentVisual();
            //}
            if (copyControl != null)
            {
                this.UpdateContentVisual();
                SelectedControl = null;
                SelectedControl = copyControl;
            }
            copyControl = null;
            IsMouseDown = false;
            this.RemoveVisual(MouseVisual);
            this.RemoveVisual(DragControlVisual);
        }

        private void PreviewKeyDown_Event(object sender, KeyEventArgs e)
        {
            if(SelectedControl!= null)
            {
                switch(e.Key)
                {
                    case Key.Up:
                        foreach (var control in PrintItem.ControlItems)
                        {
                            if (control.IsSelected)
                            {
                                control.PosY -= 0.01;
                            }
                        }
                        break;
                    case Key.Down:
                        foreach (var control in PrintItem.ControlItems)
                        {
                            if (control.IsSelected)
                            {
                                control.PosY += 0.01;
                            }
                        }
                        break;
                    case Key.Left:
                        foreach (var control in PrintItem.ControlItems)
                        {
                            if (control.IsSelected)
                            {
                                control.PosX -= 0.01;
                            }
                        }
                        break;
                    case Key.Right:
                        foreach (var control in PrintItem.ControlItems)
                        {
                            if (control.IsSelected)
                            {
                                control.PosX += 0.01;
                            }
                        }
                        break;
                    case Key.Delete:
                        if(SelectedControl!= null)
                        {
                            PrintItem.ControlItems.Remove(SelectedControl);
                            SelectedControl= null;
                        }
                        break;
                    default: break;
                }
                e.Handled = true;
                this.UpdateContentVisual();
            }
        }

        #endregion

    }
}
