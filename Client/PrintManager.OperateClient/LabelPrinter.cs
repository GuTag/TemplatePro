using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.OperateClient
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TimeoutParams
    {
        public int read_timeout;
        public int write_timeout;
        public int wait_time_after_read;
        public int wait_time_after_write;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PrinterStatus
    {
        public int self_size; //设置为sizeof(PrinterStatus)。

        public int printer_language; //打印机语言。

        public int is_ready_to_print;//1表示打印机为就绪状态，可以接收打印任务并打印；0表示为未就绪状态，不可进行打印。

        public int is_paused; //1表示打印机处于暂停状态，此状态下打印机不能打印。

        public int is_paper_out; //1表示打印机缺纸，此状态下打印机不能打印。

        public int is_paper_near_out; //预留，固定为0。

        public int is_head_too_hot; //1表示打印头过热，此状态下打印机不能打印。

        public int is_head_opened; //1表示打印机处于开盖状态，此状态下打印机不能打印。

        public int is_receive_buffer_full;//1表示打印机接收缓冲区满，此状态下打印机将不再接收数据。

        public int is_ribbon_out; //1表示碳带用尽，此状态下打印机不能打印。

        public int is_ribbon_near_out;//预留，固定为0。

        public int is_cutter_error; //1表示切刀处于错误状态，此状态下打印机不能打印。

        public int is_printer_busy;//1表示打印机的正在进行打印。

        public int has_other_error; //预留，固定为0。
    }

    public class LabelPrinter
    {
        public IntPtr devHandle = new IntPtr();
        public int errorNo = 0;

        public const string SDK_Dll_Path = "LabelPrinterSDK.dll";

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DiscoverPrinter(int portType, StringBuilder devInfo, ref int deviceInfoLen, ref int devCount, int timeout);
        public string [] DiscoverPrinter(int portType)
        {
            int outLen = 5000;
            int count = 0;
            StringBuilder outStr = new StringBuilder(outLen);
            int r = LabelPrinter.DiscoverPrinter(portType, outStr, ref outLen, ref count, 0);
            if (r == 0)
            {
                string devInfo = outStr.ToString();
                char[] separators = new char[] { '@' };
                string[] devices = devInfo.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                return devices;
            }
            errorNo = r;
            return null;
        }
        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ConnectPrinter(int portType, string portInfo, int printerLanguage, ref IntPtr phDev);
        public int ConnectPrinter(int portType, string portInfo, int printerLanguage) {
            int r =  ConnectPrinter(portType, portInfo, printerLanguage, ref devHandle);
            errorNo = r;
            return r;
        }
        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int DisconnectPrinter(IntPtr dev);
        public int DisconnectPrinter()
        {
            int r =  DisconnectPrinter(devHandle);
            errorNo = r;
            return r;
        }
        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int WritePort(IntPtr hDev, byte[] buffer, ref int bufLen);
        public int WritePort(byte[] buffer, ref int bufLen)
        {
            int r = WritePort(devHandle, buffer, ref bufLen);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ReadPort(IntPtr hDev, byte[] buffer, ref int bufLen);
        public int ReadPort(byte[] buffer, ref int bufLen)
        {
            int r = ReadPort(devHandle, buffer, ref bufLen);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetPortTimeout(IntPtr hDev, ref TimeoutParams param);
        public int SetPortTimeout(TimeoutParams param)
        {
            int r =  SetPortTimeout(devHandle, ref param);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetPortTimeout(IntPtr hDev, ref TimeoutParams param);
        public TimeoutParams GetPortTimeout()
        {
            TimeoutParams param = new TimeoutParams();
            int r = GetPortTimeout(devHandle, ref param);
            if (r != 0)
            {
                errorNo = r;
            }
            return param;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetPrinterStatus(IntPtr hDev, ref PrinterStatus status);
        public PrinterStatus? GetPrinterStatus()
        {
            PrinterStatus status = new PrinterStatus();
            status.self_size = Marshal.SizeOf(status);
            int r = GetPrinterStatus(devHandle, ref status);
            if(r != 0)
            {
                errorNo = r;
                return null;
            }
            return status;
        }
        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetPrinterInfo(IntPtr hDev, int infoID, StringBuilder buffer);
        public string GetPrinterInfo(int infoID)
        {
            StringBuilder buffer = new StringBuilder(128);
            int r = GetPrinterInfo(devHandle, infoID, buffer);
            if(r != 0)
            {
                errorNo = r;
            }
            return buffer.ToString();
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetPrintSpeed(IntPtr hDev, int speed);
        public int SetPrintSpeed(int speed)
        {
            int r = SetPrintSpeed(devHandle, speed);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetPrintDensity(IntPtr hDev, int density);
        public int SetPrintDensity(int density)
        {
            int r = SetPrintDensity(devHandle, density);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetTearOffset(IntPtr hDev, int offset);
        public int SetTearOffset(int offset)
        {
            int r = SetTearOffset(devHandle, offset);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetPrintMode(IntPtr hDev, int mode, int distance);
        public int SetPrintMode(int mode, int distance)
        {
            int r = SetPrintMode(devHandle, mode, distance);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetLabelOffset(IntPtr hDev, int hOffset, int vOffset);
        public int SetLabelOffset(int hOffset, int vOffset)
        {
            int r = SetLabelOffset(devHandle, hOffset, vOffset);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetPrintDirection(IntPtr hDev, int direction);
        public int SetPrintDirection(int direction)
        {
            int r = SetPrintDirection(devHandle, direction);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetPaperMode(IntPtr hDev, int mode);
        public int SetPaperMode(int mode)
        {
            int r = SetPaperMode(devHandle, mode);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetPrintMethod(IntPtr hDev, int method);
        public int SetPrintMethod(int method)
        {
            int r = SetPrintMethod(devHandle, method);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetColumn(IntPtr hDev, int columnNum, int space);
        public int SetColumn(int columnNum, int space)
        {
            int r = SetColumn(devHandle, columnNum, space);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetLabelSize(IntPtr hDev, int width, int height);
        public int SetLabelSize(int width, int height)
        {
            int r = SetLabelSize(devHandle, width, height);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetPrinterCodepage(IntPtr hDev, int codepage);
        public int SetPrinterCodepage(int codepage)
        {
            int r = SetPrinterCodepage(devHandle, codepage);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetEnglishFontName(IntPtr hDev, string fontName, int verticalOffset);
        public int SetEnglishFontName(string fontName, int verticalOffset)
        {
            int r = SetEnglishFontName(devHandle, fontName, verticalOffset);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        // label edit
        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int PrintText(IntPtr hDev, int x, int y, string fontName, byte [] text, int angle, int fontSizeH, int fontSizeV, int style);
        public int PrintText(int x, int y, string fontName, string text, int angle, int fontSizeH, int fontSizeV, int style)
        {
            byte[] bytes = Encoding.Default.GetBytes(text);
            return PrintText(x, y, fontName, bytes, angle, fontSizeH, fontSizeV, style);
        }
        public int PrintText(int x, int y, string fontName, byte[] text, int angle, int fontSizeH, int fontSizeV, int style)
        {
            int r = PrintText(devHandle, x, y, fontName, text, angle, fontSizeH, fontSizeV, style);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int PrintTrueTypeText(IntPtr hDev, int x, int y, string fontName, int fontWidth, int fontHeight, string text, int angle, int style);
        public int PrintTrueTypeText(int x, int y, string fontName, int fontWidth, int fontHeight, string text, int angle, int style)
        {
            int r = PrintTrueTypeText(devHandle, x, y, fontName, fontWidth, fontHeight, text, angle, style);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int PrintBarcode1D(IntPtr hDev, int x, int y, int barcodeType, int rotate, string content, int height, int HRI, int narrowBarWidth, int wideBarWidth);
        public int PrintBarcode1D(int x, int y, int barcodeType, int rotate, string content, int height, int HRI, int narrowBarWidth, int wideBarWidth)
        {
            int r = PrintBarcode1D(devHandle, x, y, barcodeType, rotate, content, height, HRI, narrowBarWidth, wideBarWidth);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int PrintImageFile(IntPtr hDev, int x, int y, string imagePath);
        public int PrintImageFile(int x, int y, string imagePath)
        {
            int r = PrintImageFile(devHandle, x, y, imagePath);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int PrintRectangle(IntPtr hDev, int x, int y, int width, int height, int thickness);
        public int PrintRectangle(int x, int y, int width, int height, int thickness)
        {
            int r = PrintRectangle(devHandle, x, y, width, height, thickness);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int PrintLine(IntPtr hDev, int startX, int startY, int endX, int endY, int thickness);
        public int PrintLine(int startX, int startY, int endX, int endY, int thickness)
        {
            int r = PrintLine(devHandle, startX, startY, endX, endY, thickness);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int PrintBarcodePDF417(IntPtr hDev, int x, int y, int rotate, byte[] content, int contentLen, int securityLevel, int moduleWidth, int moduleHeight, int row, int col);
        public int PrintBarcodePDF417(int x, int y, int rotate, string content, int securityLevel, int moduleWidth, int moduleHeight, int row, int col)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            int r = PrintBarcodePDF417(devHandle, x, y, rotate, bytes, bytes.Length, securityLevel, moduleWidth, moduleHeight, row, col);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }
        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int PrintBarcodeQR(IntPtr hDev, int x, int y, int rotate, byte[] content, int contentLen, char ECCLever, int cellWidth, int model);
        public int PrintBarcodeQR(int x, int y, int rotate, string content, char ECCLever, int cellWidth, int model)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            int r = PrintBarcodeQR(devHandle, x, y, rotate, bytes, bytes.Length, ECCLever, cellWidth, model);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int PrintLabel(IntPtr hDev, int labelNum, int copyNum);
        public int PrintLabel(int labelNum, int copyNum)
        {
            int r = PrintLabel(devHandle, labelNum, copyNum);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FeedLabel(IntPtr hDev);
        public int FeedLabel()
        {
            int r = FeedLabel(devHandle);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int CalibrateLabel(IntPtr hDev);
        public int CalibrateLabel()
        {
            int r = CalibrateLabel(devHandle);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }


        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int PrintSelfCheckingPage(IntPtr hDev);
        public int PrintSelfCheckingPage()
        {
            int r = PrintSelfCheckingPage(devHandle);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int DownloadImage(IntPtr hDev,  string imagePath, string nameForStored, int methord, int customThreshold);
        public int DownloadImage(string imagePath, string nameForStored, int methord, int customThreshold)
        {
            int r = DownloadImage(devHandle, imagePath , nameForStored, methord, customThreshold);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

        [DllImport(SDK_Dll_Path, CallingConvention = CallingConvention.Cdecl)]
        private static extern int PrintStoredImage(IntPtr hDev, int x, int y, string nameForStored);
        public int PrintStoredImage(int x, int y, string nameForStored)
        {
            int r = PrintStoredImage(devHandle, x, y, nameForStored);
            if (r != 0)
            {
                errorNo = r;
            }
            return r;
        }

    }
}
