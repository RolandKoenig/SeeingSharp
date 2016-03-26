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
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Util;
using SeeingSharp.Checking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Namespace mappings
using SDX = SharpDX;
using SDXM = SharpDX.Multimedia;
using XA = SharpDX.XAudio2;

namespace SeeingSharp.Multimedia.PlayingSound
{
    public class CachedSoundFile : IDisposable, ICheckDisposed
    {
        #region XAudio resources
        private SDXM.WaveFormat m_format;
        private XA.AudioBuffer m_audioBuffer;
        private uint[] m_decodedPacketsInfo;
        #endregion

        /// <summary>
        /// Prevents a default instance of the <see cref="CachedSoundFile"/> class from being created.
        /// </summary>
        private CachedSoundFile()
        {

        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_audioBuffer != null)
            {
                GraphicsHelper.DisposeObject(m_audioBuffer.Stream);
                m_format = null;
                m_audioBuffer = null;
            }
        }

        /// <summary>
        /// Loads a sound file from the given resource.
        /// </summary>
        /// <param name="resource">The resource to load.</param>
        public static async Task<CachedSoundFile> FromResourceAsync(ResourceLink resource)
        {
            resource.EnsureNotNull(nameof(resource));

            CachedSoundFile result = new CachedSoundFile();

            using (Stream inStream = await resource.OpenInputStreamAsync())
            using (SDXM.SoundStream stream = new SDXM.SoundStream(inStream))
            {
                await Task.Factory.StartNew(() =>
                {
                    // Read all data into the adio buffer
                    SDXM.WaveFormat waveFormat = stream.Format;
                    XA.AudioBuffer buffer = new XA.AudioBuffer
                    {
                        Stream = stream.ToDataStream(),
                        AudioBytes = (int)stream.Length,
                        Flags = XA.BufferFlags.EndOfStream
                    };

                    // Store members
                    result.m_decodedPacketsInfo = stream.DecodedPacketsInfo;
                    result.m_format = waveFormat;
                    result.m_audioBuffer = buffer;
                });
            }

            return result;
        }

        internal SDXM.WaveFormat Format
        {
            get { return m_format; }
        }

        internal XA.AudioBuffer AudioBuffer
        {
            get { return m_audioBuffer; }
        }

        internal uint[] DecodedPacketsInfo
        {
            get { return m_decodedPacketsInfo; }
        }

        public bool IsDisposed
        {
            get { return m_audioBuffer == null; }
        }
    }
}
