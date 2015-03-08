#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2014 Roland König (RolandK)

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
using System;
using System.Xml.Serialization;

namespace RK2048.Data
{
    [XmlType]
    public class GameScore
    {
        public GameScore()
        {

        }

        public GameScore(string playerName, int totalScore)
        {
            this.PlayerName = playerName;
            this.TotalScore = totalScore;
            this.Date = DateTime.Now;
        }

        [XmlIgnore]
        public int ScoreIndex
        {
            get;
            set;
        }

        [XmlAttribute]
        public string PlayerName
        {
            get;
            set;
        }

        [XmlAttribute]
        public int TotalScore
        {
            get;
            set;
        }

        [XmlAttribute]
        public DateTime Date
        {
            get;
            set;
        }

        [XmlIgnore]
        public string DateFormatted
        {
            get { return this.Date.ToString(); }
        }
    }
}