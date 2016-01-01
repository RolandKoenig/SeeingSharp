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
using RKRocket.Game;
using SeeingSharp.Gaming;
using SeeingSharp.Infrastructure;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKRocket.ViewModel
{
    public class GameOverViewModel : ViewModelBase
    {
        public GameOverViewModel(GameOverReason reason, int reachedScore)
        {
            this.Reason = reason;
            this.ReachedScore = reachedScore;

            this.CommandPostScore = new DelegateCommand(OnCommandPostScore_Execute);
            this.CommandCancel = new DelegateCommand(OnCommandCancel_Execute);
        }

        private void OnCommandPostScore_Execute()
        {
            if (!this.Validate()) { return; }

            if (this.ReachedScore > 0)
            {
                GameDataContainer dataContainer = SeeingSharpApplication.Current.GetSingleton<GameDataContainer>();
                dataContainer.TryPostNewScore(this.Name, this.ReachedScore);
                dataContainer.SaveToRoamingFolder()
                    .FireAndForget();

                base.Messenger.Publish<MessageHighScorePosted>();
            }

            GameCore game = SeeingSharpApplication.Current.GetSingleton<GameCore>();
            game.Scene.Messenger.BeginPublish<MessageNewGame>();
        }

        private void OnCommandCancel_Execute()
        {
            GameCore game = SeeingSharpApplication.Current.GetSingleton<GameCore>();
            game.Scene.Messenger.BeginPublish<MessageNewGame>();
        }

        public DelegateCommand CommandPostScore
        {
            get;
        }

        public DelegateCommand CommandCancel
        {
            get;
        }

        [Required]
        public string Name
        {
            get;
            set;
        } = string.Empty;

        public GameOverReason Reason
        {
            get;
            private set;
        }

        public int ReachedScore
        {
            get;
            private set;
        }
    }
}
