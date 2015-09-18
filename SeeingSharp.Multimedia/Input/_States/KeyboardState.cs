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
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Multimedia.Input
{
    public class KeyboardState : InputStateBase
    {
        public static readonly KeyboardState Dummy = new KeyboardState(0, 0);

        #region current key states
        private List<WinVirtualKey> m_keysHit;
        private List<WinVirtualKey> m_keysDown;
        private bool m_focused;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardState"/> class.
        /// </summary>
        internal KeyboardState(int keyCapacityHit = 6, int keyCapacityDown = 12)
        {
            m_keysHit = new List<WinVirtualKey>(keyCapacityHit);
            m_keysDown = new List<WinVirtualKey>(keyCapacityDown);
        }

        internal void NotifyKeyDown(WinVirtualKey key)
        {
            bool anyRegistered =
                m_keysDown.Contains(key) ||
                m_keysHit.Contains(key);
            if (anyRegistered) { return; }

            m_keysHit.Add(key);
        }

        internal void NotifyKeyUp(WinVirtualKey key)
        {
            while (m_keysHit.Remove(key)) { }
            while (m_keysDown.Remove(key)) { }
        }

        internal void NotifyFocusLost()
        {
            m_focused = false;
        }

        internal void NotifyFocusGot()
        {
            m_focused = true;
        }

        public bool IsKeyHit(WinVirtualKey key)
        {
            if (!m_focused) { return false; }

            return m_keysHit.Contains(key);
        }

        public bool IsKeyDown(WinVirtualKey key)
        {
            if (!m_focused) { return false; }

            return
                m_keysDown.Contains(key) ||
                m_keysHit.Contains(key);
        }

        protected override InputStateBase CopyAndResetForUpdatePassInternal()
        {
            KeyboardState result = new KeyboardState(
                m_keysHit.Count, m_keysDown.Count);

            result.m_keysHit.AddRange(this.m_keysHit);
            result.m_keysDown.AddRange(this.m_keysDown);
            result.m_focused = this.m_focused;

            // Update local collections (move hit keys to down keys)
            this.m_keysDown.AddRange(this.m_keysHit);
            this.m_keysHit.Clear();

            return result;
        }

        public bool IsConnected
        {
            get { return m_focused; }
        }
    }
}
