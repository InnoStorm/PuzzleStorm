using System;
using System.Globalization;

namespace Client {
    public class BoolToColorConverter : BaseValueConverter<BoolToColorConverter> {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool) value)
                return System.Windows.Media.Brushes.Green;
            else
                return System.Windows.Media.Brushes.Red;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
