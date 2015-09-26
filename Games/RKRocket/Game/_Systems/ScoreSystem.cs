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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKRocket.Game
{
    public class ScoreSystem : SceneLogicalObject
    {
        #region Local data
        private int m_currentScore;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoreSystem"/> class.
        /// </summary>
        public ScoreSystem()
        {
            m_currentScore = 0;
        }

        private void OnMessage_Received(MessageLevelStarted message)
        {
            if(message.LevelNumber > 1) { return; }

            if(m_currentScore != 0)
            {
                m_currentScore = 0;
                base.Messenger.Publish(new MessageScoreChanged(m_currentScore));
            }
        }

        private void OnMessage_Received(MessageCollisionProjectileToBlockDetected message)
        {
            m_currentScore++;
            base.Messenger.Publish(new MessageScoreChanged(m_currentScore));
        }

        /// <summary>
        /// Handles the NewGame event.
        /// </summary>
        private void OnMessage_Received(MessageNewGame message)
        {
            m_currentScore = 0;
            base.Messenger.Publish(new MessageScoreChanged(m_currentScore));
        }
    }
}
