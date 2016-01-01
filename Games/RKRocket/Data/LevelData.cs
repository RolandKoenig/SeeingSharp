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
using Newtonsoft.Json;

namespace RKRocket.Data
{
    public class LevelData
    {
        [JsonProperty("Rows")]
        private List<string[]> m_levelRows;

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelData" /> class.
        /// </summary>
        public LevelData()
        {
            m_levelRows = new List<string[]>();
        }

        public void SetDefaultContent()
        {
            this.TimeSinceFallDownSeconds = 0;
            m_levelRows.Add(new string[] { "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1" });
            m_levelRows.Add(new string[] { "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1" });
            m_levelRows.Add(new string[] { "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1" });
            m_levelRows.Add(new string[] { "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1" });
            m_levelRows.Add(new string[] { "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1" });
        }

        public string[] GetRow(int loopRow)
        {
            return m_levelRows[loopRow];
        }

        [JsonProperty]
        public int TimeSinceFallDownSeconds
        {
            get;
            set;
        }

        [JsonProperty]
        public int YCellOffset
        {
            get;
            set;
        }

        public int CountOfRows
        {
            get { return m_levelRows.Count; }
        }
    }
}
