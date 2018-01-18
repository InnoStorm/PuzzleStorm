﻿using System;
using System.Globalization;

namespace Client {

    public class OnTheMoveToVisibilityConverter : BaseValueConverter<OnTheMoveToVisibilityConverter> {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool) value)
                return System.Windows.Visibility.Visible;
            else
                return System.Windows.Visibility.Hidden;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
