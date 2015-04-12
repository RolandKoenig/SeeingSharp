#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2015 Roland König (RolandK)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using PropertyTools.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace FrozenSky.Util
{
    public class FrozenSkyPropertyControlFactory : DefaultPropertyControlFactory
    {
        /// <summary>
        /// Creates the control for a property.
        /// </summary>
        /// <param name="property">The property item.</param>
        /// <param name="options">The options.</param>
        public override FrameworkElement CreateControl(PropertyItem property, PropertyControlFactoryOptions options)
        {
            if (property.ActualPropertyType == typeof(DelegateCommand))
            {
                return CreateCommandButton(property, options);
            }

            FrameworkElement result = base.CreateControl(property, options);
            result.MinWidth = 150;
            return result;
        }

        /// <summary>
        /// Creates the command button for the given property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="options">The options.</param>
        private FrameworkElement CreateCommandButton(PropertyItem property, PropertyControlFactoryOptions options)
        {
            Button result = new Button();
            result.SetBinding(Button.CommandProperty, property.PropertyName);
            result.Content = property.DisplayName;
            return result;
        }

        /// <summary>
        /// Creates the slider control.
        /// </summary>
        /// <param name="property">The property for which to create the control.</param>
        protected override FrameworkElement CreateSliderControl(PropertyItem property)
        {
            Slider slider = new Slider
            {
                Minimum = property.SliderMinimum,
                Maximum = property.SliderMaximum,
                SmallChange = property.SliderSmallChange,
                LargeChange = property.SliderLargeChange,
                TickFrequency = property.SliderTickFrequency,
                IsSnapToTickEnabled = property.SliderSnapToTicks
            };
            slider.SetBinding(RangeBase.ValueProperty, property.CreateBinding(UpdateSourceTrigger.Default, true));
            return slider;
        }
    }
}
