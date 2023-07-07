using PrintManager.UI.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PrintManager.MainClient.Components
{
    public class SystemUtil
    {
        public struct DPI
        {
            public double X { get; set; }
            public double Y { get; set; }
            public DPI(double x, double y)
            {
                X = x;
                Y = y;
            }
        }
        public static DPI GetDpiBySystemParameters()
        {
            const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", bindingFlags);
            var dpiYProperty = typeof(SystemParameters).GetProperty("DpiY", bindingFlags);
            var dpiX = 96.0;
            var dpiY = 96.0;
            if (dpiXProperty != null)
            {
                dpiX = (double)dpiXProperty.GetValue(null, null);
            }
            if (dpiYProperty != null)
            {
                dpiY = (double)dpiYProperty.GetValue(null, null);
            }
            return new DPI(dpiX, dpiY);
        }

        public static double GetRatio()
        {
            double ratio = 0;
            Graphics graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            ratio = (double)(graphics.DpiX * 1.041666667);

            return ratio;
        }
    }
}
