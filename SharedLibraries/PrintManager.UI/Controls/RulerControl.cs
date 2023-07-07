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
    public class RulerControl : Control
    {
        public static readonly DependencyProperty DpiProperty = DependencyProperty.Register("Dpi", typeof(Dpi), typeof(RulerControl));
        public static readonly DependencyProperty DisplayTypeProperty = DependencyProperty.Register("DisplayType", typeof(RulerDisplayType), typeof(RulerControl));
        public static readonly DependencyProperty DisplayUnitProperty = DependencyProperty.Register("DisplayUnit", typeof(RulerDisplayUnit), typeof(RulerControl));
        public static readonly DependencyProperty ZeroPointProperty = DependencyProperty.Register("ZeroPoint", typeof(double), typeof(RulerControl));

        public static readonly DependencyProperty DisplayPercentProperty
            = DependencyProperty.Register("DisplayPercent", typeof(double), typeof(RulerControl), new FrameworkPropertyMetadata((double)1, DisplayPercentChangeCallBack));
        //, new FrameworkPropertyMetadata((double)1, DisplayPercentChangeCallBack)
        private static void DisplayPercentChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rulur = d as RulerControl;
            rulur.DisplayPercent = (double)e.NewValue;
        }
        /// <summary>
        /// 定义静态构造函数
        /// </summary>
        static RulerControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RulerControl), new FrameworkPropertyMetadata(typeof(RulerControl)));
        }

        #region 属性
        /// <summary>
        /// 屏幕分辨率
        /// </summary>
        public Dpi Dpi
        {
            get
            {
                return ((Dpi)GetValue(DpiProperty));
            }
            set
            {
                SetValue(DpiProperty, value);
            }
        }

        /// <summary>
        /// 设置0点从哪里开始
        /// </summary>
        public double ZeroPoint
        {
            get
            {
                return ((double)GetValue(ZeroPointProperty));
            }
            set
            {
                SetValue(ZeroPointProperty, value);
                InvalidateVisual();
            }
        }

        /// <summary>
        /// 显示的比率（目前支持0-1的选项）
        /// </summary>
        public double DisplayPercent
        {
            get
            {
                return ((double)GetValue(DisplayPercentProperty));
            }
            set
            {
                //if (value > 1)
                //{
                //    value = 1;
                //}
                SetValue(DisplayPercentProperty, value);
                InvalidateVisual();
            }
        }

        /// <summary>
        /// 显示的类型：枚举类（支持横向或者竖向）
        /// </summary>
        public RulerDisplayType DisplayType
        {
            get
            {
                return ((RulerDisplayType)GetValue(DisplayTypeProperty));
            }
            set
            {
                SetValue(DisplayTypeProperty, value);
            }
        }

        /// <summary>
        /// 显示的单位：cm和pixel
        /// </summary>
        public RulerDisplayUnit DisplayUnit
        {
            get
            {
                return ((RulerDisplayUnit)GetValue(DisplayUnitProperty));
            }
            set
            {
                SetValue(DisplayUnitProperty, value);
            }
        }
        #endregion

        #region 常量
        public const double _inchCm = 2.54; //一英寸为2.54cm
        private const int _p100StepSpanPixel = 100;
        private const int _p100StepSpanCm = 1;
        private const int _p100StepCountPixel = 10;
        private const int _p100StepCountCm = 10;

        #endregion

        #region 变量
        private double _minStepLengthCm;
        private double _maxStepLengthCm;
        private double _actualLength;
        private int _stepSpan;
        private int _stepCount;
        private double _stepLength;
        Line mouseVerticalTrackLine;
        Line mouseHorizontalTrackLine;
        #endregion

        #region 标尺边框加指针显示
        public void RaiseHorizontalRulerMoveEvent(MouseEventArgs e)
        {
            Point mousePoint = e.GetPosition(this);
            mouseHorizontalTrackLine.X1 = mouseHorizontalTrackLine.X2 = mousePoint.X;
        }
        public void RaiseVerticalRulerMoveEvent(MouseEventArgs e)
        {
            Point mousePoint = e.GetPosition(this);
            mouseVerticalTrackLine.Y1 = mouseVerticalTrackLine.Y2 = mousePoint.Y;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mouseVerticalTrackLine = GetTemplateChild("verticalTrackLine") as Line;
            mouseHorizontalTrackLine = GetTemplateChild("horizontalTrackLine") as Line;
            mouseVerticalTrackLine.Visibility = Visibility.Visible;
            mouseHorizontalTrackLine.Visibility = Visibility.Visible;
        }
        #endregion

        /// <summary>
        /// 重画标尺数据
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            try
            {
                Pen pen = new Pen(new SolidColorBrush(Colors.Black), 0.8d);
                pen.Freeze();
                Initialize();
                GetActualLength();
                GetStep();
                base.OnRender(drawingContext);

                this.BorderBrush = new SolidColorBrush(Colors.Black);
                this.BorderThickness = new Thickness(0.1);
                this.Background = new SolidColorBrush(Colors.White);

                #region try
                // double actualPx = this._actualLength / DisplayPercent;
                Position currentPosition = new Position
                {
                    CurrentStepIndex = 0,
                    Value = 0
                };

                switch (DisplayType)
                {
                    case RulerDisplayType.Horizontal:
                        {
                            /* 绘制前半段 */
                            DrawLine(drawingContext, ZeroPoint, currentPosition, pen, 0);
                            /* 绘制后半段 */
                            DrawLine(drawingContext, ZeroPoint, currentPosition, pen, 1);
                            break;
                        }
                    case RulerDisplayType.Vertical:
                        {
                            /* 绘制前半段 */
                            DrawLine(drawingContext, ZeroPoint, currentPosition, pen, 0);
                            /* 绘制后半段 */
                            DrawLine(drawingContext, ZeroPoint, currentPosition, pen, 1);
                            break;
                        }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void DrawLine(DrawingContext drawingContext, double currentPoint, Position currentPosition, Pen pen, int type)
        {
            double linePercent = 0d;
            while (true)
            {
                if (currentPosition.CurrentStepIndex == 0)
                {

                    FormattedText formattedText = GetFormattedText((currentPosition.Value / 10.0).ToString());


                    switch (DisplayType)
                    {
                        case RulerDisplayType.Horizontal:
                            {
                                var point = new Point(currentPoint + formattedText.Width / 2, formattedText.Height / 3);
                                if (point.X < 0)
                                {
                                    break;
                                }
                                drawingContext.DrawText(formattedText, point);
                                break;
                            }
                        case RulerDisplayType.Vertical:
                            {
                                Point point = new Point(this.ActualWidth, currentPoint + formattedText.Height / 2);
                                RotateTransform rotateTransform = new RotateTransform(90, point.X, point.Y);
                                if (point.Y < 0)
                                {
                                    break;
                                }
                                drawingContext.PushTransform(rotateTransform);
                                drawingContext.DrawText(formattedText, point);
                                drawingContext.Pop();
                                break;
                            }
                    }

                    linePercent = (int)LinePercent.P100;
                }
                else if (IsFinalNum(currentPosition.CurrentStepIndex, 3))
                {
                    linePercent = (int)LinePercent.P30;
                }
                else if (IsFinalNum(currentPosition.CurrentStepIndex, 5))
                {
                    linePercent = (int)LinePercent.P50;
                }
                else if (IsFinalNum(currentPosition.CurrentStepIndex, 7))
                {
                    linePercent = (int)LinePercent.P30;
                }
                else if (IsFinalNum(currentPosition.CurrentStepIndex, 0))
                {
                    linePercent = (int)LinePercent.P70;
                }
                else
                {
                    linePercent = (int)LinePercent.P20;
                }

                linePercent = linePercent * 0.01;

                switch (DisplayType)
                {
                    case RulerDisplayType.Horizontal:
                        {
                            if (currentPoint > 0)
                            {
                                drawingContext.DrawLine(pen, new Point(currentPoint, 0), new Point(currentPoint, this.ActualHeight * linePercent));
                            }

                            if (type == 0)
                            {
                                currentPoint = currentPoint - _stepLength;
                                currentPosition.CurrentStepIndex--;

                                if (currentPosition.CurrentStepIndex < 0)
                                {
                                    currentPosition.CurrentStepIndex = _stepCount - 1;
                                    currentPosition.Value = GetNextStepValue(currentPosition.Value, _stepSpan, 0);
                                }
                                else if (currentPosition.CurrentStepIndex == 0)
                                {
                                    if (currentPosition.Value % _stepSpan != 0)
                                    {
                                        currentPosition.Value = GetNextStepValue(currentPosition.Value, _stepSpan, 0);
                                    }
                                }

                                if (currentPoint <= 0)
                                {
                                    return;
                                }
                            }
                            else
                            {
                                currentPoint = currentPoint + _stepLength;
                                currentPosition.CurrentStepIndex++;

                                if (currentPosition.CurrentStepIndex >= _stepCount)
                                {
                                    currentPosition.CurrentStepIndex = 0;
                                    currentPosition.Value = GetNextStepValue(currentPosition.Value, _stepSpan, 1);
                                }

                                if (currentPoint >= _actualLength)
                                {
                                    return;
                                }
                            }
                            break;
                        }
                    case RulerDisplayType.Vertical:
                        {
                            if (currentPoint > 0)
                            {
                                drawingContext.DrawLine(pen, new Point(0, currentPoint), new Point(this.ActualWidth * linePercent, currentPoint));
                            }
                            if (type == 0)
                            {
                                currentPoint = currentPoint - _stepLength;
                                currentPosition.CurrentStepIndex--;

                                if (currentPosition.CurrentStepIndex < 0)
                                {
                                    currentPosition.CurrentStepIndex = _stepCount - 1;
                                    currentPosition.Value = GetNextStepValue(currentPosition.Value, _stepSpan, 0);
                                }
                                else if (currentPosition.CurrentStepIndex == 0)
                                {
                                    if (currentPosition.Value % _stepSpan != 0)
                                    {
                                        currentPosition.Value = GetNextStepValue(currentPosition.Value, _stepSpan, 0);
                                    }
                                }

                                if (currentPoint <= 0)
                                {
                                    return;
                                }
                            }
                            else
                            {
                                currentPoint = currentPoint + _stepLength;
                                currentPosition.CurrentStepIndex++;

                                if (currentPosition.CurrentStepIndex >= _stepCount)
                                {
                                    currentPosition.CurrentStepIndex = 0;
                                    currentPosition.Value = GetNextStepValue(currentPosition.Value, _stepSpan, 1);
                                }

                                if (currentPoint >= _actualLength)
                                {
                                    return;
                                }
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 获取下一个步长值
        /// </summary>
        /// <param name="value">起始值</param>
        /// <param name="times">跨度</param>
        /// <param name="type">半段类型，分为前半段、后半段</param>
        /// <returns></returns>
        private int GetNextStepValue(int value, int times, int type)
        {
            if (type == 0)
            {
                do
                {
                    value--;
                }
                while (value % times != 0);
            }
            else
            {
                do
                {
                    value++;
                }
                while (value % times != 0);
            }
            return (value);
        }

        
        private FormattedText GetFormattedText(string text)
        {
            var _lang = new System.Globalization.CultureInfo("zh-CHS", false);
            var _font = new Typeface("Microsoft YaHei");
            return new FormattedText(text, _lang, FlowDirection.LeftToRight, _font, 12, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
        }

        private bool IsFinalNum(int value, int finalNum)
        {
            string valueStr = value.ToString();
            if (valueStr.Substring(valueStr.Length - 1, 1) == finalNum.ToString())
            {
                return (true);
            }
            return (false);
        }

        /// <summary>
        /// 初始化获取屏幕的DPI
        /// </summary>
        private void Initialize()
        {
            Dpi dpi = new Dpi();
            dpi.DpiX = Dpi.DpiX;
            dpi.DpiY = Dpi.DpiY;
            if (Dpi.DpiX == 0)
            {
                dpi.DpiX = 96;
            }

            if (Dpi.DpiY == 0)
            {
                dpi.DpiY = 96;
            }

            Dpi = dpi;
            _minStepLengthCm = 0.1;
            _maxStepLengthCm = 0.3;

            if (DisplayPercent == 0)
                DisplayPercent = 1;

            switch (DisplayUnit)
            {
                case RulerDisplayUnit.pixel:
                    {
                        _stepSpan = _p100StepSpanPixel;
                        _stepCount = _p100StepCountPixel;
                        break;
                    }
                case RulerDisplayUnit.cm:
                    {
                        _stepSpan = _p100StepSpanCm;
                        _stepCount = _p100StepCountCm;
                        break;
                    }
            }
            int width = 15;
            switch (DisplayType)
            {
                case RulerDisplayType.Horizontal:
                    {
                        if (this.ActualHeight == 0)
                        {
                            Height = width;
                        }
                        break;
                    }
                case RulerDisplayType.Vertical:
                    {
                        if (this.ActualWidth == 0)
                        {
                            Width = width;
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 获取每一个数字间隔的跨度
        /// </summary>
        private void GetStep()
        {
            switch (DisplayUnit)
            {
                case RulerDisplayUnit.pixel:
                    {
                        double stepSpanCm;
                        while (true)
                        {
                            stepSpanCm = _stepSpan / Convert.ToDouble(GetDpi()) * _inchCm * DisplayPercent;
                            double stepLengthCm = stepSpanCm / _stepCount;
                            int type = 0;
                            bool isOut = false;
                            if (stepLengthCm > _maxStepLengthCm)
                            {
                                type = 1;
                                _stepCount = GetNextStepCount(_stepCount, type, ref isOut);
                            }

                            if (stepLengthCm < _minStepLengthCm)
                            {
                                type = 0;
                                _stepCount = GetNextStepCount(_stepCount, type, ref isOut);
                            }

                            if (stepLengthCm <= _maxStepLengthCm && stepLengthCm >= _minStepLengthCm)
                            {
                                _stepLength = stepSpanCm / _inchCm * Convert.ToDouble(GetDpi()) / _stepCount;
                                break;
                            }
                            /* 已超出或小于最大步进长度 */
                            if (isOut)
                            {
                                _stepSpan = GetNextStepSpan(_stepSpan, type);
                                continue;
                            }
                        }
                        break;
                    }
                case RulerDisplayUnit.cm:
                    {
                        double stepSpanCm;
                        while (true)
                        {
                            stepSpanCm = _stepSpan/10.0  * DisplayPercent;
                            double stepLengthCm = stepSpanCm / _stepCount;
                            int type = 0;
                            bool isOut = false;
                            if (stepLengthCm > _maxStepLengthCm)
                            {
                                type = 1;
                                _stepCount = GetNextStepCount(_stepCount, type, ref isOut);
                            }

                            if (stepLengthCm < _minStepLengthCm)
                            {
                                type = 0;
                                _stepCount = GetNextStepCount(_stepCount, type, ref isOut);
                            }

                            if (stepLengthCm <= _maxStepLengthCm && stepLengthCm >= _minStepLengthCm)
                            {
                                _stepLength = stepSpanCm / _inchCm * Convert.ToDouble(GetDpi()) / _stepCount;
                                break;
                            }
                            /* 已超出或小于最大步进长度 */
                            if (isOut)
                            {
                                _stepSpan = GetNextStepSpan(_stepSpan, type);
                                continue;
                            }
                        }
                        break;
                    }
            }
        }


        private int GetNextStepCount(int stepCount, int type, ref bool isOut)
        {
            int result = stepCount;
            isOut = false;
            switch (type)
            {
                case 0:
                    {
                        if (stepCount == 20)
                        {
                            result = 10;
                        }
                        else
                        {
                            isOut = true;
                        }
                        break;
                    }
                case 1:
                    {
                        if (stepCount == 10)
                        {
                            result = 20;
                        }
                        else
                        {
                            isOut = true;
                        }

                        break;
                    }
            }
            return result;
        }


        private int GetNextStepSpan(int stepSpan, int type)
        {
            string stepCountStr = stepSpan.ToString();
            string resultStr = string.Empty;

            switch (DisplayUnit)
            {
                case RulerDisplayUnit.pixel:
                    {
                        switch (type)
                        {
                            case 0:
                                {
                                    if (stepCountStr.IndexOf('5') > -1)
                                    {
                                        resultStr = GetNumberAndZeroNum(1, stepCountStr.Length);
                                    }
                                    else if (stepCountStr.IndexOf('2') > -1)
                                    {
                                        resultStr = GetNumberAndZeroNum(5, stepCountStr.Length - 1);
                                    }
                                    else if (stepCountStr.IndexOf('1') > -1)
                                    {
                                        resultStr = GetNumberAndZeroNum(2, stepCountStr.Length - 1);
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    if (stepCountStr.IndexOf('5') > -1)
                                    {
                                        resultStr = GetNumberAndZeroNum(2, stepCountStr.Length - 1);
                                    }
                                    else if (stepCountStr.IndexOf('2') > -1)
                                    {
                                        resultStr = GetNumberAndZeroNum(1, stepCountStr.Length - 1);
                                    }
                                    else if (stepCountStr.IndexOf('1') > -1)
                                    {
                                        resultStr = GetNumberAndZeroNum(5, stepCountStr.Length - 2);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case RulerDisplayUnit.cm:
                    {
                        switch (type)
                        {
                            case 0:
                                {
                                    if (stepCountStr.IndexOf('5') > -1)
                                    {
                                        resultStr = GetNumberAndZeroNum(1, stepCountStr.Length);
                                    }
                                    else if (stepCountStr.IndexOf('2') > -1)
                                    {
                                        resultStr = GetNumberAndZeroNum(5, stepCountStr.Length - 1);
                                    }
                                    else if (stepCountStr.IndexOf('1') > -1)
                                    {
                                        resultStr = GetNumberAndZeroNum(2, stepCountStr.Length - 1);
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    if (stepCountStr.IndexOf('5') > -1)
                                    {
                                        resultStr = GetNumberAndZeroNum(2, stepCountStr.Length - 1);
                                    }
                                    else if (stepCountStr.IndexOf('2') > -1)
                                    {
                                        resultStr = GetNumberAndZeroNum(1, stepCountStr.Length - 1);
                                    }
                                    else if (stepCountStr.IndexOf('1') > -1)
                                    {
                                        resultStr = GetNumberAndZeroNum(5, stepCountStr.Length - 2);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
            }

            int result = 0;
            if (string.IsNullOrWhiteSpace(resultStr))
            {
                return 0;
            }

            if (int.TryParse(resultStr, out result))
            {
                return result;
            }
            return result;
        }


        private string GetNumberAndZeroNum(int num, int zeroNum)
        {
            string result = string.Empty;
            result += num;
            for (int i = 0; i < zeroNum; i++)
            {
                result += "0";
            }
            return (result);
        }


        private int GetDpi()
        {
            switch (DisplayType)
            {
                case RulerDisplayType.Horizontal:
                    {
                        return (Dpi.DpiX);
                    }
                case RulerDisplayType.Vertical:
                    {
                        return (Dpi.DpiY);
                    }
                default:
                    {
                        return (Dpi.DpiX);
                    }
            }
        }

        private void GetActualLength()
        {
            switch (DisplayType)
            {
                case RulerDisplayType.Horizontal:
                    {
                        _actualLength = this.ActualWidth;
                        break;
                    }
                case RulerDisplayType.Vertical:
                    {
                        _actualLength = this.ActualHeight;
                        break;
                    }
            }
        }
    }

    public enum RulerDisplayType
    {
        Horizontal, Vertical
    }

    public enum RulerDisplayUnit
    {
        pixel, cm
    }

    public enum LinePercent
    {
        P20 = 20, P30 = 30, P50 = 50, P70 = 70, P100 = 100
    }

    public struct Dpi
    {
        public int DpiX
        {
            get; set;
        }
        public int DpiY
        {
            get; set;
        }
    }

    public struct Position
    {
        public int Value
        {
            get; set;
        }
        public int CurrentStepIndex
        {
            get; set;
        }
    }
}
