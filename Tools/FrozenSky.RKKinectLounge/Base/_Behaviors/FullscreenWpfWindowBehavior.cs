using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace FrozenSky.RKKinectLounge.Base
{ 
    public class FullscreenWpfWindowBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            base.AssociatedObject.Loaded += OnAssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            base.AssociatedObject.Loaded -= OnAssociatedObject_Loaded;
        }

        /// <summary>
        /// Handle the load event of the associated window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnAssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            // Change window style and state to apply fullscreen mode
            base.AssociatedObject.WindowStyle = System.Windows.WindowStyle.None;
            base.AssociatedObject.WindowState = System.Windows.WindowState.Maximized;
        }
    }
}
