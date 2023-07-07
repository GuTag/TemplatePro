using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PrintManager.UI.Converters
{
    public class BooleanModeColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Brush))
                throw new InvalidOperationException("The target must be a Brush");

            if ((bool)value)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF00"));
            }
            else
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF00"));
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
