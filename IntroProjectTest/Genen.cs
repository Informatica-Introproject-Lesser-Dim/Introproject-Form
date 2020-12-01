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
                    Genen idemGenesA = stableGeneSetA * stableGeneSetA;
                    Assert.AreEqual(idemGenesA, stableGeneSetA);

                    Genen idemGenesB = stableGeneSetB * stableGeneSetB;
                    Assert.AreEqual(idemGenesB, stableGeneSetB);
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
            }
        }
    }
}
