#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it.
    More info at
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
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
#endregion License information (SeeingSharp and all based games/applications)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using Microsoft.Kinect.Input;
using Microsoft.Kinect.Wpf.Controls;
using SeeingSharp.RKKinectLounge.Base;
using SeeingSharp.Util;

namespace SeeingSharp.RKKinectLounge.Base
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
            while (actObject != null)
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
            if (topLevelControl != null)
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