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
