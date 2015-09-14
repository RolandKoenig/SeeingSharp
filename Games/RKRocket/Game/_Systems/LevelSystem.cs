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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using RKRocket.Data;

namespace RKRocket.Game
{
    public class LevelSystem : SceneLogicalObject
    {
        #region level parameters
        private int m_actLevelNumber;
        private List<BlockEntity> m_aliveBlocks;
        private TimeSpan m_timeSinceNoBlock;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelSystem"/> class.
        /// </summary>
        public LevelSystem()
        {
            m_actLevelNumber = 3;
            m_aliveBlocks = new List<BlockEntity>();
        }

        protected override void UpdateInternal(SceneRelatedUpdateState updateState)
        {
            base.UpdateInternal(updateState);

            // Record empty screen time
            if(m_aliveBlocks.Count == 0) { m_timeSinceNoBlock += updateState.UpdateTime; }
            else { m_timeSinceNoBlock = TimeSpan.Zero; }

            // Decide whether wie jump to the next level
            if((m_actLevelNumber == 0) ||
               (m_timeSinceNoBlock > Constants.LEVEL_MIN_TIME_WITHOUT_BLOCKS))
            {
                // Increment level number
                m_actLevelNumber += 1;

                // Get corresponding level properties
                int levelPropertiesIndex = m_actLevelNumber - 1;
                if (Constants.LEVEL_PROPERTIES.Length <= levelPropertiesIndex)
                {
                    levelPropertiesIndex = Constants.LEVEL_PROPERTIES.Length - 1;
                }
                LevelProperties currentLevel = Constants.LEVEL_PROPERTIES[levelPropertiesIndex];

                // Load first level (create all blocks)
                Random randomizer = new Random(Environment.TickCount);
                float cellWidth = Constants.GFX_SCREEN_VPIXEL_WIDTH / Constants.BLOCKS_COUNT_X;
                float cellHeight = Constants.BLOCK_VPIXEL_HEIGHT + 10f;
                float creationYOffset = cellHeight * currentLevel.CountOfRows;
                for (int loopRow = 0; loopRow < currentLevel.CountOfRows; loopRow++)
                {
                    for (int loopXBlock = 0; loopXBlock < Constants.BLOCKS_COUNT_X; loopXBlock++)
                    {
                        Vector2 actBlockPosition = new Vector2(
                            cellWidth * loopXBlock + (cellWidth / 2f),
                            cellHeight * loopRow + (cellHeight / 2f));

                        BlockEntity newBlock = new BlockEntity(randomizer.Next(1, 4));
                        newBlock.Position = new Vector2(actBlockPosition.X, actBlockPosition.Y - creationYOffset);
                        newBlock.BuildAnimationSequence()
                            .Move2DTo(actBlockPosition, TimeSpan.FromSeconds(1.0))
                            .Apply();
                        m_aliveBlocks.Add(newBlock);
                    }
                }

                // Append all generated blocks to the scene
                base.Scene.ManipulateSceneAsync(
                    (manipulator) =>
                    {
                        manipulator.AddRange(m_aliveBlocks);
                    })
                    .FireAndForget();

                // Notify the started level
                base.Messenger.Publish(new MessageLevelStarted(m_actLevelNumber));
            }
        }

        /// <summary>
        /// This method gets called when a block was fired off
        /// </summary>
        private void OnMessage_Received(MessageBlockStartsLeaving message)
        {
            m_aliveBlocks.Remove(message.Block);
        }
    }
}
