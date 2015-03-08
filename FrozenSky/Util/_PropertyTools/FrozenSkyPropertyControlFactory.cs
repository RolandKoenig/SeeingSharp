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
