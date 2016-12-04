#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
	Exception are projects where it is noted otherwhise.
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using SeeingSharp.Checking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeeingSharp.Util
{
    public static partial class CommonTools
    {
        /// <summary>
        /// Executes the given action after the given amount of time.
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        /// <param name="delayTime">The delay time to be passed before executing the given action.</param>
        /// <param name="forceValidSyncContext">True to force a valid (UI) SynchronizationContext.</param>
        public static async void InvokeDelayed(Action action, TimeSpan delayTime, bool forceValidSyncContext = true)
        {
            action.EnsureNotNull(nameof(action));
            delayTime.EnsureLongerThanZero(nameof(delayTime));

            // Gets current Synchronization context
            if (forceValidSyncContext)
            {
                SynchronizationContext syncContext = SynchronizationContext.Current;
                if (syncContext == null)
                {
                    throw new SeeingSharpException("No SynchronizationContext is available on current thread!");
                }
                if (syncContext.GetType() == typeof(SynchronizationContext))
                {
                    throw new SeeingSharpException("This method is not available on default synchronization context!");
                }
            }

            // Wait specified time
            await Task.Delay(delayTime);

            action();
        }
    }
}
