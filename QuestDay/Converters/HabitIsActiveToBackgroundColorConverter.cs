using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace QuestDay.Converters
{
    public class HabitIsActiveToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
            {
                return isActive ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#F5F5F5");
            }
            return Color.FromArgb("#FFFFFF");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}