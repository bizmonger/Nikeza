using System;
using System.Globalization;
using System.Windows.Data;

namespace Desktop.App.Converters
{
    class NullableToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            ! (value is null);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
