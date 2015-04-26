using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SeeingSharp.RKKinectLounge.Base
{
    public class ViewModelExtensionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            NavigateableViewModelBase mainVM = value as NavigateableViewModelBase;
            if (mainVM == null) { throw new ArgumentException("Unsupported source value!"); }

            return mainVM.TryGetExtension(parameter as Type);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            INavigateableViewModelExtension givenVM = value as INavigateableViewModelExtension;
            if (givenVM == null) { throw new ArgumentException("Unsupported source value!"); }

            return givenVM.Owner;
        }
    }
}
