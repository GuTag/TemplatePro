using PrintManager.UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace PrintManager.UI.Controls
{
    [TemplatePart(Name = CanvasTemplateName, Type = typeof(Canvas))]
    [TemplatePart(Name = PopupTemplateName, Type = typeof(Popup))]
    public class DiagramChart : Control
    {
        const string CanvasTemplateName = "PART_Canvas";
        const string PopupTemplateName = "PART_Popup";

        private Canvas _canvas;
        private Popup _popup;
        //private double fontsize = 12;
        private bool flg = false;



        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(DiagramChart), new PropertyMetadata(null));



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DiagramChart), new PropertyMetadata(null));


        public ObservableCollection<DiagramSerise> ItemsSource
        {
            get { return (ObservableCollection<DiagramSerise>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<DiagramSerise>), typeof(DiagramChart), new PropertyMetadata(null, new PropertyChangedCallback(ItemsSourceChanged)));

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as DiagramChart;
            if (e.NewValue != null)
                view.DrawArc();
        }

        static DiagramChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramChart), new FrameworkPropertyMetadata(typeof(DiagramChart)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = GetTemplateChild(CanvasTemplateName) as Canvas;
            _popup = GetTemplateChild(PopupTemplateName) as Popup;

        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            DrawArc();
        }

        void DrawArc()
        {
            if (ItemsSource is null || !ItemsSource.Any() || _canvas is null)//
                return;
            _canvas.Children.Clear();
            var zeroPoint = new Point(20, 20);
            var lineX = new Line()
            {
                X1 = 0,
                Y1 = _canvas.Height - zeroPoint.Y - 10,
                X2 = _canvas.Width - zeroPoint.X - 10,
                Y2 = _canvas.Height - zeroPoint.Y - 10,
                Stroke = Brushes.Black,
                StrokeThickness = 0.8,
            };
            var lineY = new Line()
            {
                X1 = 0,
                Y1 = _canvas.Height - zeroPoint.Y - 10,
                X2 = 0,
                Y2 = 0,
                Stroke = Brushes.Black,
                StrokeThickness = 0.8,
            };
            Canvas.SetLeft(lineX, zeroPoint.X);
            Canvas.SetLeft(lineY, zeroPoint.X);

            Canvas.SetBottom(lineX, zeroPoint.Y);
            Canvas.SetBottom(lineY, zeroPoint.Y);

            _canvas.Children.Add(lineX);
            _canvas.Children.Add(lineY);

            var maxPosY = ItemsSource.Select(ser => ser.RequestNum).Max();
            var maxPosX = ItemsSource.Count;
            var intervalWidth = (_canvas.Width - zeroPoint.X * 2) * 1.0 / maxPosX;
            var rectangleWidth = intervalWidth / 4.0;
            double heightPercent = (_canvas.Height - zeroPoint.X * 2) * 1.0 / maxPosY;

            

            int index = 0;
            foreach (var item in ItemsSource)
            {
                DiagramBase Diagrambase = new DiagramBase();
                Diagrambase.Title = item.Title;
                Diagrambase.RequestNum = item.RequestNum;
                //Diagrambase.ComplateNum = item.ComplateNum;
                Diagrambase.Rect1 = new Rectangle() { Width= rectangleWidth, Height= heightPercent*item.RequestNum };
                //if (( heightPercent * item.ComplateNum) > 1000 )
                //{
                //    heightPercent = 0;
                //}
                //Diagrambase.Rect2 = new Rectangle() { Width = rectangleWidth, Height = heightPercent * item.ComplateNum };
                Diagrambase.DiagramColor = item.DiagramColor;

                Diagrambase.Rect1.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(item.DiagramColor));
                //Diagrambase.Rect2.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(item.DiagramColor));
                //Diagrambase.Rect2.Opacity = 0.5;

                


                var textName = new TextBlock() {TextAlignment=TextAlignment.Center, Width= rectangleWidth*2, Text = Diagrambase.Title};
                var text1 = new TextBlock() { TextAlignment = TextAlignment.Center, Width = rectangleWidth, Text = Diagrambase.RequestNum.ToString()};
                //var text2 = new TextBlock() { TextAlignment = TextAlignment.Center, Width = rectangleWidth, Text = Diagrambase.ComplateNum.ToString()};

                Canvas.SetBottom(Diagrambase.Rect1, zeroPoint.Y);
                //Canvas.SetBottom(Diagrambase.Rect2, zeroPoint.Y);
                Canvas.SetBottom(textName, 5);
                Canvas.SetBottom(text1, zeroPoint.Y + Diagrambase.Rect1.Height);
                //Canvas.SetBottom(text2, zeroPoint.Y + Diagrambase.Rect2.Height);
                if (index == 0)
                {
                    Canvas.SetLeft(Diagrambase.Rect1, rectangleWidth + zeroPoint.X);
                    //Canvas.SetLeft(Diagrambase.Rect2, rectangleWidth + rectangleWidth + zeroPoint.X);
                    Canvas.SetLeft(textName, rectangleWidth + zeroPoint.X);
                    Canvas.SetLeft(text1, rectangleWidth + zeroPoint.X);
                    //Canvas.SetLeft(text2, rectangleWidth + rectangleWidth + zeroPoint.X);
                }
                else 
                {
                    Canvas.SetLeft(Diagrambase.Rect1, rectangleWidth * index * 4 + zeroPoint.X);
                    //Canvas.SetLeft(Diagrambase.Rect2, rectangleWidth * index * 4 + rectangleWidth + zeroPoint.X);
                    Canvas.SetLeft(textName, rectangleWidth * index * 4 + zeroPoint.X);
                    Canvas.SetLeft(text1, rectangleWidth * index * 4 + zeroPoint.X);
                    //Canvas.SetLeft(text2, rectangleWidth * index * 4 + rectangleWidth + zeroPoint.X);
                }
                _canvas.Children.Add(Diagrambase.Rect1);
                //_canvas.Children.Add(Diagrambase.Rect2);
                _canvas.Children.Add(textName);
                _canvas.Children.Add(text1);
                //_canvas.Children.Add(text2);
                index++;

                Diagrambase.Rect1.DataContext = Diagrambase;
                //Diagrambase.Rect2.DataContext = Diagrambase;
                Diagrambase.Rect1.MouseMove += Rectangle_MouseMove;
                Diagrambase.Rect1.MouseLeave += Rectangle_MouseLeave;
                //Diagrambase.Rect2.MouseMove += Rectangle_MouseMove;
               // Diagrambase.Rect2.MouseLeave += Rectangle_MouseLeave;
            }
        }
        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            _popup.IsOpen = false;
            //var path = sender as Rectangle;
            //var dt = path.DataContext as DiagramBase;

            //TranslateTransform ttf = new TranslateTransform();
            //ttf.X = 0;
            //ttf.Y = 0;
            //path.RenderTransform = ttf;
            //dt.Line.RenderTransform = new TranslateTransform()
            //{
            //    X = 0,
            //    Y = 0,
            //};

            //dt.TextPath.RenderTransform = new TranslateTransform()
            //{
            //    X = 0,
            //    Y = 0,
            //};

            //path.Effect = new DropShadowEffect()
            //{
            //    Color = (Color)ColorConverter.ConvertFromString("#FF949494"),
            //    BlurRadius = 20,
            //    Opacity = 0,
            //    ShadowDepth = 0
            //};
            flg = false;
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            //动画
            if (!flg)
            {

                //BegionOffsetAnimation(path);
            }
            ShowMousePopup(rect, e);


        }

        void ShowMousePopup(Rectangle rect, MouseEventArgs e)
        {
            var data = rect.DataContext as DiagramBase;
            if (!_popup.IsOpen)
                _popup.IsOpen = true;

            var mousePosition = e.GetPosition((UIElement)_canvas.Parent);

            _popup.HorizontalOffset = mousePosition.X + 10;
            _popup.VerticalOffset = mousePosition.Y + 10;

            //Text = ("总量" + " : " + data.RequestNum) + "\r\n" + ("已打印" + " : " + data.ComplateNum);//显示鼠标当前坐标点
            Text = ("总量" + " : " + data.RequestNum);
            Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.DiagramColor));;
        }

        //void BegionOffsetAnimation(Path path)
        //{
        //    //NameScope.SetNameScope(this, new NameScope());
        //    //var pathDataContext = path.DataContext as DiagramBase;
        //    //var angle = pathDataContext.Angle;

        //    //minPoint = new Point(Math.Round(pathDataContext.StarPoint.X + pathDataContext.EndPoint.X) / 2, Math.Round(pathDataContext.StarPoint.Y + pathDataContext.EndPoint.Y) / 2);


        //    //var v1 = minPoint - new Point(centenrX, centenrY);

        //    //var v2 = new Point(2000, 0) - new Point(0, 0);
        //    //double vAngle = 0;
        //    //if (180 < angle && angle <= 360 && pathDataContext.Percentage / ItemsSource.Select(p => p.Percentage).Sum() >= 0.5)
        //    //{
        //    //    vAngle = Math.Round(Vector.AngleBetween(v2, -v1));
        //    //}
        //    //else
        //    //{
        //    //    vAngle = Math.Round(Vector.AngleBetween(v2, v1));
        //    //}


        //    //offsetX = 10 * Math.Cos(vAngle * Math.PI / 180);
        //    //offsetY = 10 * Math.Sin(vAngle * Math.PI / 180);

        //    //var line3 = pathDataContext.Line;
        //    //var textPath = pathDataContext.TextPath;

        //    //TranslateTransform LineAnimatedTranslateTransform =
        //    //    new TranslateTransform();
        //    //this.RegisterName("LineAnimatedTranslateTransform", LineAnimatedTranslateTransform);
        //    //line3.RenderTransform = LineAnimatedTranslateTransform;


        //    //TranslateTransform animatedTranslateTransform =
        //    //    new TranslateTransform();
        //    //this.RegisterName("AnimatedTranslateTransform", animatedTranslateTransform);
        //    //path.RenderTransform = animatedTranslateTransform;

        //    //TranslateTransform TextAnimatedTranslateTransform =
        //    //   new TranslateTransform();
        //    //this.RegisterName("TextAnimatedTranslateTransform", animatedTranslateTransform);
        //    //textPath.RenderTransform = animatedTranslateTransform;


        //    //DoubleAnimation daX = new DoubleAnimation();
        //    //Storyboard.SetTargetProperty(daX, new PropertyPath(TranslateTransform.XProperty));
        //    //daX.Duration = new Duration(TimeSpan.FromSeconds(0.2));
        //    //daX.From = 0;
        //    //daX.To = offsetX;


        //    //DoubleAnimation daY = new DoubleAnimation();

        //    //Storyboard.SetTargetName(daY, nameof(animatedTranslateTransform));
        //    //Storyboard.SetTargetProperty(daY, new PropertyPath(TranslateTransform.YProperty));
        //    //daY.Duration = new Duration(TimeSpan.FromSeconds(0.2));
        //    //daY.From = 0;
        //    //daY.To = offsetY;

        //    //path.Effect = new DropShadowEffect()
        //    //{
        //    //    Color = (Color)ColorConverter.ConvertFromString("#2E2E2E"),
        //    //    BlurRadius = 33,
        //    //    Opacity = 0.6,
        //    //    ShadowDepth = 0
        //    //};

        //    //animatedTranslateTransform.BeginAnimation(TranslateTransform.XProperty, daX);
        //    //animatedTranslateTransform.BeginAnimation(TranslateTransform.YProperty, daY);
        //    //LineAnimatedTranslateTransform.BeginAnimation(TranslateTransform.XProperty, daX);
        //    //LineAnimatedTranslateTransform.BeginAnimation(TranslateTransform.YProperty, daY);
        //    //TextAnimatedTranslateTransform.BeginAnimation(TranslateTransform.XProperty, daX);
        //    //TextAnimatedTranslateTransform.BeginAnimation(TranslateTransform.YProperty, daY);




        //    //flg = true;
        //}
        /// <summary>
        /// 画指示线
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        //Polyline DrawLine(Path path)
        //{
        //    NameScope.SetNameScope(this, new NameScope());
        //    var pathDataContext = path.DataContext as DiagramBase;
        //    var angle = pathDataContext.Angle;
        //    pathDataContext.Line = null;
        //    minPoint = new Point(Math.Round(pathDataContext.StarPoint.X + pathDataContext.EndPoint.X) / 2, Math.Round(pathDataContext.StarPoint.Y + pathDataContext.EndPoint.Y) / 2);

        //    Vector v1;
        //    if (angle > 180 && angle < 360)
        //    {
        //        v1 = new Point(centenrX, centenrY) - minPoint;
        //    }
        //    else if (angle == 180 || angle == 360)
        //    {
        //        if (Math.Round(pathDataContext.StarPoint.X) == Math.Round(pathDataContext.EndPoint.X))
        //        {
        //            v1 = new Point(radius * 2, radius) - new Point(centenrX, centenrY);

        //        }
        //        else
        //        {
        //            if (Math.Round(pathDataContext.StarPoint.X) - Math.Round(pathDataContext.EndPoint.X) == 2 * radius)
        //            {
        //                v1 = new Point(radius, 2 * radius) - new Point(centenrX, centenrY);
        //            }
        //            else
        //            {
        //                v1 = new Point(radius, 0) - new Point(centenrX, centenrY);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        v1 = minPoint - new Point(centenrX, centenrY);
        //    }
        //    v1.Normalize();
        //    var Vmin = v1 * radius;
        //    var RadiusToNodal = Vmin + new Point(centenrX, centenrY);
        //    var v2 = new Point(2000, 0) - new Point(0, 0);
        //    double vAngle = 0;
        //    vAngle = Math.Round(Vector.AngleBetween(v2, v1));

        //    offsetX = 10 * Math.Cos(vAngle * Math.PI / 180);
        //    offsetY = 10 * Math.Sin(vAngle * Math.PI / 180);

        //    var prolongPoint = new Point(RadiusToNodal.X + offsetX * 1, RadiusToNodal.Y + offsetY * 1);

        //    if (RadiusToNodal.X == double.NaN || RadiusToNodal.Y == double.NaN || prolongPoint.X == double.NaN || prolongPoint.Y == double.NaN)
        //        return null;


        //    var point1 = RadiusToNodal;
        //    var point2 = prolongPoint;
        //    Point point3;
        //    if (prolongPoint.X >= radius)
        //        point3 = new Point(prolongPoint.X + 10, prolongPoint.Y);
        //    else
        //        point3 = new Point(prolongPoint.X - 10, prolongPoint.Y);
        //    PointCollection polygonPoints = new PointCollection();
        //    polygonPoints.Add(point1);
        //    polygonPoints.Add(point2);
        //    polygonPoints.Add(point3);
        //    var line3 = new Polyline();
        //    line3.Points = polygonPoints;
        //    line3.Stroke = pathDataContext.DiagramColor;
        //    pathDataContext.PolylineEndPoint = point3;

        //    return line3;
        //}

        //PathGeometry DrawText(Path path)
        //{
        //    NameScope.SetNameScope(this, new NameScope());
        //    var pathDataContext = path.DataContext as DiagramBase;

        //    Typeface typeface = new Typeface
        //        (new FontFamily("Microsoft YaHei"),
        //        FontStyles.Normal,
        //        FontWeights.Normal, FontStretches.Normal);

        //    FormattedText text = new FormattedText(
        //        pathDataContext.Title,
        //        new System.Globalization.CultureInfo("zh-cn"),
        //        FlowDirection.LeftToRight, typeface, fontsize, Brushes.RosyBrown
        //        );

        //    var textWidth = text.Width;

        //    Geometry geo = null;
        //    if (pathDataContext.PolylineEndPoint.X > radius)
        //        geo = text.BuildGeometry(new Point(pathDataContext.PolylineEndPoint.X + 4, pathDataContext.PolylineEndPoint.Y - fontsize / 1.8));
        //    else
        //        geo = text.BuildGeometry(new Point(pathDataContext.PolylineEndPoint.X - textWidth - 4, pathDataContext.PolylineEndPoint.Y - fontsize / 1.8));
        //    PathGeometry pathGeometry = geo.GetFlattenedPathGeometry();
        //    return pathGeometry;

        //}
    }
}
