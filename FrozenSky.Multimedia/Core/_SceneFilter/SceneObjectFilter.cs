#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
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
using System.Xml.Serialization;
using FrozenSky.Util;

namespace FrozenSky.Multimedia.Core
{
    public abstract class SceneObjectFilter
    {
        /// <summary>
        /// An event that notifies changed filter configuration.
        /// </summary>
        public event EventHandler FilterConfigurationChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneObjectFilter"/> class.
        /// </summary>
        public SceneObjectFilter()
        {

        }

        /// <summary>
        /// Checks for visibility of the given object.
        /// </summary>
        /// <param name="input">The object to be checked..</param>
        public abstract bool IsObjectVisible(SceneObject input, ViewInformation viewInformation);
        
        /// <summary>
        /// Sets some informational data telling the filter where it is used.
        /// </summary>
        /// <param name="layerToFilter">The SceneLayer that gets filtered.</param>
        /// <param name="viewInformation">The information object of the corresponding view.</param>
        public virtual void SetEnvironmentData(SceneLayer layerToFilter, ViewInformation viewInformation)
        {

        }

        /// <summary>
        /// Raises the FilterConfigurationChanged event.
        /// </summary>
        protected void RaiseFilterConfigurationChanged()
        {
            ConfigurationChangedUI = true;
            FilterConfigurationChanged.Raise(this, EventArgs.Empty);
        }

        /// <summary>
        /// Do update this filter on each frame?
        /// </summary>
        [XmlIgnore]
        public virtual bool UpdateEachFrame
        {
            get { return false; }
        }

        /// <summary>
        /// Has filter configuration changed?
        /// </summary>
        [XmlIgnore]
        internal bool ConfigurationChanged;

        /// <summary>
        /// Has filter configuration changed? (temporary flag on UI because of thread synchronization)
        /// </summary>
        [XmlIgnore]
        internal bool ConfigurationChangedUI;
    }
}
