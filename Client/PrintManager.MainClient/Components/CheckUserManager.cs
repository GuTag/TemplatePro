using System.Runtime.InteropServices;
using System.Windows.Threading;
using System;
using System.Windows;

public class CheckUsedManager
{
    public static event EventHandler TimeToFinishedEvent = null;

    private static DispatcherTimer checkUsedTimer = new DispatcherTimer();

    private static Point mousePosition = GetMousePoint();

    static CheckUsedManager()
    {
        checkUsedTimer.Interval = TimeSpan.FromSeconds(600);

        checkUsedTimer.Tick += new EventHandler(CheckUsedTimer_Tick);

        checkUsedTimer.Start();
    }

    static void CheckUsedTimer_Tick(object sender, EventArgs e)
    {
        if (!HaveUsedTo())
        {
            if (TimeToFinishedEvent != null)
            {
                TimeToFinishedEvent(null, null);
            }
        }
    }

    private static bool HaveUsedTo()
    {
        Point point = GetMousePoint();

        if (point == mousePosition)
        {
            return false;
        }

        mousePosition = point;

        return true;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MPoint
    {
        public int X;

        public int Y;

        public MPoint(int x, int y)
        {
            this.X = x;

            this.Y = y;
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetCursorPos(out MPoint mpt);
    /// <summary>
    /// 获取当前屏幕鼠标位置
    /// </summary>
    /// <returns></returns>
    public static Point GetMousePoint()
    {
        MPoint mpt = new MPoint();

        GetCursorPos(out mpt);

        Point p = new Point(mpt.X, mpt.Y);

        return p;
    }
}