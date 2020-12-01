using System;
using NUnit.Framework;

using IntroProject;

namespace IntroProjectTest
{
    class GenenTest
    {
        public sealed class GenenTestable : Genen
        {
            public GenenTestable(Func<bool> willMutate) => this.willMutate = willMutate;
        }

        [TestFixture]
        public class GeneCombining
        {
            public class GeneCombiningStable
            {
                Genen stableGeneSetA, stableGeneSetB;

                [SetUp]
                public void SetUp()
                {
                    stableGeneSetA = new GenenTestable(willMutate: () => false);
                    stableGeneSetB = new GenenTestable(willMutate: () => false);
                }

                [Test]
                public void TestCombiningSameGenesNoRefEqCheck()
                {
                    Assert.IsFalse(ReferenceEquals(stableGeneSetA, stableGeneSetA * stableGeneSetA));
                }

                [Test]
                public void TestCombiningSameGenesResultsInSameGenes()
                {
                    Genen idemGenesAPlus = stableGeneSetA + stableGeneSetA;
                    Assert.AreEqual(idemGenesAPlus, stableGeneSetA);

                    Genen idemGenesBPlus = stableGeneSetB + stableGeneSetB;
                    Genen idemGenesBMult = stableGeneSetB * stableGeneSetB;
                }

                [Test]
                public void TestCombineSameGenesStableAddEqMult()
                {
                    Genen idemGenesAPlus = stableGeneSetA + stableGeneSetA;
                    Genen idemGenesAMult = stableGeneSetA * stableGeneSetA;
                    Assert.AreEqual(idemGenesAPlus, idemGenesAMult);

                    Genen idemGenesBPlus = stableGeneSetB + stableGeneSetB;
                    Genen idemGenesBMult = stableGeneSetB * stableGeneSetB;
                    Assert.AreEqual(idemGenesBPlus, idemGenesBMult);

                }
            }

            public class GeneCombiningUnstable
            {
                Genen unstableGeneSetA, unstableGeneSetB;

                [SetUp]
                public void SetUp()
                {
                    unstableGeneSetA = new GenenTestable(willMutate: () => true);
                    unstableGeneSetB = new GenenTestable(willMutate: () => true);
                }

                [Test]
                public void TestCombiningSameGenesResultsInDifferentGenes()
                {
                    Genen mutatedGenesA = unstableGeneSetA * unstableGeneSetA;
                    Assert.AreNotEqual(mutatedGenesA, unstableGeneSetA);

                    Genen mutatedGenesB = unstableGeneSetB * unstableGeneSetB;
                    Assert.AreNotEqual(mutatedGenesB, unstableGeneSetB);
                }

                [Test]
                public void TestCombineSameGenesStableAddNotEqMult()
                {
                    Genen idemGenesAPlus = unstableGeneSetA + unstableGeneSetA;
                    Genen idemGenesAMult = unstableGeneSetA * unstableGeneSetA;
                    Assert.AreNotEqual(idemGenesAPlus, idemGenesAMult);

                    Genen idemGenesBPlus = unstableGeneSetB + unstableGeneSetB;
                    Genen idemGenesBMult = unstableGeneSetB * unstableGeneSetB;
                    Assert.AreNotEqual(idemGenesBPlus, idemGenesBMult);
                }
            }
        }
    }
}
