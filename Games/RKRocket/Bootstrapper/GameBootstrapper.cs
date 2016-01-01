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
using SeeingSharp.Infrastructure;
using SeeingSharp.Gaming;
using RKRocket.Game;

// Define assembly attributes for type that is defined in this file
[assembly: AssemblyQueryableType(
    targetType: typeof(RKRocket.Bootstrapper.GameBootstrapper),
    contractType: typeof(SeeingSharp.Infrastructure.IBootstrapperItem))]

namespace RKRocket.Bootstrapper
{
    public class GameBootstrapper : IBootstrapperItem
    {
        public async Task Execute()
        {
            // Load the game data container
            GameDataContainer gameData = await GameDataContainer.LoadFromRoamingFolderAsync(Constants.GAME_SHORT_NAME);
            SeeingSharpApplication.Current.RegisterSingleton(gameData);

            // Create and initialize the GameCore object
            GameCore gameCore = new GameCore();
            SeeingSharpApplication.Current.RegisterSingleton(gameCore);
        }

        public string Description
        {
            get
            {
                return Translatables.BOOTRSAPPER_GAME_DATA;
            }
        }

        public int OrderID
        {
            get
            {
                return 0;
            }
        }
    }
}
