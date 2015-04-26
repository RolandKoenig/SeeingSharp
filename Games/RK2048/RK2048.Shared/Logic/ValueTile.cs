#region License information (SeeingSharp and all based games/applications)
/*
    SeeingSharp and all games/applications based on it (more info at http://www.rolandk.de/wp)
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

using SeeingSharp.Multimedia.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using SeeingSharp.Multimedia.Core;
using SeeingSharp;

namespace RK2048.Logic
{
    internal class ValueTile : GenericObject
    {
        private int m_currentID;
        private int m_coordX;
        private int m_coordY;

        public ValueTile(int coordX, int coordY)
            : this(coordX, coordY, 0)
        {

        }

        public ValueTile(int coordX, int coordY, int id)
            : base(Constants.RES_GEO_TILES_BY_ID[id])
        {
            m_coordX = coordX;
            m_coordY = coordY;
            m_currentID = id;

            this.UpdateWorldPosition();
        }

        /// <summary>
        /// Calculates the world position for the given x and y tile positions.
        /// </summary>
        /// <param name="tilePosX">X coordinate of the tile.</param>
        /// <param name="tilePosY">Y coordinate of the tile.</param>
        public static Vector3 CalculateWorldPosition(int tilePosX, int tilePosY)
        {
            float tileWidth = Constants.TILE_WIDTH;
            float tileWidthHalf = tileWidth / 2f;
            float tileWidthDouble = tileWidth * 2f;

            Vector3 topLeft = new Vector3(-tileWidthDouble, 0f, -tileWidthDouble);
            Vector3 worldPosition =
                topLeft +
                new Vector3(tilePosX * tileWidth + tileWidthHalf, 0f, tilePosY * tileWidth + tileWidthHalf);
            return worldPosition;
        }

        /// <summary>
        /// Updates current world position.
        /// </summary>
        public void UpdateWorldPosition()
        {
            Vector3 worldPosition = CalculateWorldPosition(m_coordX, m_coordY);
            this.Position = worldPosition;
        }

        /// <summary>
        /// Gets or sets the current id of the tile (2, 4, 8, 16, ...).
        /// </summary>
        public int CurrentID
        {
            get { return m_currentID; }
            set
            {
                if(m_currentID != value)
                {
                    m_currentID = value;
                    base.ChangeGeometry(Constants.RES_GEO_TILES_BY_ID[m_currentID]);
                }
            }
        }

        /// <summary>
        /// Gets the xcoord of the tile (0...3).
        /// </summary>
        public int CoordX
        {
            get { return m_coordX; }
            set { m_coordX = value; }
        }

        /// <summary>
        /// Gets the ycoord of the tile (0...3).
        /// </summary>
        public int CoordY
        {
            get { return m_coordY; }
            set { m_coordY = value; }
        }
    }
}
