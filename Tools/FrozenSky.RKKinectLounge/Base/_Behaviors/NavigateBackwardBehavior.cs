using FrozenSky.Infrastructure;
using FrozenSky.RKKinectLounge.Base;
using FrozenSky.Util;
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
    public class NavigateBackwardBehaviorBehavior : Behavior<Button>
    {
        /// <summary>
        /// Gets the element which hosts the ViewModel.
        /// </summary>
        private FrameworkElement TryGetViewModelHostElement()
        {
            if (this.AssociatedObject == null) { return null; }

            // Query for the toplevel control
            DependencyObject actObject = base.AssociatedObject;
            DependencyObject lastObject = null;
            while (actObject != null)
            {
                lastObject = actObject;
                actObject = VisualTreeHelper.GetParent(actObject);

                // Break on MainWindow elements
                FrameworkElement actFrameworkElement = actObject as FrameworkElement;
                if((actFrameworkElement != null) && (actFrameworkElement.DataContext is MainWindowViewModel))
                {
                    actObject = null;
                }
            }

            // Check whether we have a toplevel control
            return lastObject as FrameworkElement;
        }

        /// <summary>
        /// Gets the current ViewModel.
        /// </summary>
        private NavigateableViewModelBase TryGetCurrentViewModel()
        {
            FrameworkElement topLevelControl = TryGetViewModelHostElement();
            if (topLevelControl == null) { return null; }

            // Get the current viewmodel
            return topLevelControl.DataContext as NavigateableViewModelBase;
        }

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
            // Get the current viewmodel
            FrameworkElement topLevelControl = TryGetViewModelHostElement();
            if (topLevelControl == null) { return; }

            NavigateableViewModelBase actViewModel = topLevelControl.DataContext as NavigateableViewModelBase;
            if (actViewModel == null) { return; }

            // Navigate to the parent viewmodel if there is any
            if(actViewModel.ParentViewModel != null)
            {
                topLevelControl.DataContext = actViewModel.ParentViewModel.CreateNewWithSameTarget();
            }
            else
            {
                FrozenSkyApplication.Current.UIMessenger.Publish<MessageManualExit>();
            }
        }
    }
}
