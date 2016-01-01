#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

#if DESKTOP
using System.Windows.Data;
#elif UNIVERSAL
using Windows.UI.Xaml.Data;
#endif

namespace SeeingSharp.Util
{
    public class Vector2ToMultilineStringConverter : IValueConverter
    {
        private const string START_X = "X: ";
        private const string START_Y = "Y: ";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Vector2)) { throw new ArgumentException("Invalid source type, expected Vector2!"); }
            if (!(targetType == typeof(string))) { throw new ArgumentException("Invalid target type, expected String!"); }

            Vector2 sourceValue = (Vector2)value;
        
            StringBuilder resultBuilder = new StringBuilder(512);
            resultBuilder.Append(START_X + sourceValue.X.ToString(culture.NumberFormat) + Environment.NewLine);
            resultBuilder.Append(START_Y + sourceValue.Y.ToString(culture.NumberFormat));
            return resultBuilder.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is String)) { throw new ArgumentException("Invalid source type, expected String!"); }
            if (!(targetType == typeof(Vector2))) { throw new ArgumentException("Invalid target type, expected Vector2!"); }

            String sourceValue = value as String;
            string[] components = sourceValue.Split(
                new string[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            if (components.Length != 2) { throw new ArgumentException("Invalid count of components!"); }

            try
            {
                Vector2 result = new Vector2();
                result.X = float.Parse(components[0].Replace(START_X, ""), culture.NumberFormat);
                result.Y = float.Parse(components[1].Replace(START_Y, ""), culture.NumberFormat);
                return result;
            }
            catch(Exception ex)
            {
                throw new SeeingSharpException("Error while parsing vector data: " + ex.Message, ex);
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
