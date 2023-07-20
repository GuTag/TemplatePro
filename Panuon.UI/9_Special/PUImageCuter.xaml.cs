﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Panuon.UI
{
    /// <summary>
    /// ImageCuter.xaml 的交互逻辑
    /// </summary>
    public partial class PUImageCuter : UserControl
    {
        public PUImageCuter()
        {
            InitializeComponent();
        }

        #region Property
        public BitmapImage ImageSource
        {
            get { return (BitmapImage)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(BitmapImage), typeof(PUImageCuter));

        public ImageType ImageType
        {
            get { return (ImageType)GetValue(ImageTypeProperty); }
            set { SetValue(ImageTypeProperty, value); }
        }

        public static readonly DependencyProperty ImageTypeProperty =
            DependencyProperty.Register("ImageType", typeof(ImageType), typeof(PUImageCuter), new PropertyMetadata(ImageType.Rectangle, OnImageTypeChanged));

        private static void OnImageTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cuter = d as PUImageCuter;
            cuter.ResizeContainer.IsSquare = cuter.ImageType == ImageType.Square;
        }

        public double ImageMinHeight
        {
            get { return (double)GetValue(ImageMinHeightProperty); }
            set { SetValue(ImageMinHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageMinHeightProperty =
            DependencyProperty.Register("ImageMinHeight", typeof(double), typeof(PUImageCuter), new PropertyMetadata(50.0));

        public double ImageMinWidth
        {
            get { return (double)GetValue(ImageMinWidthProperty); }
            set { SetValue(ImageMinWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageMinWidthProperty =
            DependencyProperty.Register("ImageMinWidth", typeof(double), typeof(PUImageCuter), new PropertyMetadata(50.0));

        public double ImageMaxHeight
        {
            get { return (double)GetValue(ImageMaxHeightProperty); }
            set { SetValue(ImageMaxHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageMaxHeightProperty =
            DependencyProperty.Register("ImageMaxHeight", typeof(double), typeof(PUImageCuter), new PropertyMetadata(double.NaN));



        public double ImageMaxWidth
        {
            get { return (double)GetValue(ImageMaxWidthProperty); }
            set { SetValue(ImageMaxWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageMaxWidthProperty =
            DependencyProperty.Register("ImageMaxWidth", typeof(double), typeof(PUImageCuter), new PropertyMetadata(double.NaN));
        #endregion

        #region Event
        private void Img_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var image = sender as Image;
            CvaMain.Width = image.ActualWidth;
            CvaMain.Height = image.ActualHeight;
            CvaMain.Margin = new Thickness((ActualWidth - image.ActualWidth) / 2, (ActualHeight - image.ActualHeight) / 2, (ActualWidth - image.ActualWidth) / 2, (ActualHeight - image.ActualHeight) / 2);
            ResizeContainer.Visibility = Visibility.Visible;
        }
        #endregion

        #region APIs
        public BitmapSource GetCutedImage()
        {
            if (ImageSource == null)
                return null;

            var widthScale = ImageSource.PixelWidth / CvaMain.ActualWidth;
            var heightScale = ImageSource.PixelHeight / CvaMain.ActualHeight;

            return new CroppedBitmap(BitmapFrame.Create(ImageSource), new Int32Rect((int)(Canvas.GetLeft(ResizeContainer) * widthScale), (int)(Canvas.GetTop(ResizeContainer) * heightScale), (int)(ResizeContainer.ActualWidth * widthScale), (int)(ResizeContainer.ActualHeight * heightScale)));

        }
        #endregion
    }
}
