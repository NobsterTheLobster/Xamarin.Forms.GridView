using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Globalization;

namespace XamarinFormsGridView.Converters
{
    /// <summary>
    /// Converts a type into a boolean
    /// </summary>
    public class TypeToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Formats the string in accordance with the string format specified.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <param name="targetType">The desired type for the object.</param>
        /// <param name="parameter">Not required.</param>
        /// <param name="language">Not required.</param>
        /// <returns>The value object converted to the specified type.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Return true if the type name matches the parameters.
            return String.Compare(value?.GetType().Name, parameter.ToString()) == 0;
        }

        /// <summary>
        /// Required for interface implementation.
        /// </summary>
        /// <param name="value">Not defined.</param>
        /// <param name="targetType">Not defined.</param>
        /// <param name="parameter">Not defined.</param>
        /// <param name="language">Not defined.</param>
        /// <returns>Not defined.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Throw exception.
            return value;
        }
    }
}
