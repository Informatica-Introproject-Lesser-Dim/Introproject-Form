using System.Collections.Generic;

using NUnit.Framework;

using IntroProject;

namespace IntroProjectTest
{
    class ScreenTest
    {
        [TestFixture]
        public class MultiLanguageTest
        {
            [TestFixture]
            public class ValidateTranslationsFile
            {
                [TestFixture]
                public class TestHeaderOnly
                {
                    MultiLanguage multiLanguage = new MultiLanguage(@".\fixtures\translations\header-only.csv");

                    [Test]
                    public void TestLanguagesInTranslationDictionaryKeys()
                    {
                        var languages = new List<string> { "Dutch", "English" };

                        Assert.AreEqual(multiLanguage.translations.Count, languages.Count);

                        foreach (string languageKey in multiLanguage.translations.Keys)
                            Assert.Contains(languageKey, languages);
                    }

                    [Test]
                    public void TestTranslationDictionaryValuesAreEmptyDictionaries()
                    {
                        foreach (Dictionary<string, string> dictForSelectedLanguage in multiLanguage.translations.Values)
                        {
                            Assert.IsEmpty(dictForSelectedLanguage.Keys);
                            Assert.IsEmpty(dictForSelectedLanguage.Values);
                        }
                    }
                }
            }
        }
    }
}
