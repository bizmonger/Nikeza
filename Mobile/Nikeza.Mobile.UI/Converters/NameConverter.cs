using System;
using System.Globalization;
using Xamarin.Forms;
using static Nikeza.Common;

namespace Nikeza.Mobile.UI.Converters
{
    public class NameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var profile = value as ProfileRequest;
            return $"{profile.FirstName} {profile.LastName}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}