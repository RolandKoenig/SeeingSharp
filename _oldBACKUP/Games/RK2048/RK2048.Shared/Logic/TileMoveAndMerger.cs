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
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Multimedia.Core;
using RK2048.Graphics;

namespace RK2048.Logic
{
    internal struct TileMoveAndMerger
    {
        private ValueTile[,] m_gameTiles;
        private ValueTile m_lastValue;
        private Tuple<int, int> m_lastValueTargetPos;
        private Tuple<int, int> m_lastEmptyPos;
        private Tuple<int, int> m_singleMove;

        public TileMoveAndMerger(ValueTile[,] tileMap, Tuple<int, int> singleMove)
        {
            m_gameTiles = tileMap;
            m_lastValue = null;
            m_lastEmptyPos = null;
            m_lastValueTargetPos = null;
            m_singleMove = singleMove;
        }

        /// <summary>
        /// Generate move or merge animations for the next step on tilemap update.
        /// </summary>
        /// <param name="actLocation">The location where we are currently.</param>
        public Task<int> CalculateForNextRow(int actXCoord, int actYCoord)
        {
            Task result = null;
            int gainedScore = 0;

            ValueTile actValue = m_gameTiles[actXCoord, actYCoord];
            if (actValue == null)
            {
                if (m_lastEmptyPos == null) { m_lastEmptyPos = Tuple.Create(actXCoord, actYCoord); }
            }
            else
            {
                if ((m_lastValue != null) && 
                    (m_lastValue.CurrentID == actValue.CurrentID) &&
                    (m_lastValue.CurrentID < Constants.TILE_VALUE_BY_ID.Length - 1))
                {
                    int targetXCoord = m_lastValueTargetPos != null ? m_lastValueTargetPos.Item1 : m_lastValue.CoordX;
                    int targetYCoord = m_lastValueTargetPos != null ? m_lastValueTargetPos.Item2 : m_lastValue.CoordY;

                    // Detected merge operation
                    result = actValue.BuildAnimationSequence()
                        .MoveTileAndMergeWithOther(
                            m_gameTiles, m_lastValue, 
                            targetXCoord, targetYCoord)
                        .ApplyAsync();
                    m_lastValue = null;
                    m_lastValueTargetPos = null;

                    // Calculate gained score with this movement.
                    gainedScore = Constants.TILE_VALUE_BY_ID[actValue.CurrentID] * 2;

                    m_lastEmptyPos = Tuple.Create(
                        targetXCoord + m_singleMove.Item1,
                        targetYCoord + m_singleMove.Item2);
                }
                else if (m_lastEmptyPos != null)
                {
                    // Detected move operation
                    result = actValue.BuildAnimationSequence()
                        .MoveTileToEmptyLocation(m_gameTiles, m_lastEmptyPos.Item1, m_lastEmptyPos.Item2)
                        .ApplyAsync();
                    m_lastValue = actValue;
                    m_lastValueTargetPos = m_lastEmptyPos;
                    
                    m_lastEmptyPos = Tuple.Create(
                        m_lastEmptyPos.Item1 + m_singleMove.Item1, 
                        m_lastEmptyPos.Item2 + m_singleMove.Item2);
                }
                else
                {
                    // Detected nothin special, all full..
                    m_lastValue = actValue;
                    m_lastValueTargetPos = null;
                    m_lastEmptyPos = null;
                }
            }

            if (result == null) { return null; }
            else { return result.ContinueWith<int>((lastTask) => gainedScore); }
        }
    }
}
