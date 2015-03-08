using FrozenSky.Util;
using PropertyTools.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;

namespace FrozenSky.Behaviors
{
    public class FrozenSkyPropertyGridBehavior : Behavior<PropertyGrid>
    {
        /// <summary>
        /// Wird nach dem Anfügen des Verhaltens an das "AssociatedObject" aufgerufen.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            base.AssociatedObject.PropertyControlFactory = new FrozenSkyPropertyControlFactory();
            base.AssociatedObject.PropertyItemFactory = new FrozenSkyPropertyItemFactory();
        }

        /// <summary>
        /// Wird aufgerufen, wenn das Verhalten vom "AssociatedObject" getrennt wird. Der Aufruf erfolgt vor dem eigentlichen Trennvorgang.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}
