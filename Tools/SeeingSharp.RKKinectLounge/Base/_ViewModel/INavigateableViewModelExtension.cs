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
using System.Threading;
using System.Threading.Tasks;

namespace SeeingSharp.RKKinectLounge.Base
{
    public interface INavigateableViewModelExtension
    {
        /// <summary>
        /// Does this object extend the given object?
        /// Return false to ensure that no further logic is executed here.
        /// </summary>
        /// <param name="ownerViewModel">The owner view model.</param>
        bool ExtendsViewModelType(NavigateableViewModelBase ownerViewModel);

        /// <summary>
        /// Loads the preview content asynchronous.
        /// </summary>
        /// <param name="ownerViewModel">The owner view model.</param>
        /// <param name="cancelToken">The cancel token.</param>
        Task LoadPreviewContentAsync(NavigateableViewModelBase ownerViewModel, CancellationToken cancelToken);

        /// <summary>
        /// Loads all folder contents that belong to this extension.
        /// </summary>
        /// <param name="ownerViewModel">The ViewModel for which to load detail content.</param>
        /// <param name="cancelToken">The cancel token.</param>
        Task LoadDetailContentAsync(NavigateableViewModelBase ownerViewModel, CancellationToken cancelToken);

        /// <summary>
        /// Unloads all contents which are loaded currently.
        /// </summary>
        void Unload();

        /// <summary>
        /// Gets the short name of this ViewModel extension.
        /// </summary>
        string ShortName { get; }
    }
}