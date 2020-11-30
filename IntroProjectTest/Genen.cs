using NUnit.Framework;

using IntroProject;

namespace IntroProjectTest
{
    class GenenTest
    {
        [TestFixture]
        public class GeneCombining
        {
            Genen genenSetA, genenSetB;

            [SetUp]
            public void SetUp()
            {
                genenSetA = new Genen();
                genenSetB = new Genen();
            }

            [Test]
            public void TestCombiningSameGenesResultsInSameGenes()
            {
                Genen idemGenesA = genenSetA + genenSetA;
                Assert.AreEqual(idemGenesA, genenSetA);

                Genen idemGenesB = genenSetB + genenSetB;
                Assert.AreEqual(idemGenesB, genenSetB);
            }
        }
    }
}
