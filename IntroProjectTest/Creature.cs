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
                this.coolDown = matingWillWork ? 0 : int.MaxValue;
                this.chunk = new Hexagon(size: 10, c: 10, x: 10, y: 10, longitudeOnMap: 0, new Map(width: 1, height: 1, size: 1, margin: 0));
            }

            public CreatureTestable(Creature parentFirst, Creature parentSecond) : base(parentFirst, parentSecond) { }
        }

        [TestFixture]
        public class Mating
        {
            [Test]
            public void TestMatingWithSuccess()
            {
                Creature parentA = new CreatureTestable(matingWillWork: true);
                Creature parentB = new CreatureTestable(matingWillWork: true);

                parentB.MateWithMale(parentA);
            }

            [TestFixture]
            public class AfterMating
            {
                Creature parentA, parentB;

                [SetUp]
                public void SetUp()
                {
                    parentA = new CreatureTestable(matingWillWork: true);
                    parentB = new CreatureTestable(matingWillWork: true);
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
                    Assert.Throws<UnreadyForMating>(() => parentB.MateWithMale(parentA));
                }
            }
        }
    }
}
