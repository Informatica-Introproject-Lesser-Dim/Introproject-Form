using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using System.Linq;

namespace IntroProjectTest
{
    class ScreenTest
    {
        [TestFixture]
        public class TextFile
        {
            public class CheckForNoDuplicates
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
