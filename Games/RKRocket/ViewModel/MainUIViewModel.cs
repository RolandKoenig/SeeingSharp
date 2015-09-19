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
using RKRocket.Game;
using SeeingSharp.Infrastructure;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKRocket.ViewModel
{
    public class MainUIViewModel : ViewModelBase
    {
        #region core game object
        private GameCore m_game;
        #endregion

        #region captured state variables
        private int m_currentLevel;
        private int m_currentScore;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainUIViewModel"/> class.
        /// </summary>
        public MainUIViewModel()
        {
            m_currentLevel = 1;

            // Create core game object
            if (SeeingSharpApplication.IsInitialized)
            {
                m_game = new GameCore();

                // Attach to all messages 
                SeeingSharpApplication.Current.UIMessenger.SubscribeAll(this);
            }
        }

        /// <summary>
        /// Called when we reached a new level.
        /// </summary>
        private void OnMessage_Received(MessageLevelStarted message)
        {
            this.CurrentLevel = message.LevelNumber;
        }

        private void OnMessage_Received(MessageScoreChanged message)
        {
            this.CurrentScore = message.NewScore;
        }

        public GameCore Game
        {
            get { return m_game; }
        }

        /// <summary>
        /// The number of the current level.
        /// </summary>
        public int CurrentLevel
        {
            get { return m_currentLevel; }
            set
            {
                if(m_currentLevel != value)
                {
                    m_currentLevel = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public int CurrentScore
        {
            get { return m_currentScore; }
            set
            {
                if(m_currentScore != value)
                {
                    m_currentScore = value;
                    base.RaisePropertyChanged();
                }
            }
        }
    }
}
