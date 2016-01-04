﻿#region License information (SeeingSharp and all based games/applications)
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
using SeeingSharp.Multimedia.PlayingSound;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKRocket.Game
{
    public class AudioSystem : SceneLogicalObject
    {
        #region Resources for this system
        private CachedSoundFile m_soundExplosion;
        private CachedSoundFile m_soundLaserFire;
        #endregion

        public AudioSystem()
        {
            this.LoadAudioFiles();
        }

        /// <summary>
        /// Loads the audio files.
        /// </summary>
        private async void LoadAudioFiles()
        {
            m_soundExplosion = await CachedSoundFile.FromResourceAsync(
                new AssemblyResourceUriBuilder(
                    "RKRocket", true,
                    "Assets/Sounds/Explosion.wav"));
            m_soundLaserFire = await CachedSoundFile.FromResourceAsync(
                new AssemblyResourceUriBuilder(
                    "RKRocket", true,
                    "Assets/Sounds/LaserFire.wav"));
        }

        private void OnMessage_Received(MessageProjectileShooted message)
        {
            if(m_soundLaserFire != null)
            {
                GraphicsCore.Current.SoundManager.PlaySoundAsync(m_soundLaserFire)
                    .FireAndForget();
            }
        }

        private void OnMessage_Received(MessageCollisionProjectileToPlayerDetected message)
        {
            if(m_soundExplosion != null)
            {
                GraphicsCore.Current.SoundManager.PlaySoundAsync(m_soundExplosion)
                    .FireAndForget();
            }
        }
    }
}
