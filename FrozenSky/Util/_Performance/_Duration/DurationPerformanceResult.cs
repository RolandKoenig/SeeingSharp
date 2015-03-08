#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2014 Roland König (RolandK)

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
#if !WINDOWS_PHONE
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Util
{
    public class DurationPerformanceResult : PerformanceAnalyzeResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DurationPerformanceResult"/> class.
        /// </summary>
        public DurationPerformanceResult(DurationPerformanceCalculator calculator, DateTime timestampKey, long sumAvgTicks, long sumMaxTicks, long sumMinTicks)
            : base(calculator, timestampKey)
        {
            this.SumAverageTicks = sumAvgTicks;
            this.SumMaxTicks = sumMaxTicks;
            this.SumMinTicks = sumMinTicks;
        }

        [Browsable(false)]
        public long SumMaxTicks { get; set; }

        [Browsable(false)]
        public long SumAverageTicks { get; set; }

        [Browsable(false)]
        public long SumMinTicks { get; set; }

        /// <summary>
        /// Gets the FPS value.
        /// </summary>
        public int Fps
        {
            get
            {
                if (this.SumAverageTicks == 0) { return 0; }
                return (int)(10000000L / this.SumAverageTicks);
            }
        }

        [Browsable(false)]
        public TimeSpan SumMax
        {
            get { return TimeSpan.FromTicks(SumMaxTicks); }
        }

        [Browsable(false)]
        public long SumMaxMS
        {
            get { return (long)Math.Round(this.SumMax.TotalMilliseconds); }
        }

        [Browsable(false)]
        public TimeSpan SumMin
        {
            get { return TimeSpan.FromTicks(SumMinTicks); }
        }

        [Browsable(false)]
        public long SumMinMS
        {
            get { return (long)Math.Round(this.SumMin.TotalMilliseconds); }
        }

        public TimeSpan SumAverage
        {
            get { return TimeSpan.FromTicks(SumAverageTicks); }
        }

        public long SumAverageMS
        {
            get { return (long)Math.Round(this.SumAverage.TotalMilliseconds); }
        }

        /// <summary>
        /// Gets the average millisecond value as a double.
        /// </summary>
        
        public double SumAverageMSDouble
        {
            get { return this.SumAverage.TotalMilliseconds; }
        }
    }
}
#endif