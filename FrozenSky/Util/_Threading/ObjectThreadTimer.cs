using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrozenSky.Util
{
    public class ObjectThreadTimer
    {
        private DateTime m_startTimeStamp;
        private TimeSpan m_timeSinceStart;
        private double m_speedFactor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectThreadTimer"/> class.
        /// </summary>
        public ObjectThreadTimer()
        {
            m_speedFactor = 1.0;
            m_startTimeStamp = DateTime.MinValue;
            m_timeSinceStart = TimeSpan.Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectThreadTimer"/> class.
        /// </summary>
        /// <param name="startTimeStamp">The start time stamp.</param>
        public ObjectThreadTimer(DateTime startTimeStamp)
        {
            m_startTimeStamp = startTimeStamp;
            m_timeSinceStart = TimeSpan.Zero;
            m_speedFactor = 1.0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectThreadTimer"/> class.
        /// </summary>
        /// <param name="startTimeStamp">The start time stamp.</param>
        /// <param name="speedFactor">Speed factor (standard: 1.0).</param>
        public ObjectThreadTimer(DateTime startTimeStamp, double speedFactor)
        {
            m_startTimeStamp = startTimeStamp;
            m_timeSinceStart = TimeSpan.Zero;
            m_speedFactor = speedFactor;
        }

        /// <summary>
        /// Adds the given timespan to the timer.
        /// </summary>
        internal void Add(TimeSpan timeSpan)
        {
            if (m_speedFactor == 1.0)
            {
                m_timeSinceStart = m_timeSinceStart.Add(timeSpan);
            }
            else
            {
                m_timeSinceStart = m_timeSinceStart.Add(TimeSpan.FromTicks((long)(timeSpan.Ticks * m_speedFactor)));
            }
        }

        /// <summary>
        /// Gets current time (thread-time, not pc-time!).
        /// </summary>
        public DateTime Now
        {
            get { return m_startTimeStamp.Add(m_timeSinceStart); }
        }

        /// <summary>
        /// Gets or sets current speed factor of the timer (default: 1.0).
        /// </summary>
        public double SpeedFactor
        {
            get { return m_speedFactor; }
            set
            {
                if (value < 0.0) { throw new ArgumentException("SpeedFactor can not be less than zero!", "value"); }
                m_speedFactor = value;
            }
        }
    }
}