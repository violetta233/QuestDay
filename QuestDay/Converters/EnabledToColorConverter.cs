using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace QuestDay.Converters
{
    public class EnabledToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEnabled)
            {
                return isEnabled ? Color.FromArgb("#4CAF50") : Color.FromArgb("#CCCCCC");
            }
            return Color.FromArgb("#CCCCCC");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}