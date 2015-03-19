using FrozenSky.RKKinectLounge.Base;
using FrozenSky.Util;
using Microsoft.Kinect.Input;
using Microsoft.Kinect.Wpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace FrozenSky.RKKinectLounge.Base
{
    public class NavigateForwardToBindingBehavior : Behavior<Button>
    {
        // Using a DependencyProperty as the backing store for NavigationTargetObject.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigationTargetObjectProperty =
            DependencyProperty.Register("NavigationTargetObject", typeof(NavigateableViewModelBase), typeof(NavigateForwardToBindingBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Wird nach dem Anfügen des Verhaltens an das "AssociatedObject" aufgerufen.
        /// </summary>
        /// <remarks>
        /// Setzen Sie dies außer Kraft, um die Funktionalität in das "AssociatedObject" einzubinden.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            base.AssociatedObject.Click += OnAssociatedObject_Click;
        }

        /// <summary>
        /// Wird aufgerufen, wenn das Verhalten vom "AssociatedObject" getrennt wird. Der Aufruf erfolgt vor dem eigentlichen Trennvorgang.
        /// </summary>
        /// <remarks>
        /// Setzen Sie dies außer Kraft, um die Bindung der Funktionalität zum "AssociatedObject" zu lösen.
        /// </remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            base.AssociatedObject.Click -= OnAssociatedObject_Click;
        }

        /// <summary>
        /// Called when user clicked on the element which we are observing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e"></param>
        private void OnAssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            // Get the ViewModel to which we have to navigate
            NavigateableViewModelBase navigationTarget = this.NavigationTargetObject;
            if (navigationTarget == null) { navigationTarget = base.AssociatedObject.DataContext as NavigateableViewModelBase; }
            if (navigationTarget == null) { return; }

            // Query for the toplevel control
            DependencyObject actObject = base.AssociatedObject;
            DependencyObject lastObject = null;
            while(actObject != null)
            {
                lastObject = actObject;
                actObject = VisualTreeHelper.GetParent(actObject);

                // Break on MainWindow elements
                FrameworkElement actFrameworkElement = actObject as FrameworkElement;
                if ((actFrameworkElement != null) && (actFrameworkElement.DataContext is MainWindowViewModel))
                {
                    actObject = null;
                }
            }

            // Apply new DataContext on the toplevel control
            FrameworkElement topLevelControl = lastObject as FrameworkElement;
            if(topLevelControl != null)
            {
                topLevelControl.DataContext = navigationTarget;
            }
        }

        /// <summary>
        /// A custom navigation target object which can be assigned using Xaml.
        /// </summary>  
        public NavigateableViewModelBase NavigationTargetObject
        {
            get { return (NavigateableViewModelBase)GetValue(NavigationTargetObjectProperty); }
            set { SetValue(NavigationTargetObjectProperty, value); }
        }
    }
}
