using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PrintManager.UI.Controls
{
    public class TextControl : BaseControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(BaseControl));
        public TextControl(Canvas parent) : base(parent)
        {
            this.Width = 60;
            this.Height = 30;
            Text = "Text";
        }

        #region 属性
        public string Text
        {
            get
            {
                return ((string)GetValue(TextProperty));
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }
        #endregion

        #region 变量
        #endregion

        #region 事件
        #endregion

        #region 方法
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }
        #endregion
    }
}
