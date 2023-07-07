using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PrintManager.UI.Controls
{
    [TemplatePart(Name = "trackLine", Type = typeof(Line))]
    public class BaseControl : Control
    {
        //public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(string), typeof(BaseContorl));
        //public static readonly DependencyProperty ContorlTypeProperty = DependencyProperty.Register("Content", typeof(ControlType), typeof(BaseControl));
        public static readonly DependencyProperty TopProperty = DependencyProperty.Register("Top", typeof(double), typeof(BaseControl));
        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register("Left", typeof(double), typeof(BaseControl));
        public static readonly DependencyProperty IsAssociationProperty = DependencyProperty.Register("IsAssociation", typeof(bool), typeof(BaseControl));

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(BaseControl));


        /// <summary>
        /// 定义静态构造函数
        /// </summary>
        public BaseControl(Canvas parent)
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseContorl), new FrameworkPropertyMetadata(typeof(BaseContorl)));
            CanvasParent = parent;
            CanvasParent.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;

            this.PreviewMouseLeftButtonDown += Contorl_MouseLeftButtonDown;
            this.PreviewMouseLeftButtonUp += Contorl_MouseLeftButtonUp;
            this.MouseMove += Contorl_MouseMove;
            this.MouseLeave += Contorl_MouseLeave;

            
        }

        #region 属性
        /// <summary>
        /// 屏幕分辨率
        /// </summary>
        //public string Content
        //{
        //    get
        //    {
        //        return ((string)GetValue(ContentProperty));
        //    }
        //    set
        //    {
        //        SetValue(ContentProperty, value);
        //    }
        //}

       

        /// <summary>
        /// 显示的类型
        /// </summary>
        //public ControlType ControlType
        //{
        //    get
        //    {
        //        return ((ControlType)GetValue(ContorlTypeProperty));
        //    }
        //    set
        //    {
        //        SetValue(ContorlTypeProperty, value);
        //    }
        //}

        public double PosX
        {
            get
            {
                return ((double)GetValue(LeftProperty));
            }
            set
            {
                SetValue(LeftProperty, value);
            }
        }

        public double PosY
        {
            get
            {
                return ((double)GetValue(TopProperty));
            }
            set
            {
                SetValue(TopProperty, value);
            }
        }

        public bool IsAssociation
        {
            get
            {
                return ((bool)GetValue(IsAssociationProperty));
            }
            set
            {
                SetValue(IsAssociationProperty, value);
            }
        }

        public bool IsSelected
        {
            get
            {
                return ((bool)GetValue(IsSelectedProperty));
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

 
        #endregion

        #region 常量
        public const double _inchCm = 2.54; //一英寸为2.54cm


        #endregion


        #region 变量
        public Canvas CanvasParent;

        private bool _IsMouseDown = false;
        private Point _mouseDownPoint;

        #endregion

        #region 事件
        //点击画布时取消控件选择
        private void Canvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _IsMouseDown = false;
            IsSelected= false;
        }
        private void Contorl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _IsMouseDown= true;
            _mouseDownPoint = e.GetPosition(this);
            IsSelected = true;
            var c = sender as FrameworkElement;
            c.CaptureMouse();
        }

        private void Contorl_MouseLeftButtonUp(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _IsMouseDown = false;
            var c = sender as FrameworkElement;
            c.ReleaseMouseCapture();
        }

        private void Contorl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_IsMouseDown)
            {
                var c = sender as FrameworkElement;
                var _mousePoint = e.GetPosition(this);
                var dp = _mousePoint - _mouseDownPoint;
                this.PosX = PosX + dp.X;
                this.PosY = PosY + dp.Y;
                UpdatePosition();
            }
        }
        private void Contorl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        #endregion

        /// <summary>
        /// 更新控件位置
        /// </summary>
        private void UpdatePosition()
        {

        }


        /// <summary>
        /// 重画标尺数据
        /// </summary>
        /// <param name="drawingContext"></param>
        //protected override void OnRender(DrawingContext drawingContext)
        //{
            
        //}

        //private void DrawLine(DrawingContext drawingContext, double currentPoint, Position currentPosition, Pen pen, int type)
        //{
        //    double linePercent = 0d;
            
        //}

       
        private FormattedText GetFormattedText(string text)
        {
            var _lang = new System.Globalization.CultureInfo("zh-CHS", false);
            var _font = new Typeface("Microsoft YaHei");
            return new FormattedText(text, _lang, FlowDirection.LeftToRight, _font, 12, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
        }

        
        /// <summary>
        /// 初始化获取屏幕的DPI
        /// </summary>
        private void Initialize()
        {
            
        }

        


     


       


        

        
    }

    //public enum ControlType
    //{
    //    Text,
    //    Image,
    //    BarCode,
    //    QRCode
    //}



    
}
