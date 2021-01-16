using System;
using NUnit.Framework;

using IntroProject;
using System.Collections.Generic;

namespace IntroProjectTest
{
    class GeneTest
    {
        /// <summary>
        /// GeneTestable inherits from a Gene inheritor due to the operators not being available for Gene
        /// </summary>
        public sealed class GeneTestable : HerbivoreGene
        {
            public GeneTestable(Func<bool> willMutate) => this.willMutate = willMutate;
            public GeneTestable(List<float[]> allelParentA, List<float[]> allelParentB) : base(allelParentA, allelParentB) { }
        }

        [TestFixture]
        public class GeneCombining
        {
            public class GeneCombiningStable
            {
                GeneTestable stableGeneSetA, stableGeneSetB;

                [SetUp]
                public void SetUp()
                {
                    stableGeneSetA = new GeneTestable(willMutate: () => false);
                    stableGeneSetB = new GeneTestable(willMutate: () => false);
                }

                [Test]
                public void TestCombiningSameGenesNoRefEqCheck()
                {
                    Assert.IsFalse(ReferenceEquals(stableGeneSetA, stableGeneSetA * stableGeneSetA));
                }

                [Test]
                public void TestOrderOfAddingGenesResultsInSameFenotype() 
                {
                    List<float[]> a = stableGeneSetA.getAllel();
                    List<float[]> b = stableGeneSetB.getAllel();

                    Gene childA = new GeneTestable(a, b);
                    Gene childB = new GeneTestable(b, a);

                    Assert.IsTrue(childA.EqualFenoType(childB));
                }

                [Test]
                public void TestCloneIsEqual() {
                    Assert.AreEqual(stableGeneSetA, stableGeneSetA.CloneTyped());
                }
            }

            public class GeneCombiningUnstable
            {
                GeneTestable unstableGeneSetA, unstableGeneSetB;

                [SetUp]
                public void SetUp()
                {
                    unstableGeneSetA = new GeneTestable(willMutate: () => true);
                    unstableGeneSetB = new GeneTestable(willMutate: () => true);
                }

                [Test]
                public void TestCombiningSameGenesResultsInDifferentGenes()
                {
                    Gene mutatedGeneA = unstableGeneSetA * unstableGeneSetA;
                    Assert.AreNotEqual(mutatedGeneA, unstableGeneSetA);

                    Gene mutatedGeneB = unstableGeneSetB * unstableGeneSetB;
                    Assert.AreNotEqual(mutatedGeneB, unstableGeneSetB);
                }

                [Test]
                public void TestCombineSameGenesStableAddNotEqMult()
                {
                    Gene idemGeneAPlus = unstableGeneSetA + unstableGeneSetA;
                    Gene idemGeneAMult = unstableGeneSetA * unstableGeneSetA;
                    Assert.AreNotEqual(idemGeneAPlus, idemGeneAMult);

                    Gene idemGeneBPlus = unstableGeneSetB + unstableGeneSetB;
                    Gene idemGeneBMult = unstableGeneSetB * unstableGeneSetB;
                    Assert.AreNotEqual(idemGeneBPlus, idemGeneBMult);
                }


                [Test]
                public void TestOriginalGeneNotAffectedByMutationOperation()
                {
                    Gene _;
                    var cloneA = unstableGeneSetA.CloneTyped();
                    _ =  unstableGeneSetA * unstableGeneSetA;
                    Assert.AreEqual(unstableGeneSetA, cloneA);

                    var cloneB = unstableGeneSetB.CloneTyped();
                    _ =  unstableGeneSetB * unstableGeneSetB;
                    Assert.AreEqual(unstableGeneSetB, cloneB);
                }

                [Test]
                public void TestGeneChangesWhenMutating()
                {
                    Assert.IsFalse(unstableGeneSetA.CloneTyped().Mutate().Equals(unstableGeneSetA));
                }
            }
        }
    }
}
