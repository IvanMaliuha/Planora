using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    using System;
    using System.Globalization;
    using System.Windows.Data;

    namespace Planora.UI.Converters
    {
        public class BoolToViewModeConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                // Якщо true — показуємо "Тиждень", інакше "День"
                if (value is bool isWeekView)
                    return isWeekView ? "Тиждень" : "День";

                return "Режим";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                // Конвертація назад не потрібна
                return Binding.DoNothing;
            }
        }
    }


