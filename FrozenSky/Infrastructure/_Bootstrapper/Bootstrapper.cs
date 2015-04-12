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
using System.Threading.Tasks;
using FrozenSky.Util;

namespace FrozenSky.Infrastructure
{
    public class Bootstrapper : PropertyChangedBase
    {
        private IBootstrapperItem m_currentItem;
        private List<IBootstrapperItem> m_items;
        private bool m_booted;

        public event EventHandler<BootstrapperItemArgs> ItemExecuted;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper" /> class.
        /// </summary>
        public Bootstrapper()
        {
            m_items = new List<IBootstrapperItem>();
        }

        /// <summary>
        /// Loads all bootstrapper items
        /// </summary>
        internal void LoadBootstrapperItems()
        {
            m_items.AddRange(FrozenSkyApplication.Current.TypeQuery.GetAndInstanciateByContract<IBootstrapperItem>());
            m_currentItem = null;
            m_booted = false;
        }

        /// <summary>
        /// Runs all items within this bootstrapper.
        /// </summary>
        internal async Task Run()
        {
            foreach (IBootstrapperItem actItem in m_items)
            {
                //Update current item property
                this.CurrentItem = actItem;

                //Execute the item
                await actItem.Execute();

                //Raise item executed event
                ItemExecuted.Raise(this, new BootstrapperItemArgs(actItem));
            }

            this.CurrentItem = null;
            this.Booted = true;
        }

        /// <summary>
        /// Are all items finished?
        /// </summary>
        public bool Booted
        {
            get { return m_booted; }
            private set
            {
                if (m_booted != value)
                {
                    m_booted = value;
                    base.RaisePropertyChanged(() => this.Booted);
                }
            }
        }

        /// <summary>
        /// Gets the currently executing item.
        /// </summary>
        public IBootstrapperItem CurrentItem
        {
            get { return m_currentItem; }
            private set
            {
                if (m_currentItem != value)
                {
                    m_currentItem = value;
                    base.RaisePropertyChanged(() => CurrentItem);
                }
            }
        }
    }
}
