using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Shared.Utils
{
    public class PixelUtil
    {
       public static double mmToPixel(double mm, double delta)
        {
            //return Math.Round(mm / 25.4 * 96 * delta);
            return mm / 25.4 * 96 * delta;
        }

        public static double PixelTomm(double pixel, double delta)
        {
            //return Math.Round( pixel * 25.4 / 96 / delta );
            return pixel * 25.4 / 96 / delta;
        }

        public static int mmToPixel_Print(double mm)
        {
            //return Math.Round(mm / 25.4 * 96 * delta);
            return (int)Math.Round(mm * 12);
        }

        public static int PixelTomm_Print(double pixel)
        {
            //return Math.Round( pixel * 25.4 / 96 / delta );
            return (int)Math.Round(pixel * 25.4 / 12);
        }
    }
}
