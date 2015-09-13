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
using SeeingSharp.Util;
using SeeingSharp.Multimedia.Core;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKRocket.Game
{
    public class FragmentCreationSystem : SceneLogicalObject
    {
        #region Logic
        private Random m_randomizer;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FragmentCreationSystem"/> class.
        /// </summary>
        public FragmentCreationSystem()
        {
            m_randomizer = new Random(Environment.TickCount);
        }

        /// <summary>
        /// The CollisionSystem notifies us that a projectile collided with a block.
        /// </summary>
        private void OnMessage_Received(MessageCollisionProjectileToBlockDetected message)
        {
            Vector2 fragmentSourcePoint =
                message.Projectile.Position + new Vector2(0f, -Constants.GFX_PROJECTILE_VPIXEL_HEIGHT / 2f);

            // Create all fragments
            int fragmentCount = m_randomizer.Next(Constants.FRAGMENT_MIN_COUNT, Constants.FRAGMENT_MAX_COUNT + 1);
            FragmentEntity[] createdEntities = new FragmentEntity[fragmentCount];
            for(int loop=0; loop<fragmentCount; loop++)
            {
                Vector2 directionNormal = Vector2.Normalize(new Vector2(
                    m_randomizer.Next(-100, 100) / 100f,
                    m_randomizer.Next(0, 100) / 100f));
                float fragmentSpeed = (float)m_randomizer.Next(Constants.FRAGMENT_MIN_SPEED, Constants.FRAGMENT_MAX_SPEED);

                createdEntities[loop] = new FragmentEntity(
                    fragmentSourcePoint + directionNormal * (Constants.FRAGMENT_VPIXEL_WIDTH / 2f), 
                    directionNormal,
                    fragmentSpeed);
            }

            // Add the created fragments to the scene
            base.Scene.ManipulateSceneAsync(
                (manipulator) =>
                {
                    manipulator.AddRange(createdEntities);
                })
                .FireAndForget();
        }
    }
}
