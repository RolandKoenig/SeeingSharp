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

	********************************************************************************
    Additional permission under GNU GPL version 3 section 7

    If you modify this Program, or any covered work, by linking or combining it with 
	DirectX (or a modified version of that library), containing parts covered by the 
	terms of [name of library's license], the licensors of this Program grant you additional 
	permission to convey the resulting work. {Corresponding Source for a non-source form of 
	such a combination shall include the source code for the parts of DirectX used 
	as well as that of the covered work.}
*/
#endregion
#if DESKTOP
using PropertyTools.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace FrozenSky.Behaviors
{
    public class FrozenSkyPropertyGridAutoRefreshBehavior : Behavior<PropertyGrid>
    {
        private DispatcherTimer m_refreshTimer;
        private TimeSpan m_interval;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrozenSkyPropertyGridAutoRefreshBehavior"/> class.
        /// </summary>
        public FrozenSkyPropertyGridAutoRefreshBehavior()
        {
            m_interval = TimeSpan.FromSeconds(1.0);
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

            if(m_refreshTimer == null)
            {
                m_refreshTimer = new DispatcherTimer(DispatcherPriority.Background);
                m_refreshTimer.Interval = m_interval;
                m_refreshTimer.Tick += OnRefreshTimerTick;
                m_refreshTimer.Start();
            }
        }

        private void OnRefreshTimerTick(object sender, EventArgs e)
        {
            PropertyGrid targetGrid = base.AssociatedObject;
            if((targetGrid != null) && (!targetGrid.IsKeyboardFocusWithin))
            {
                object selectedObject = targetGrid.SelectedObject;
                targetGrid.SelectedObject = null;
                targetGrid.SelectedObject = selectedObject;
            }
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

            if(m_refreshTimer != null)
            {
                m_refreshTimer.Stop();
                m_refreshTimer = null;
            }
        }

        /// <summary>
        /// Gets or sets the refresh interval.
        /// </summary>
        public TimeSpan RefreshInterval
        {
            get { return m_interval; }
            set
            {
                if(m_interval > TimeSpan.Zero)
                {
                    m_interval = value;
                    if (m_refreshTimer != null) { m_refreshTimer.Interval = value; }
                }
            }
        }
    }
}
#endif