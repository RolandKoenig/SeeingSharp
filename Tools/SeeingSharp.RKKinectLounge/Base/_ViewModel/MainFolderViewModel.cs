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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SeeingSharp.Infrastructure;
using SeeingSharp.Util;

namespace SeeingSharp.RKKinectLounge.Base
{
    public class MainFolderViewModel : FolderViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainFolderViewModel"/> class.
        /// </summary>
        public MainFolderViewModel(string mainPath)
            : base(null, mainPath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainFolderViewModel"/> class.
        /// </summary>
        public MainFolderViewModel()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Creates the new ViewModel with the same target (.. same constructor arguments).
        /// </summary>
        public override NavigateableViewModelBase CreateNewWithSameTarget()
        {
            return new MainFolderViewModel(base.BasePath);
        }
    }
}