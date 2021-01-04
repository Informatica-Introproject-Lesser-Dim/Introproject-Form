﻿using System;
using NUnit.Framework;

using IntroProject;
using System.Collections.Generic;

namespace IntroProjectTest
{
    class GeneTest
    {
        public sealed class GeneTestable : Gene
        {
            public GeneTestable(Func<bool> willMutate) => this.willMutate = willMutate;
        }

        [TestFixture]
        public class GeneCombining
        {
            public class GeneCombiningStable
            {
                Gene stableGeneSetA, stableGeneSetB;

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

                    Gene childA = new Gene(a, b);
                    Gene childB = new Gene(b, a);

                    Assert.IsTrue(childA.EqualFenoType(childB));
                }

                [Test]
                public void TestCloneIsEqual() {
                    Assert.AreEqual(stableGeneSetA, stableGeneSetA.CloneTyped());
                }

                [Test]
                public void TestCombineSameGenesStableAddEqMult()
                {
                    Gene idemGeneAPlus = stableGeneSetA + stableGeneSetA;
                    Gene idemGeneAMult = stableGeneSetA * stableGeneSetA;
                    Assert.AreEqual(idemGeneAPlus, idemGeneAMult);

                    Gene idemGeneBPlus = stableGeneSetB + stableGeneSetB;
                    Gene idemGeneBMult = stableGeneSetB * stableGeneSetB;
                    Assert.AreEqual(idemGeneBPlus, idemGeneBMult);
                }
            }

            public class GeneCombiningUnstable
            {
                Gene unstableGeneSetA, unstableGeneSetB;

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
                    Assert.AreNotEqual(unstableGeneSetA.CloneTyped().Mutate(), unstableGeneSetA);
                }
            }
        }
    }
}
