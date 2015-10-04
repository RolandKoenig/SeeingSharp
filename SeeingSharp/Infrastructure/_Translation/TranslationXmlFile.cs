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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SeeingSharp.Infrastructure
{
    [XmlType("translations")]
    public class TranslationXmlFile
    {
        private List<TranslatedWord> m_words;
        private Dictionary<string, string> m_wordsDict;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationXmlFile"/> class.
        /// </summary>
        public TranslationXmlFile()
        {
            m_words = new List<TranslatedWord>(1024);
            m_wordsDict = new Dictionary<string, string>();
        }

        /// <summary>
        /// Builds a dictionary for faster access.
        /// </summary>
        internal void BuildDictionary()
        {
            foreach(TranslatedWord actWord in m_words)
            {
                m_wordsDict[actWord.Original] = actWord.Translated;
            }
        }

        /// <summary>
        /// Gets the translation for the given sentence.
        /// </summary>
        /// <param name="original">The original sentence.</param>
        /// <param name="actTranslated">The translation of the given string.</param>
        public bool TryGetTranslation(string original, ref string actTranslated)
        {
            return m_wordsDict.TryGetValue(original, out actTranslated);
        }

        /// <summary>
        /// Gets the language this file is loaded for.
        /// </summary>
        [XmlIgnore]
        public SeeingSharpLanguageKey LanguageKey
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a collection containing all translated words.
        /// </summary>
        [XmlElement("word")]
        public List<TranslatedWord> Words
        {
            get { return m_words; }
        }
    }
}
