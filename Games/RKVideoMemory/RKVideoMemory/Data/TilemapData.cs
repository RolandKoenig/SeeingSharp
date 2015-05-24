using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp;
using SeeingSharp.Checking;
using SeeingSharp.Util;

namespace RKVideoMemory.Data
{
    public class TilemapData
    {
        private bool[,] m_tilesAllowed;

        public TilemapData(
            int tilesX = Constants.TILEMAP_DEFAULT_X_COUNT,
            int tilesY = Constants.TILEMAP_DEFAULT_Y_COUNT)
        {
            tilesX.EnsurePositiveAndNotZero("tilesX");
            tilesY.EnsurePositiveAndNotZero("tilesY");

            m_tilesAllowed = new bool[tilesX, tilesY];

            // Enable all tile locations by default
            for (int loopX = 0; loopX < tilesX; loopX++)
            {
                for (int loopY = 0; loopY < tilesY; loopY++)
                {
                    m_tilesAllowed[loopX, loopY] = true;
                }
            }
        }

        public static TilemapData FromFile(ResourceLink link)
        {
            int maxLength = 0;
            List<string> lines = new List<string>(16);

            // Read raw data from the tilemap file
            using (Stream inStream = link.OpenInputStream())
            using (StreamReader inStreamReader = new StreamReader(inStream))
            {
                string actLine = inStreamReader.ReadLine();
                while (actLine != null)
                {
                    lines.Add(actLine);
                    if (actLine.Length > maxLength) { maxLength = actLine.Length; }

                    actLine = inStreamReader.ReadLine();
                }
            }

            // Enable/Disable specific tiles
            TilemapData result = new TilemapData(maxLength, lines.Count);
            for (int loopY = 0; loopY < lines.Count; loopY++)
            {
                string actLine = lines[loopY];
                for (int loopX = 0; loopX < maxLength; loopX++)
                {
                    if (actLine.Length <= loopX) { result[loopX, loopY] = false; }
                    else if (actLine[loopX] == Constants.TILE_CHAR_UNALLOWED) { result[loopX, loopY] = false; }
                    else if (actLine[loopX] == Constants.TILE_CHAR_ALLOWED)
                    {
                        result[loopX, loopY] = true;
                    }
                    else
                    {
                        throw new SeeingSharpException("Unknown character in tilemap file: " + actLine[loopX]);
                    }
                }
            }

            return result;
        }

        public bool this[int xPos, int yPos]
        {
            get
            {
                xPos.EnsureInRange(0, m_tilesAllowed.GetLength(0) - 1, "xPos");
                yPos.EnsureInRange(0, m_tilesAllowed.GetLength(1) - 1, "xPos");

                return m_tilesAllowed[xPos, yPos];
            }
            set
            {
                xPos.EnsureInRange(0, m_tilesAllowed.GetLength(0) - 1, "xPos");
                yPos.EnsureInRange(0, m_tilesAllowed.GetLength(1) - 1, "xPos");

                m_tilesAllowed[xPos, yPos] = value;
            }
        }

        public int TilesX
        {
            get { return m_tilesAllowed.GetLength(0); }
        }

        public int TilesY
        {
            get { return m_tilesAllowed.GetLength(1); }
        }
    }
}