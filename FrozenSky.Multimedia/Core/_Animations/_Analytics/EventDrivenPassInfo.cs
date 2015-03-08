using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Multimedia.Core
{
    /// <summary>
    /// Holds some detail information about event-driven animation calculation.
    /// </summary>
    public class EventDrivenPassInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventDrivenPassInfo"/> class.
        /// </summary>
        /// <param name="steps">All steps performed in this calculation.</param>
        internal EventDrivenPassInfo(List<EventDrivenStepInfo> steps)
        {
            this.CountSteps = steps.Count;
            this.Steps = steps;
        }

        public List<EventDrivenStepInfo> Steps
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the total time the animation took (simulation time).
        /// </summary>
        public TimeSpan TotalTime
        {
            get
            {
                if (Steps == null) { return TimeSpan.Zero; }

                TimeSpan totalTime = TimeSpan.Zero;
                foreach(var actAnimStep in this.Steps)
                {
                    totalTime = totalTime + actAnimStep.UpdateTime;
                }

                return totalTime;
            }
        }

        /// <summary>
        /// Total count of calculation steps.
        /// </summary>
        public int CountSteps
        {
            get;
            private set;
        }
    }
}
