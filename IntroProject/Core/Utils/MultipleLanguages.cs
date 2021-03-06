﻿using System;
using System.Collections.Generic;
using System.IO;

namespace IntroProject.Core.Utils
{
    using Language = String;
    public class MultipleLanguages
    {
        private static readonly Lazy<MultipleLanguages> lazy = new Lazy<MultipleLanguages>
                                                        (() => new MultipleLanguages());

        public static MultipleLanguages Instance { get { return lazy.Value; } }

        bool debugdisplayedText = true;

        public Dictionary<Language, Dictionary<string, string>> translations = new Dictionary<Language, Dictionary<string, string>>();

        private StreamReader reader;
        private string[] headerSplit;

        public MultipleLanguages() : this(@".\dataFiles\translations.csv") { }

        public MultipleLanguages(string translationFile)
        {
            reader = new StreamReader(translationFile);
            ReadLanguagesFromTranslationsFileHeader();
            ReadTranslationsFromTranslationsFile();
        }

        private void ReadLanguagesFromTranslationsFileHeader()
        {
            if (reader.EndOfStream)
                throw new FileLoadException("Missing header; End of Filestream reached");
            headerSplit = reader.ReadLine().Split(',');

            for (int i = 1; i < headerSplit.Length; i++)
                translations.Add(headerSplit[i], new Dictionary<string, string>());
        }

        private void ReadTranslationsFromTranslationsFile()
        {
            for (int i = 0; !reader.EndOfStream; i++)
            {
                string[] entries = reader.ReadLine().Split(',');
                string key = entries[0];
                for (int j = 1; j < entries.Length; j++)
                {
                    string translated = entries[j];
                    // Add (key => translated) per language
                    translations[headerSplit[j]].Add(key, translated);
                }
            }
            reader.Close();
        }

        /// <summary>
        /// Attempts to translate lookupText to selected language either by passing a languageNumber
        /// or reading it from Settings.LanguageIndex
        /// </summary>
        /// <param name="lookupText"></param>
        /// <returns>The translated lookupText</returns>
        public string DisplayText(string lookupText) => DisplayText(lookupText, Settings.LanguageIndex);
        public string DisplayText(string lookupText, int languageNumber)
        {
            try { return translations[headerSplit[++languageNumber]][lookupText]; }
            catch (KeyNotFoundException)
            {
                if (debugdisplayedText == true)
                    return $"Translation for {lookupText} is unknown";
                return lookupText;
            }
        }
    }
}
