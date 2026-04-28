using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Linq;
using Microsoft.Maui.Controls;

namespace QuestDay.Converters
{
    public class RussianListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Collections.IList list && list.Count > 0)
            {
                var russianCulture = new CultureInfo("ru-RU");
                var items = new List<string>();

                foreach (var item in list)
                {
                    if (item is DayOfWeek day)
                    {
                        items.Add(russianCulture.DateTimeFormat.GetDayName(day));
                    }
                    else
                    {
                        items.Add(item?.ToString() ?? "");
                    }
                }

                var separator = parameter as string ?? ", ";
                return string.Join(separator, items);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
