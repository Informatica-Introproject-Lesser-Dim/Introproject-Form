using NUnit.Framework;

using IntroProject;
using IntroProject.Core.Error;

namespace IntroProjectTest
{
    class WezenTest
    {
        [TestFixture]
        public class Mating
        {
            [Test]
            public void TestMatingWithSuccess()
            {
                Wezen wezen_wild = new WezenTestable(matingWillWork: true);
                Wezen wezen_hitsig = new WezenTestable(matingWillWork: true);

                wezen_hitsig.MateWith(wezen_wild);
            }

            [TestFixture]
            public class AfterMating
            {
                Wezen wezen_wild, wezen_hitsig;

                [SetUp]
                public void SetUp()
                {
                    wezen_wild = new WezenTestable(matingWillWork: true);
                    wezen_hitsig = new WezenTestable(matingWillWork: true);

                    wezen_hitsig.MateWith(wezen_wild);
                }

                [Test]
                public void TestAfterMatingBothNotReadyForMatingAgain()
                {
                    Assert.IsFalse(wezen_wild.isReadyToMate);
                    Assert.IsFalse(wezen_hitsig.isReadyToMate);
                }

                [Test]
                public void TestAfterMatingMatingAgainThrowsInvalidMating()
                {
                    Assert.Throws<UnreadyForMating>(() => wezen_hitsig.MateWith(wezen_wild));
                }
            }
        }
    }
}
