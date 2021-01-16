using System.Collections.Generic;

using NUnit.Framework;

using IntroProject.Core.Utils;

namespace IntroProjectTest.Core.Utils
{
    class MultipleLanguagesTest
    {
        [TestFixture]
        public class ValidateTranslationsFile
        {
            [TestFixture]
            public class TestHeaderOnly
            {
                MultipleLanguages multiLanguage = new MultipleLanguages(@".\fixtures\translations\header-only.csv");

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

            [TestFixture]
            public class TestHeaderWithTwoTranslations
            {
                MultipleLanguages multiLanguage = new MultipleLanguages(@".\fixtures\translations\header-with-two-translations.csv");
                private const int languageCount = 2;
                private const int translationEntriesCount = 2;

                Dictionary<string, string>[] dicts;
                string[] langs;

                [SetUp]
                public void SetUp()
                {
                    dicts = new Dictionary<string, string>[languageCount];
                    multiLanguage.translations.Values.CopyTo(dicts, 0);
                    langs = new string[languageCount];
                    multiLanguage.translations.Keys.CopyTo(langs, 0);
                }

                [Test]
                public void TestTranslationDictionaryValuesFilledWithTranslations()
                {
                    Assert.AreEqual(languageCount, multiLanguage.translations.Count);
                    foreach(var dict in dicts)
                        Assert.AreEqual(translationEntriesCount, dict.Count);

                    for(int i = 0; i < dicts.Length; i++)
                        switch (langs[i])
                        {
                            case "Dutch":
                                Assert.AreEqual(dicts[i]["banana"], "Banaan");
                                Assert.AreEqual(dicts[i]["hp"], "Aantal levenspunten");
                                break;
                            case "English":
                                Assert.AreEqual(dicts[i]["banana"], "Banana");
                                Assert.AreEqual(dicts[i]["hp"], "Amount of healthpoints");
                                break;
                            default:
                                Assert.Fail("Unknown language");
                                break;
                        }
                }
            }
        }
    }
}
