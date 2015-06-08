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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SeeingSharp.Checking;
using SeeingSharp.Util;

namespace RKVideoMemory.Behaviors
{
    /// <summary>
    /// This component is responsible to hide the menubar of the mainscreen
    /// when the user is inactive for a view seconds.
    /// </summary>
    public class HideControlOnInactivityBehavior : Component
    {
        private const string CATEGORY_BEHAVIOR = "Behavior";
        private const int INACTIVITY_SECS_DEFAULT = 2;
        private const int TIMER_INTERVAL_MS = 300;
        private const int MIN_MOUSE_MOVE_PIXELS = 10;
        private const int MIN_MOUSE_Y_POS = 75;

        #region Associated controls and configuration
        private Control m_observedControl;
        private Control m_controlToHide;
        private double m_inactivitySeconds = INACTIVITY_SECS_DEFAULT;
        private bool m_isHidingActive;
        #endregion Associated controls and configuration

        #region Local resources
        private Timer m_refreshTimer;
        #endregion Local resources

        #region State variables
        private DateTime m_lastMouseMove;
        private Point m_lastMouseLocation;
        #endregion State variables

        /// <summary>
        /// Initializes a new instance of the <see cref="HideControlOnInactivityBehavior"/> class.
        /// </summary>
        /// <param name="container">The container in which this component should be encapsulated.</param>
        public HideControlOnInactivityBehavior(IContainer container)
        {
            container.EnsureNotNull("container");
            container.Add(this);

            m_refreshTimer = new Timer(container);
            m_refreshTimer.Interval = TIMER_INTERVAL_MS;
            m_refreshTimer.Tick += OnRefreshTimer_Tick;

            m_lastMouseMove = DateTime.UtcNow;
            m_isHidingActive = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HideControlOnInactivityBehavior"/> class.
        /// </summary>
        public HideControlOnInactivityBehavior()
        {
            m_refreshTimer = new Timer();
            m_refreshTimer.Interval = TIMER_INTERVAL_MS;
            m_refreshTimer.Tick += OnRefreshTimer_Tick;

            m_lastMouseMove = DateTime.UtcNow;
        }

        private void UpdateTargetControlVisibility()
        {
            if(!m_isHidingActive)
            {
                m_controlToHide.Visible = true;
                return;
            }

            if (DateTime.UtcNow - m_lastMouseMove > TimeSpan.FromSeconds(m_inactivitySeconds))
            {
                m_controlToHide.Visible = false;
            }
            else
            {
                m_controlToHide.Visible = true;
            }
        }

        /// <summary>
        /// Default dispose logic.
        /// </summary>
        /// <param name="disposing">Are we called by code or garbage collector?</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                CommonTools.SafeDispose(ref m_refreshTimer);
            }
        }

        /// <summary>
        /// Cyclic refresh call.
        /// This one is responsible for show/hide logic.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnRefreshTimer_Tick(object sender, EventArgs e)
        {
            if (m_observedControl == null) { return; }
            if (m_controlToHide == null) { return; }

            UpdateTargetControlVisibility();
        }

        private void OnObservedControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Location.Y > MIN_MOUSE_Y_POS) { return; }

            if ((Math.Abs(e.Location.X - m_lastMouseLocation.X) < MIN_MOUSE_MOVE_PIXELS) ||
                (Math.Abs(e.Location.Y - m_lastMouseLocation.Y) < MIN_MOUSE_MOVE_PIXELS))
            {
                return;
            }

            m_lastMouseLocation = e.Location;
            m_lastMouseMove = DateTime.UtcNow;

            UpdateTargetControlVisibility();
        }

        [Category(CATEGORY_BEHAVIOR)]
        [DefaultValue(null)]
        public Control ObservedControl
        {
            get { return m_observedControl; }
            set
            {
                if (m_observedControl != null)
                {
                    m_observedControl.MouseMove -= OnObservedControl_MouseMove;
                    m_refreshTimer.Stop();
                }
                m_observedControl = value;
                if (m_observedControl != null)
                {
                    m_observedControl.MouseMove += OnObservedControl_MouseMove;
                    m_refreshTimer.Start();
                }

                // Reset all state if we got an empty control
                if (m_observedControl == null)
                {
                    m_lastMouseLocation = Point.Empty;
                    m_lastMouseMove = DateTime.UtcNow;
                }
            }
        }

        [Category(CATEGORY_BEHAVIOR)]
        [DefaultValue(null)]
        public Control ControlToHide
        {
            get { return m_controlToHide; }
            set { m_controlToHide = value; }
        }

        [Category(CATEGORY_BEHAVIOR)]
        [DefaultValue(INACTIVITY_SECS_DEFAULT)]
        public double InactivitySecs
        {
            get { return m_inactivitySeconds; }
            set
            {
                value.EnsurePositiveAndNotZero("value");
                m_inactivitySeconds = value;
            }
        }

        [Browsable(false)]
        public bool IsHidingActive
        {
            get { return m_isHidingActive; }
            set
            {
                if(m_isHidingActive != value)
                {
                    m_isHidingActive = value;
                    m_lastMouseMove = DateTime.UtcNow;
                    this.UpdateTargetControlVisibility();
                }
            }
        }
    }
}