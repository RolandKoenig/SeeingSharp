#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
	Exception are projects where it is noted otherwhise.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Checking;
#if DESKTOP
using System.Windows;
#endif
#if UNIVERSAL
using Windows.UI.Xaml;
#endif

namespace SeeingSharp.View
{
    public static class Interaction
    {
        public static readonly DependencyProperty ViewServicesProperty =
            DependencyProperty.RegisterAttached("ShadowViewServices", typeof(ViewServiceCollection), typeof(Interaction), null);

        public static ViewServiceCollection GetViewServices(DependencyObject obj)
        {
            FrameworkElement hostElement = obj as FrameworkElement;
            hostElement.EnsureNotNull(nameof(hostElement));

            ViewServiceCollection triggerCollection = (ViewServiceCollection)obj.GetValue(Interaction.ViewServicesProperty);
            if (triggerCollection == null)
            {
                triggerCollection = new ViewServiceCollection(hostElement);
                obj.SetValue(Interaction.ViewServicesProperty, triggerCollection);
            }
            return triggerCollection;
        }
    }
}
