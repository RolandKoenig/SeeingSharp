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
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Checking;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Multimedia.Components
{
    public abstract class ObjectCreatorComponent<ContextType> : SceneComponent<ContextType>
        where ContextType : class
    {
        /// <summary>
        /// Creates the configured layer if it does not exist on the given scene.
        /// </summary>
        /// <param name="manipulator">The manipulator for manipulating the scene.</param>
        protected void CreateLayerIfNotAvailable(SceneManipulator manipulator)
        {
            this.TargetLayer.EnsureNotNullOrEmptyOrWhiteSpace(nameof(this.TargetLayer));

            if (!manipulator.ContainsLayer(this.TargetLayer))
            {
                SceneLayer bgLayer = manipulator.AddLayer(this.TargetLayer);
                manipulator.SetLayerOrderID(bgLayer, this.TargetLayerOrderID);
            }
        }

#if DESKTOP
        [Browsable(true)]
        [Category(Constants.DESIGNER_CATEGORY_GRADIENT)]
#endif
        public string TargetLayer
        {
            get;
            set;
        }

#if DESKTOP
        [Browsable(true)]
        [Category(Constants.DESIGNER_CATEGORY_GRADIENT)]
#endif
        public int TargetLayerOrderID
        {
            get;
            set;
        }
    }
}
