#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# Tools. More info at
     - https://github.com/RolandKoenig/SeeingSharp/tree/master/Tools (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)

	This program is distributed under the terms of the Microsoft Public License (Ms-PL)-
	More info at https://msdn.microsoft.com/en-us/library/ff647676.aspx
*/
#endregion License information (SeeingSharp and all based games/applications)

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SeeingSharpModelViewer
{
    public class NullableToVisibilityConverter : IValueConverter
    {
        public NullableToVisibilityConverter()
        {
            this.ResultOnNull = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility)) { throw new InvalidOperationException("Invalid target Type!"); }

            if (value != null) { return Visibility.Visible; }
            else { return ResultOnNull; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Convert back not supported!");
        }

        public Visibility ResultOnNull
        {
            get;
            set;
        }
    }
}