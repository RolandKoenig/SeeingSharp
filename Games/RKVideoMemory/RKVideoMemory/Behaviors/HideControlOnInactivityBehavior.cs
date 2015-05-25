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
        private const int INACTIVITY_SECS_DEFAULT = 5;
        private const int TIMER_INTERVAL_MS = 300;
        private const int MIN_MOUSE_MOVE_PIXELS = 30;

        #region Associated controls and configuration
        private Control m_observedControl;
        private Control m_controlToHide;
        private double m_inactivitySeconds = INACTIVITY_SECS_DEFAULT;
        #endregion Associated controls and configuration

        #region Local resources
        private Timer m_refreshTimer;
        #endregion Local resources

        #region State variables
        private DateTime m_lastMouseMove;
        private Point m_lastMouseLocation;
        private bool m_focused;
        private bool m_isMouseInside;
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HideControlOnInactivityBehavior"/> class.
        /// </summary>
        public HideControlOnInactivityBehavior()
        {
            m_refreshTimer = new Timer();
            m_refreshTimer.Interval = TIMER_INTERVAL_MS;
            m_refreshTimer.Tick += OnRefreshTimer_Tick;
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

            if ((m_focused) && (m_isMouseInside) &&
                (DateTime.UtcNow - m_lastMouseMove > TimeSpan.FromSeconds(m_inactivitySeconds)))
            {
                m_controlToHide.Visible = false;
            }
            else
            {
                m_controlToHide.Visible = true;
            }
        }

        private void OnObservedControl_LostFocus(object sender, EventArgs e)
        {
            m_focused = false;
        }

        private void OnObservedControl_GotFocus(object sender, EventArgs e)
        {
            m_focused = true;
        }

        private void OnObservedControl_MouseMove(object sender, MouseEventArgs e)
        {
            if ((Math.Abs(e.Location.X - m_lastMouseLocation.X) < MIN_MOUSE_MOVE_PIXELS) ||
               (Math.Abs(e.Location.Y - m_lastMouseLocation.Y) < MIN_MOUSE_MOVE_PIXELS))
            {
                return;
            }

            m_lastMouseLocation = e.Location;
            m_lastMouseMove = DateTime.UtcNow;
        }

        private void OnObservedControl_MouseLeave(object sender, EventArgs e)
        {
            m_isMouseInside = false;
        }

        private void OnObservedControl_MouseEnter(object sender, EventArgs e)
        {
            m_isMouseInside = true;
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
                    m_observedControl.GotFocus -= OnObservedControl_GotFocus;
                    m_observedControl.LostFocus -= OnObservedControl_LostFocus;
                    m_refreshTimer.Stop();
                }
                m_observedControl = value;
                if (m_observedControl != null)
                {
                    m_observedControl.MouseMove += OnObservedControl_MouseMove;
                    m_observedControl.GotFocus += OnObservedControl_GotFocus;
                    m_observedControl.LostFocus += OnObservedControl_LostFocus;
                    m_observedControl.MouseEnter += OnObservedControl_MouseEnter;
                    m_observedControl.MouseLeave += OnObservedControl_MouseLeave;
                    m_refreshTimer.Start();
                }

                // Reset all state if we got an empty control
                if (m_observedControl == null)
                {
                    m_lastMouseLocation = Point.Empty;
                    m_focused = false;
                    m_lastMouseMove = DateTime.MinValue;
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
    }
}