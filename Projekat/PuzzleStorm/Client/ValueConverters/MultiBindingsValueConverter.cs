using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Client {
    public class MultiBindingsValueConverter : MarkupExtension, IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<String> parameters = new List<string>();

            foreach (var obj in values)
            {
                if (obj is ComboBoxItem)
                    parameters.Add(((ComboBoxItem)obj).Content.ToString());
            }

            return parameters;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return new MultiBindingsValueConverter();
        }
    }
}
