using PrintManager.Shared.Utils;
using Svg2Xaml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PrintManager.Shared.Helpers
{
    public class ImageHelpers
    {
        public static string ImageConvertString(string imagefile)
        {
            if(!File.Exists(imagefile)) { return null; }
            string str;
            using(Stream s = File.Open(imagefile, FileMode.Open, FileAccess.Read))
            {
                int leng = 0;
                if (s.Length < Int32.MaxValue)
                    leng = (int)s.Length;
                byte[] by = new byte[leng];
                s.Read(by, 0, leng);//把图片读到字节数组中
                s.Close();
                s.Dispose();
                str = Convert.ToBase64String(by);//把字节数组转换成字符串
            };
            return str;

        }

        //把字符串还原成图片
        public static BitmapImage StringConvertBitmapImage(string data)
        {
            byte[] buf = Convert.FromBase64String(data);//把字符串读到字节数组中
            
            using (MemoryStream ms = new MemoryStream(buf))
            {
                BitmapImage bitmapImage = new BitmapImage();
                ms.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        //public static string BitmapConvertString(Bitmap bitmap)
        //{
        //    if (!File.Exists(imagefile)) { return null; }
        //    Stream s = File.Open(imagefile, FileMode.Open);
        //    int leng = 0;
        //    if (s.Length < Int32.MaxValue)
        //        leng = (int)s.Length;
        //    byte[] by = new byte[leng];
        //    s.Read(by, 0, leng);//把图片读到字节数组中
        //    s.Close();

        //    string str = Convert.ToBase64String(by);//把字节数组转换成字符串
        //    return str;
        //}

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            System.Drawing.Bitmap ImageOriginalBase = new System.Drawing.Bitmap(bitmap);
            BitmapImage bitmapImage = new BitmapImage();
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                ImageOriginalBase.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }


        public static DrawingImage SVG2Image(string filepath)
        {
            try
            {
                if (filepath.ToLower().EndsWith(".svg"))
                {
                    string _path = Path.Combine(PathUtil.GetBasePath, filepath);
                    using (FileStream stream = new FileStream(_path, FileMode.Open, FileAccess.Read))
                    {
                        return SvgReader.Load(stream);
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
