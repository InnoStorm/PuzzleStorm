using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Client {

    /// <summary>
    /// Base konverter koji omogucava direktno koriscenje u XAML
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter 
        where T : class, new()
    {
        #region Private Members

        /// <summary>
        /// Staticka instanca
        /// </summary>
        private static T mConverter = null;

        #endregion

        #region Markup extension metode

        /// <summary>
        /// Vraca staticku instancu
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return mConverter ?? (mConverter = new T());
        }

        #endregion

        #region Converter Metode

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

        #endregion
    }
}
