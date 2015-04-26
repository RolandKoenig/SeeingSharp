#region License information (SeeingSharp and all based games/applications)
/*
    SeeingSharp and all games/applications based on it (more info at http://www.rolandk.de/wp)
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
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Util;

namespace SeeingSharp.Multimedia.Core
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
