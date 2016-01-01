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
using System.Xml.Serialization;

namespace SeeingSharp.Gaming
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