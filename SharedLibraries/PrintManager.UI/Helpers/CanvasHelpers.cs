using PrintManager.Shared.Entity;
using PrintManager.Shared.Utils;
using PrintManager.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.IO;
using ZXing;
using ZXing.QrCode;
using PrintManager.Shared.Helpers;
using ZXing.Common;
using ZXing.QrCode.Internal;
using System.Windows.Media.Media3D;
using ZXing.Presentation;
using ZXing.Rendering;
using System.Windows.Controls;
using System.Drawing.Design;
using System.Windows.Ink;
using PrintManager.UI.Controls;

namespace PrintManager.UI.Helpers
{
    public class CanvasHelpers
    {
        /// <summary>
        /// 画直线
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="dc"></param>
        /// <param name="control"></param>
        /// <param name="ZoomPercent"></param>
        public static void DrawingLine(DrawingContext dc, ControlItem control, double ZoomPercent)
        {
            Pen pen = new Pen(Brushes.Black, 1.0);
            Point startPoint = new Point(50, 50);
            Point endPoint = new Point(200, 50);
            dc.DrawLine(pen, startPoint, endPoint);
        }
        /// <summary>
        /// 画文本
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="control"></param>
        public static void DrawingText(Canvas parent,DrawingContext dc, ControlItem control, double ZoomPercent)
        {
            var _lang = new System.Globalization.CultureInfo("en-us", false);
            var _font = new Typeface(
                FontHelpers.GetFontFamily(control.FontFamily), 
                FontHelpers.GetFontStyle(control.FontStyle), 
                FontHelpers.GetFontWeight(control.FontWeight), 
                FontStretches.Normal);
            SolidColorBrush _foreground = Brushes.Black;
            string _text = control.DisplayName;

            if (control.IsAssociation)
            {
                _foreground = StringConvertBrush("#ED4BE1");
            }

            FormattedText formattedText = new FormattedText(_text, _lang, FlowDirection.LeftToRight, _font, control.FontSize * ZoomPercent, _foreground, VisualTreeHelper.GetDpi(parent).PixelsPerDip);
            control.Width = PixelUtil.PixelTomm(formattedText.Width, ZoomPercent);//_stepSpan / Convert.ToDouble(GetDpi()) * _inchCm * DisplayPercent
            control.Height = PixelUtil.PixelTomm(formattedText.Baseline, ZoomPercent);

            var rect = GetContorlZoomPercent(control, ZoomPercent);

            dc.PushTransform(new ScaleTransform(control.WidthFactor, 1));// { CenterX = text.Width / 2 });
            if(control.FontWeight == "Bold")
            {
                dc.DrawText(formattedText, new Point(rect.X / control.WidthFactor, rect.Y));
            }
            else
            {
                var geometry = formattedText.BuildGeometry(new Point(rect.X / control.WidthFactor, rect.Y));
                dc.DrawGeometry(_foreground, new Pen(_foreground, 0.5), geometry);
            }
            dc.Pop();
        }

        /// <summary>
        /// 画图片
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="control"></param>
        public static void DrawingImage(DrawingContext dc, ControlItem control, double ZoomPercent)
        {
            if (!string.IsNullOrEmpty(control.ImageData))
            {
                double width, height;
                //var image = new BitmapImage(new Uri(imagefile));
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
                if (control.IsAspectRatio)
                {
                    control.Height = image.Height / image.Width * control.Width;
                }
                var rect = GetContorlZoomPercent(control, ZoomPercent);
                dc.DrawImage(image, rect);
            }
        }

        /// <summary>
        /// 画二维码
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="control"></param>
        public static void DrawingQRCode(DrawingContext dc, ControlItem control, double ZoomPercent)
        {
            QrCodeEncodingOptions options = new QrCodeEncodingOptions()
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = (int)Math.Round(PixelUtil.mmToPixel(control.Width, ZoomPercent)),
                Height = (int)Math.Round(PixelUtil.mmToPixel(control.Height, ZoomPercent)),
                Margin = 0
            };

            ZXing.BarcodeWriter writer= new ZXing.BarcodeWriter() 
            { 
                Format=BarcodeFormat.QR_CODE,
                Options= options
            };

            //writer.Renderer = new BitmapRenderer()
            //{
            //    Background = System.Drawing.Color.FromName("#0680D7"),
            //    Foreground = System.Drawing.Color.Black
            //};

            BitmapImage image = ImageHelpers.BitmapToBitmapImage(writer.Write(control.DisplayName));
            var rect = GetContorlZoomPercent(control, ZoomPercent);
            dc.DrawImage(image, rect);
        }

        /// <summary>
        /// 画二维码
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="control"></param>
        public static void DrawingBarCode(DrawingContext dc, ControlItem control, double ZoomPercent)
        {
            EncodingOptions options = new EncodingOptions()
            {
                Width = (int)Math.Round(PixelUtil.mmToPixel(control.Width, ZoomPercent)),
                Height = (int)Math.Round(PixelUtil.mmToPixel(control.Height, ZoomPercent)),
                Margin = 0,
                
            };

            ZXing.BarcodeWriter writer = new ZXing.BarcodeWriter()
            {
                Format = BarcodeFormat.CODE_128,
                Options = options
            };
            //string a = "#0680D7";
            //var background = System.Drawing.Color.FromArgb(
            //    int.Parse(a.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
            //    int.Parse(a.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
            //    int.Parse(a.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier)
            // );
            //writer.Renderer = new BitmapRenderer()
            //{
            //    Background = System.Drawing.Color.Transparent,
            //    Foreground = System.Drawing.Color.Black
            //};

            BitmapImage image = ImageHelpers.BitmapToBitmapImage(writer.Write(control.DisplayName));
            var rect = GetContorlZoomPercent(control, ZoomPercent);
            dc.DrawImage(image, rect);
        }

        /// <summary>
        /// 获取缩放位置与大小
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Rect GetContorlZoomPercent(ControlItem control, double ZoomPercent)
        {
            var x = PixelUtil.mmToPixel(control.PosX, ZoomPercent);
            var y = PixelUtil.mmToPixel(control.PosY, ZoomPercent);
            var width = PixelUtil.mmToPixel(control.Width * control.WidthFactor, ZoomPercent);
            var height = PixelUtil.mmToPixel(control.Height, ZoomPercent);
            return new Rect(x, y, width, height);
        }

        /// <summary>
        /// 颜色从字符串转换
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static SolidColorBrush StringConvertBrush(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString(str));
                }
                catch (Exception)
                {
                }
            }
            return Brushes.White;
        }

        /// <summary>
        /// 生成带logo的二维码
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="text"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static System.Drawing.Bitmap GetQRCodeWithLogo(string filepath, string text, int width, int height)
        {
            //logo图片
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(filepath);
            
            MultiFormatWriter writer = new MultiFormatWriter();
            Dictionary<EncodeHintType, object> hint = new Dictionary<EncodeHintType, object>();
            hint.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            hint.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
            hint.Add(EncodeHintType.MARGIN, 2);

            BitMatrix bm = writer.encode(text, BarcodeFormat.QR_CODE, width + 80, height + 80, hint);
            bm = DeleteWhite(bm);
            ZXing.BarcodeWriter barcodeWriter = new ZXing.BarcodeWriter();
            System.Drawing.Bitmap map = barcodeWriter.Write(bm);

            int[] rectangle = bm.getEnclosingRectangle();

            int middleW = Math.Min((int)(rectangle[2] / 3.5), logo.Width);
            int middleH = Math.Min((int)(rectangle[3] / 3.5), logo.Height);
            int middleL = (map.Width - middleW) / 2;
            int middleT = (map.Height - middleH) / 2;

            System.Drawing.Bitmap bmpimg = new System.Drawing.Bitmap(map.Width, map.Height);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmpimg))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.DrawImage(map, 0, 0, width, height);
                g.FillRectangle(System.Drawing.Brushes.White, middleL, middleT, middleW, middleH);
                g.DrawImage(logo, middleL, middleT, middleW, middleH);
            }
            return bmpimg;
        }

        /// <summary>
        /// 删除生成的二维码默认对应的空白
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static BitMatrix DeleteWhite(BitMatrix matrix)
        {
            int[] rec = matrix.getEnclosingRectangle();
            int resWidth = rec[2] + 1;
            int resHeight = rec[3] + 1;

            BitMatrix resMatrix = new BitMatrix(resWidth, resHeight);
            resMatrix.clear();
            for (int i = 0; i < resWidth; i++)
            {
                for (int j = 0; j < resHeight; j++)
                {
                    if (matrix[i + rec[0], j + rec[1]])
                    {
                        resMatrix[i, j] = true;
                    }
                }
            }
            return resMatrix;
        }

        /// <summary>
        /// Canvas转换为图片
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static BitmapSource ConvertToBitmapSource(PrintCanvas canvas)
        {
            double scale = 300.0/96;
            var width = (int)Math.Round(canvas.RenderSize.Width/canvas.ZoomPercent  * scale);
            var height = (int)Math.Round(canvas.RenderSize.Height / canvas.ZoomPercent * scale);
            var target = new RenderTargetBitmap(width, height, 96 * scale, 96 * scale, PixelFormats.Pbgra32);
            var brush = new VisualBrush(canvas);

            var visual = new DrawingVisual();
            var drawingContext = visual.RenderOpen();


            drawingContext.DrawRectangle(brush, null, new Rect(new Point(0, 0),
                new Point(canvas.RenderSize.Width / canvas.ZoomPercent, canvas.RenderSize.Height / canvas.ZoomPercent)));

            drawingContext.PushOpacityMask(brush);

            drawingContext.Close();

            target.Render(visual);

            return target;
        }

        public static void ScaleImage(string sourceImg, string targetImg, double width, double height)
        {
            var source = new System.Drawing.Bitmap(sourceImg);
            ScaleImage(source, targetImg, width, height);
        }

        public static void ScaleImage(System.Drawing.Bitmap source, string targetImg, double width, double height)
        {
            var img = new System.Drawing.Bitmap(source, (int)Math.Round(width), (int)Math.Round(height));
            source.Dispose();
            img.Save(targetImg);
            img.Dispose();
        }
    }
}
