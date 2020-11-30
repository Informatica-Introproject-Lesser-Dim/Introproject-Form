﻿using NUnit.Framework;

using IntroProject;
using IntroProject.Core.Error;

namespace IntroProjectTest
{
    class WezenTest
    {
        public sealed class WezenTestable : Wezen
        {
            public WezenTestable() : base() { }

            public WezenTestable(bool matingWillWork) : base()
            {
                this.isReadyToMate = matingWillWork;
            }

            public WezenTestable(Wezen ouder1, Wezen ouder2) : base(ouder1, ouder2) { }

            public override Wezen MateWith(Wezen wezen)
            {
                base.MateWith(wezen);
                return new WezenTestable(this, wezen);
            }
        }

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
