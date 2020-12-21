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

                    int i = 0;
                    StreamReader reader = new StreamReader(@".\dataFiles\translations.csv");
                    while (!reader.EndOfStream)
                    {
                        textFile[i] = reader.ReadLine();

                        languageList[i] = textFile[i].Split(';').ToList();
                        keyWordList[i] = languageList[i][0];
                        i++;
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
