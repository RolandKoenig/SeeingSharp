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
using System.Windows.Interactivity;

namespace SeeingSharp.RKKinectLounge.Base
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