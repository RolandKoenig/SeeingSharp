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
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FrozenSky.Infrastructure
{
    public class FrozenSkyTranslator
    {
        private CultureInfo m_startupCulture;
        private FrozenSkyLanguageKey m_currentLanguage;
        private Dictionary<string, List<TranslationXmlFile>> m_loadedFilesByCategory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrozenSkyTranslator" /> class.
        /// </summary>
        internal FrozenSkyTranslator()
        {
            // Initialize members
            m_loadedFilesByCategory = new Dictionary<string, List<TranslationXmlFile>>();
            m_startupCulture = CultureInfo.CurrentCulture;

            // Select current language which is to be loaded
            m_currentLanguage = FrozenSkyLanguageKey.Default;
            Enum.TryParse<FrozenSkyLanguageKey>(m_startupCulture.TwoLetterISOLanguageName.ToUpper(), out m_currentLanguage);
            if (m_currentLanguage == FrozenSkyLanguageKey.Default) { m_currentLanguage = FrozenSkyLanguageKey.EN; }
        }

        /// <summary>
        /// Translates all translatable classes.
        /// </summary>
        internal void TranslateAllTranslatableClasses()
        {
            Parallel.ForEach(
                FrozenSkyApplication.Current.TypeQuery.QueryTypesByAttribute<TranslateableClassAttribute>(),
                (actRow) =>
                {
                    // Get a list containing all translatables
                    string actCategory = actRow.Item2.Category;
                    List<TranslationXmlFile> translatables = null;
                    if (!m_loadedFilesByCategory.TryGetValue(actCategory, out translatables)) { return; }

                    // Translate all static members
                    foreach(FieldInfo actMember in actRow.Item1.GetTypeInfo().DeclaredFields)
                    {
                        // Check for correct field type
                        if (!actMember.IsStatic) { continue; }
                        if (actMember.FieldType != typeof(string)) { continue; }

                        // Get the original sentence/word
                        string actOriginal = actMember.GetValue(null) as string;
                        if (string.IsNullOrEmpty(actOriginal)) { continue; };

                        // Translate by enumerating through translation files loaded for this category
                        string actTranslation = actOriginal;
                        foreach(TranslationXmlFile actXmlFile in translatables)
                        {
                            actXmlFile.GetTranslation(actOriginal, ref actTranslation);
                        }

                        // Change the sentence/word, if found in dictionaries
                        if((!string.IsNullOrEmpty(actTranslation)) &&
                           (actTranslation != actOriginal))
                        {
                            actMember.SetValue(null, actTranslation);
                        }
                    }
                });
        }

        /// <summary>
        /// Queries for all translations found in loaded assemblies.
        /// </summary>
        /// <param name="currentAssemblies">A collection containing all currently loaded assemblies.</param>
        internal Task QueryTranslationsAsync(IEnumerable<Assembly> currentAssemblies)
        {
            return Task.Factory.StartNew(() =>
            {
                foreach (Assembly actAssembly in currentAssemblies)
                {
                    string[] manifestResourcesNames = actAssembly.GetManifestResourceNames();
                    for(int loop=0 ; loop<manifestResourcesNames.Length; loop++)
                    {
                        string actManifestResourceName = manifestResourcesNames[loop];
                        string actManifestResourceNameLower = actManifestResourceName.ToLower();

                        // Handle file by file type
                        switch(Path.GetExtension(actManifestResourceName))
                        {
                                // Translation file
                            case ".langXml":
                                using (Stream inStream = actAssembly.GetManifestResourceStream(actManifestResourceName))
                                {
                                    LoadTranslationFile(actManifestResourceName, inStream);
                                }
                                break;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Loads a translation file from the given stream.
        /// </summary>
        /// <param name="fileName">The name of the file from which the given stream loads data.</param>
        /// <param name="inStream">The input stream.</param>
        private void LoadTranslationFile(string fileName, Stream inStream)
        {
            // Parse and set the language key (parsed from file name)
            //  .. return here if the language does not match the currently loaded
            //  .. load default language everytime
            FrozenSkyLanguageKey langKey = CommonTools.GetLanguageKeyFromFileName(fileName);
            if ((langKey != FrozenSkyLanguageKey.Default) &&
                (langKey != m_currentLanguage)) 
            { 
                return; 
            }

            // Deserialize the translation file
            XmlSerializer xmlSerializer = SerializerRepository.Current.GetSerializer<TranslationXmlFile>();
            TranslationXmlFile translationData = xmlSerializer.Deserialize(inStream) as TranslationXmlFile;
            translationData.LanguageKey = langKey;
            translationData.BuildDictionary();
            if (translationData == null) { return; }

            // Get the category of the language file
            string category = CommonTools.GetLanguageFileCategory(fileName);

            // Register loaded translation file in global dictionary
            List<TranslationXmlFile> fileList = null;
            if(!m_loadedFilesByCategory.TryGetValue(category, out fileList))
            {
                fileList = new List<TranslationXmlFile>();
                m_loadedFilesByCategory.Add(category, fileList);
            }

            // Place default language-key on first places
            if (langKey == FrozenSkyLanguageKey.Default) { fileList.Insert(0, translationData); }
            else { fileList.Add(translationData); }
        }

        /// <summary>
        /// Gets the current language this app runs on.
        /// </summary>
        public FrozenSkyLanguageKey Language
        {
            get { return m_currentLanguage; }
        }

        /// <summary>
        /// Gets the culture this app is started with.
        /// </summary>
        public CultureInfo Culture
        {
            get { return m_startupCulture; }
        }
    }
}
