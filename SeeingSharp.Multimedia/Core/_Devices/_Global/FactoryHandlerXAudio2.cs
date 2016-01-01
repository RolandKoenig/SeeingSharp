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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Namespace mappings
using XA = SharpDX.XAudio2;

namespace SeeingSharp.Multimedia.Core
{
    public class FactoryHandlerXAudio2
    {
        #region Main XAudio2 resources
        private XA.XAudio2 m_xaudioDevice;
        private XA.MasteringVoice m_masteringVoice;
        #endregion

        internal FactoryHandlerXAudio2()
        {
            m_xaudioDevice = new SharpDX.XAudio2.XAudio2(SharpDX.XAudio2.XAudio2Version.Default);
            m_masteringVoice = new SharpDX.XAudio2.MasteringVoice(m_xaudioDevice);
        }

        internal XA.XAudio2 Device
        {
            get { return m_xaudioDevice; }
        }

        internal XA.MasteringVoice MasteringVoice
        {
            get { return m_masteringVoice; }
        }
    }
}
