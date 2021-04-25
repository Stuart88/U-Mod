using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace U_Mod.Converters
{
    public class ActiveColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool b)
            {
                return b 
                    ? Application.Current.TryFindResource("LightGrey") as SolidColorBrush 
                    : Application.Current.TryFindResource("LighterGrey") as SolidColorBrush;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Visibility v)
            {
                return v == Visibility.Collapsed;
            }

            return false;
        }
    }
}
