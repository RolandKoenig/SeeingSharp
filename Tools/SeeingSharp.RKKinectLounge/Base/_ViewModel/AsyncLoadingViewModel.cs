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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using SeeingSharp.Util;

namespace SeeingSharp.RKKinectLounge.Base
{
    public class AsyncLoadingViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLoadingViewModel"/> class.
        /// </summary>
        /// <param name="loadingTask">The loading task.</param>
        public AsyncLoadingViewModel(Task loadingTask)
        {
            if (Dispatcher.CurrentDispatcher == null) { throw new SeeingSharpException("Unable to create a AsyncLoadingViewModel if we are currently not on the UI thread!"); }
            if (loadingTask == null) { throw new ArgumentNullException("loadingTask"); }

            this.IsLoadingScreenVisible = false;

            loadingTask.ContinueWith(
                (task) =>
                {
                    this.IsLoadingScreenVisible = true;
                    base.RaisePropertyChanged(() => this.IsLoadingScreenVisible);
                },
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Tells the system whether we are loading content currently.
        /// </summary>
        public bool IsLoadingScreenVisible
        {
            get;
            set;
        }
    }
}