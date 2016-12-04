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
using SeeingSharp.Infrastructure;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace SeeingSharp.View
{
    public class SeeingSharpPage : Page
    {
        #region Message subscriptions
        private IEnumerable<MessageSubscription> m_msgSubscriptions;
        #endregion

        public SeeingSharpPage()
        {
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!SeeingSharpApplication.IsInitialized) { return; }

            // Deregister all messages
            if (m_msgSubscriptions != null)
            {
                CommonTools.DisposeObjects(m_msgSubscriptions);
                m_msgSubscriptions = null;
            }
        }

        private void OnLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!SeeingSharpApplication.IsInitialized) { return; }

            // Register on all messages for which are methods defined here
            if (SeeingSharpApplication.IsUIEnvironmentInitialized &&
               (m_msgSubscriptions == null))
            {
                m_msgSubscriptions = SeeingSharpApplication.Current.UIMessenger.SubscribeAll(this);
            }
        }

        public SeeingSharpMessenger Messenger
        {
            get
            {
                if (!SeeingSharpApplication.IsInitialized) { return null; }
                return SeeingSharpApplication.Current.UIMessenger;
            }
        }
    }
}
