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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrozenSky.Util
{
    public class TplBasedLoop
    {
        private Task m_runningTask;
        private CancellationToken m_runningTaskCancelToken;
        private int m_refreshIntervalMS;

        public event EventHandler Tick;

        /// <summary>
        /// Initializes a new instance of the <see cref="TplBasedLoop"/> class.
        /// </summary>
        public TplBasedLoop(int refreshIntervalMS = 1000)
        {
            m_refreshIntervalMS = refreshIntervalMS;
        }

        /// <summary>
        /// Starts running the mainloop.
        /// </summary>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        public Task RunAsync(CancellationToken cancelToken)
        {
            if (m_runningTask != null) { throw new InvalidOperationException("MyObjectWithCyclicRefresh: Mainloop is already running!"); }

            m_runningTaskCancelToken = cancelToken;
            bool isFirstPass = true;
            m_runningTask = Task.Factory.StartNew(async () =>
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    if (isFirstPass)
                    {
                        isFirstPass = false;
                        await Task.Delay(1);
                    }
                    else
                    {
                        Tick.Raise(this, EventArgs.Empty);

                        // Wait currently configured delay time
                        if (m_refreshIntervalMS > 0)
                        {
                            await Task.Delay(m_refreshIntervalMS).ConfigureAwait(false);
                        }
                    }
                }

                // Reset threading members when we are finished
                m_runningTask = null;
                m_runningTaskCancelToken = CancellationToken.None;
            }).Result;

            return m_runningTask;
        }
    }
}
