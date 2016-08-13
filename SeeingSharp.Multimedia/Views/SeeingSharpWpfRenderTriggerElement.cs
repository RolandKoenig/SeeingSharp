#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
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
#if DESKTOP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SeeingSharp.Multimedia.Views
{
    /// <summary>
    /// A wpf element which triggers the rendering in wpf in a low
    /// frequency (see property <see cref="SeeingSharpWpfRenderTriggerElement.TriggerIntervalMS"/>) 
    /// </summary>
    public class SeeingSharpWpfRenderTriggerElement : FrameworkElement
    {
        private DispatcherTimer m_triggerTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeeingSharpWpfRenderTriggerElement"/> class.
        /// </summary>
        public SeeingSharpWpfRenderTriggerElement()
        {
            m_triggerTimer = new DispatcherTimer(DispatcherPriority.Render);
            m_triggerTimer.Tick += OnTriggerTimer_Tick;
            m_triggerTimer.Interval = TimeSpan.FromMilliseconds(5.0);

            this.Width = 10;
            this.Height = 10;
            this.VerticalAlignment = VerticalAlignment.Top;
            this.HorizontalAlignment = HorizontalAlignment.Left;

            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        private void OnTriggerTimer_Tick(object sender, EventArgs e)
        {
            this.InvalidateVisual();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            m_triggerTimer.Stop();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            m_triggerTimer.Start();
        }

        /// <summary>
        /// the interval in which to trigger the Wpf rendering logic (milliseconds).
        /// </summary>
        public int TriggerIntervalMS
        {
            get { return (int)m_triggerTimer.Interval.TotalMilliseconds; }
            set
            {
                if(value < 1) { return; }
                m_triggerTimer.Interval = TimeSpan.FromMilliseconds(value);
            }
        }
    }
}
#endif