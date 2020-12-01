using NUnit.Framework;

using IntroProject;
using IntroProject.Core.Error;

namespace IntroProjectTest
{
    class CreatureTest
    {
        public sealed class CreatureTestable : Creature
        {
            public CreatureTestable() : base() { }

            public CreatureTestable(bool matingWillWork) : base()
            {
                this.isReadyToMate = matingWillWork;
            }

            public CreatureTestable(Creature parentFirst, Creature parentSecond) : base(parentFirst, parentSecond) { }


            public override Creature MateWith(Creature creature)
            {
                base.MateWith(creature);
                return new CreatureTestable(this, creature);
            }
        }

        [TestFixture]
        public class Mating
        {
            [Test]
            public void TestMatingWithSuccess()
            {
                Creature parentA = new CreatureTestable(matingWillWork: true);
                Creature parentB = new CreatureTestable(matingWillWork: true);

                parentB.MateWith(parentA);
            }

            [TestFixture]
            public class AfterMating
            {
                Creature parentA, parentB, child;

                [SetUp]
                public void SetUp()
                {
                    parentA = new CreatureTestable(matingWillWork: true);
                    parentB = new CreatureTestable(matingWillWork: true);

                    child = parentB.MateWith(parentA);
                }

                [Test]
                public void TestAfterMatingBothNotReadyForMatingAgain()
                {
                    Assert.IsFalse(parentA.isReadyToMate);
                    Assert.IsFalse(parentB.isReadyToMate);
                }

                [Test]
                public void TestAfterMatingMatingAgainThrowsInvalidMating()
                {
                    Assert.Throws<UnreadyForMating>(() => parentB.MateWith(parentA));
                }

                [Test]
                public void TestAfterMatingOnlyGenesArePassed()
                {
                    // Weak test as we don't have tons of properties yet
                    Assert.IsTrue(child.isReadyToMate);
                }
            }
        }
    }
}
