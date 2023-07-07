using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PrintManager.UI.Converters
{
    public class IntColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Brush))
                throw new InvalidOperationException("The target must be a Brush");

            int type = (int)value;
            switch (type)
            {
                case 0:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
                case 1:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF962D"));
                case 2:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                case 3:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF00"));
                default:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
