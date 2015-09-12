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
    public class CollisionSystem : SceneLogicalObject
    {
        #region all needed object references
        private PlayerRocketEntity m_player;
        private List<BlockEntity> m_blocks;
        private List<ProjectileEntity> m_projectiles;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionSystem"/> class.
        /// </summary>
        public CollisionSystem()
        {
            m_blocks = new List<BlockEntity>();
            m_projectiles = new List<ProjectileEntity>();
        }

        /// <summary>
        /// Updates this object.
        /// Performs all collision checks needed in RKRocket.
        /// </summary>
        /// <param name="updateState">State of the update pass.</param>
        protected override void UpdateInternal(SceneRelatedUpdateState updateState)
        {
            base.UpdateInternal(updateState);

            // Perfom collision checks for all projectiles
            foreach(ProjectileEntity actProjectile in m_projectiles)
            {

            }
        }

        /// <summary>
        /// Handle new objects on the scene.
        /// </summary>
        private void OnMessage_Received(SceneObjectAddedMessage message)
        {
            ProjectileEntity actProjectile = message.AdddedObject as ProjectileEntity;
            if(actProjectile != null)
            {
                m_projectiles.Add(actProjectile);
            }

            BlockEntity actBlock = message.AdddedObject as BlockEntity;
            if(actBlock != null)
            {
                m_blocks.Add(actBlock);
                return;
            }

            PlayerRocketEntity actPlayer = message.AdddedObject as PlayerRocketEntity;
            if(actPlayer != null)
            {
                m_player = actPlayer;
                return;
            }
        }

        /// <summary>
        /// Handle removed objects on the scene.
        /// </summary>
        private void OnMessage_Received(SceneObjectRemovedMessage message)
        {
            ProjectileEntity actProjectile = message.RemovedObject as ProjectileEntity;
            if (actProjectile != null)
            {
                m_projectiles.Remove(actProjectile);
            }

            BlockEntity actBlock = message.RemovedObject as BlockEntity;
            if (actBlock != null)
            {
                m_blocks.Remove(actBlock);
                return;
            }
        }
    }
}
