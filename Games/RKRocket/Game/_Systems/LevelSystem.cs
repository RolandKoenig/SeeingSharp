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
        private TimeSpan m_timeSinceBlocksActive;
        private LevelData m_lastLevel;
        private LevelData m_currentLevel;
        private bool m_isLoadingLevel;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelSystem"/> class.
        /// </summary>
        public LevelSystem()
        {
            m_actLevelNumber = 0;
            m_aliveBlocks = new List<BlockEntity>();

            m_currentLevel = new LevelData();
            m_currentLevel.SetDefaultContent();
        }

        /// <summary>
        /// Updates this object.
        /// </summary>
        /// <param name="updateState">Current state of the update pass.</param>
        protected override async void UpdateInternal(SceneRelatedUpdateState updateState)
        {
            base.UpdateInternal(updateState);

            // Record empty screen time
            if(m_aliveBlocks.Count == 0)
            {
                m_timeSinceNoBlock += updateState.UpdateTime;
                m_timeSinceBlocksActive = TimeSpan.Zero;
            }
            else
            {
                m_timeSinceNoBlock = TimeSpan.Zero;
                m_timeSinceBlocksActive += updateState.UpdateTime;
            }

            // Trigger fall down animation
            if(m_timeSinceBlocksActive > TimeSpan.FromSeconds(m_currentLevel.TimeSinceFallDownSeconds))
            {
                m_timeSinceBlocksActive = TimeSpan.Zero;
                base.Messenger.Publish<MessageBlocksFallOneCellDown>();
            }

            // Decide whether wie jump to the next level
            if (m_isLoadingLevel) { return; }
            if((m_actLevelNumber == 0) ||
               (m_timeSinceNoBlock > Constants.LEVEL_MIN_TIME_WITHOUT_BLOCKS))
            {
                // Reset current timespan since level start
                m_timeSinceNoBlock = TimeSpan.Zero;

                // Increment level number
                m_actLevelNumber += 1;

                // Exchange level objects
                m_lastLevel = m_currentLevel;

                // Load next level
                string formatedLevelNumber = m_actLevelNumber.ToString().PadLeft(3, '0');
                m_isLoadingLevel = true;
                try
                {
                    m_currentLevel = await CommonTools.DeserializeJsonFromResourceAsync<LevelData>(
                        new AssemblyResourceUriBuilder(
                            "RKRocket", true,
                            $"Assets/Levels/Level_{formatedLevelNumber}.json"));
                }
                catch (Exception)
                {
                    m_currentLevel = m_lastLevel;
                }
                finally
                {
                    m_isLoadingLevel = false;
                }

                // Load first level (create all blocks)
                float targetYOffset = Constants.BLOCK_CELL_HEIGHT * m_currentLevel.YCellOffset;
                float creationYOffset = 
                    Constants.BLOCK_CELL_HEIGHT * m_currentLevel.CountOfRows +
                    targetYOffset;
                for (int loopRow = 0; loopRow < m_currentLevel.CountOfRows; loopRow++)
                {
                    string[] actRowData = m_currentLevel.GetRow(loopRow);
                    for (int loopXBlock = 0; loopXBlock < Constants.BLOCKS_COUNT_X && loopXBlock < actRowData.Length; loopXBlock++)
                    {
                        // Get block type out of level data
                        int actBlockType = 0;
                        if (string.IsNullOrWhiteSpace(actRowData[loopXBlock])) { continue; }
                        if (!Int32.TryParse(actRowData[loopXBlock], out actBlockType)) { continue; }
                        if (actBlockType < 1) { actBlockType = 1; }
                        if (actBlockType > GraphicsResources.Bitmap_Blocks.Length) { actBlockType = 1; }

                        // Calculate the position of the block
                        Vector2 actBlockPosition = new Vector2(
                            Constants.BLOCK_CELL_WIDTH * loopXBlock + (Constants.BLOCK_CELL_WIDTH / 2f),
                            Constants.BLOCK_CELL_HEIGHT * loopRow + (Constants.BLOCK_CELL_HEIGHT / 2f));

                        // Create the block
                        BlockEntity newBlock = new BlockEntity(actBlockType);
                        newBlock.Position = new Vector2(actBlockPosition.X, actBlockPosition.Y - creationYOffset);
                        newBlock.BuildAnimationSequence()
                            .Move2DTo(actBlockPosition - new Vector2(0f, targetYOffset), TimeSpan.FromSeconds(1.0))
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

        /// <summary>
        /// Handles the NewGame event.
        /// </summary>
        private void OnMessage_Received(MessageNewGame message)
        {
            m_actLevelNumber = 0;
            m_aliveBlocks.Clear();

            m_currentLevel = new LevelData();
            m_currentLevel.SetDefaultContent();
        }
    }
}
