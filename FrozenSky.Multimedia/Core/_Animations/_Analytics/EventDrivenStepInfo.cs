using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrozenSky.Util;

namespace FrozenSky.Multimedia.Core
{
    public class EventDrivenStepInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventDrivenStepInfo"/> class.
        /// </summary>
        public EventDrivenStepInfo()
        {

        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return "" + AnimationCount + " Animations (Time: " + CommonTools.FormatTimespanCompact(UpdateTime) + ")";
        }

        public int AnimationCount
        {
            get;
            internal set;
        }

        public TimeSpan UpdateTime
        {
            get;
            internal set;
        }
    }
}
