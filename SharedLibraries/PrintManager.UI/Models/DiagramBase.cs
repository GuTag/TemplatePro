using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PrintManager.UI.Models
{
    public class DiagramBase : DiagramSerise
    {
        /// <summary>
        /// 长方体
        /// </summary>
        public Rectangle Rect1 { get; set; }

        /// <summary>
        /// 长方体
        /// </summary>
        public Rectangle Rect2 { get; set; }

        /// <summary>
        /// 折线
        /// </summary>
        public Polyline Line { get; set; }

        /// <summary>
        /// 起点
        /// </summary>
        public Point PosX { get; set; }

        /// <summary>
        /// 文字
        /// </summary>
        public Path TextPath { get; set; }
    }
}
