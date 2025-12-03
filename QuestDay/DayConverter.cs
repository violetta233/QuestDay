using System;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.Maui.Controls;





namespace QuestDay
{
    public class DayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var selectedDays = value as HashSet<string>;
            var day = parameter as string;

            // Получаем цвета из локальных ресурсов страницы
            var baseColor = (Color)Application.Current.Resources["baseColor"];
            var selectedColor = (Color)Application.Current.Resources["selectedColor"];

            // Добавляем проверку на наличие ресурсов
            if (baseColor == null || selectedColor == null)
            {
                throw new ArgumentException("Цвета не найдены в ресурсах");
            }

            return selectedDays?.Contains(day) == true ? selectedColor : baseColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}





