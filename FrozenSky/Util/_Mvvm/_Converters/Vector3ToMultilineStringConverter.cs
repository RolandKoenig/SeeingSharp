using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

#if DESKTOP
using System.Windows.Data;
#elif UNIVERSAL
using Windows.UI.Xaml.Data;
#endif

namespace FrozenSky.Util
{
    public class Vector3ToMultilineStringConverter : IValueConverter
    {
        private const string START_X = "X: ";
        private const string START_Y = "Y: ";
        private const string START_Z = "Z: ";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Vector3)) { throw new ArgumentException("Invalid source type, expected Vector3!"); }
            if (!(targetType == typeof(string))) { throw new ArgumentException("Invalid target type, expected String!"); }

            Vector3 sourceValue = (Vector3)value;
        
            StringBuilder resultBuilder = new StringBuilder(512);
            resultBuilder.Append(START_X + sourceValue.X.ToString(culture.NumberFormat) + Environment.NewLine);
            resultBuilder.Append(START_Y + sourceValue.Y.ToString(culture.NumberFormat) + Environment.NewLine);
            resultBuilder.Append(START_Z + sourceValue.Z.ToString(culture.NumberFormat));
            return resultBuilder.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is String)) { throw new ArgumentException("Invalid source type, expected String!"); }
            if (!(targetType == typeof(Vector3))) { throw new ArgumentException("Invalid target type, expected Vector3!"); }

            String sourceValue = value as String;
            string[] components = sourceValue.Split(
                new string[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            if (components.Length != 3) { throw new ArgumentException("Invalid count of components!"); }

            try
            {
                Vector3 result = new Vector3();
                result.X = float.Parse(components[0].Replace(START_X, ""), culture.NumberFormat);
                result.Y = float.Parse(components[1].Replace(START_Y, ""), culture.NumberFormat);
                result.Z = float.Parse(components[2].Replace(START_Z, ""), culture.NumberFormat);
                return result;
            }
            catch(Exception ex)
            {
                throw new FrozenSkyException("Error while parsing vector data: " + ex.Message, ex);
            }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return this.Convert(value, targetType, parameter, new CultureInfo(language));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return this.ConvertBack(value, targetType, parameter, new CultureInfo(language));
        }
    }
}
