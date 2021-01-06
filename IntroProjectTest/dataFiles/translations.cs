using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

namespace IntroProjectTest.DataFiles
{
    class DataFilesTest
    {
        [TestFixture]
        public class ValidateDataFiles
        {
            [TestFixture]
            public class ValidateTranslationsFile
            {
                private List<string> textFile = new List<string>();
                private List<string> keyWordList = new List<string>();
                private List<List<string>> languageList = new List<List<string>>();
                [SetUp]
                public void SetUp()
                {
                    StreamReader reader = new StreamReader(@".\dataFiles\translations.csv");
                    for (int i = 0; !reader.EndOfStream; i++)
                    {
                        textFile.Add(reader.ReadLine());
                        languageList.Add(textFile[i].Split(',').ToList());
                        keyWordList.Add(languageList[i][0]);
                    }
                }

                [Test]
                public void TestForNoDuplicates()
                {
                    Assert.AreEqual(keyWordList.Count, keyWordList.Distinct().Count());
                }
            }
        }
    }
}
