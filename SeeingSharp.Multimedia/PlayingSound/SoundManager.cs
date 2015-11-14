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
    /// <summary>
    /// This class manages all currently playing sounds.
    /// </summary>
    public class SoundManager
    {
        #region Audio resources
        private FactoryHandlerXAudio2 m_xaudioDevice;
        private List<XA.SourceVoice> m_playingVoices;
        #endregion

        internal SoundManager(FactoryHandlerXAudio2 xaudioDevice)
        {
            m_playingVoices = new List<XA.SourceVoice>();
            m_xaudioDevice = xaudioDevice;
        }

        /// <summary>
        /// Plays the given sound.
        /// </summary>
        /// <param name="resource">The file to be played.</param>
        public async Task PlaySoundAsync(ResourceLink resource)
        {
            resource.EnsureNotNull("resource");

            using (CachedSoundFile cachedSoundFile = await CachedSoundFile.FromResourceAsync(resource))
            {
                await PlaySoundAsync(cachedSoundFile);
            }
        }

        /// <summary>
        /// Plays the given sound.
        /// </summary>
        /// <param name="soundFile">The sound file to be played.</param>
        public async Task PlaySoundAsync(CachedSoundFile soundFile)
        {
            soundFile.EnsureNotNullOrDisposed("soundFile");

            // Play the sound on the device
            using (var sourceVoice = new XA.SourceVoice(m_xaudioDevice.Device, soundFile.Format, true))
            {
                // Register the created voice
                m_playingVoices.Add(sourceVoice);

                // Start voice playing
                TaskCompletionSource<object> complSource = new TaskCompletionSource<object>();
                sourceVoice.SubmitSourceBuffer(soundFile.AudioBuffer, soundFile.DecodedPacketsInfo);
                sourceVoice.BufferEnd += (pointer) =>
                {
                    complSource.TrySetResult(null);
                };
                sourceVoice.Start();

                // Await finished playing
                await complSource.Task;

                // Destroy the voice object
                //  A NullReference is raised later, if we forget this call
                sourceVoice.DestroyVoice();

                // Remove the created voice finally
                m_playingVoices.Remove(sourceVoice);
            }
        }

        /// <summary>
        /// Gets the total count of currently playing sounds.
        /// </summary>
        public int CountPlaying
        {
            get { return m_playingVoices.Count; }
        }
    }
}
