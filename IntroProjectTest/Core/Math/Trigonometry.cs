using NUnit.Framework;

using IntroProject.Core.Math;

namespace IntroProjectTest.Core.Math
{
    class TrigonometryTest
    {
        [TestFixture]
        public class Hypo
        {
            [Test]
            public void TestZero() =>
                Assert.Zero(Trigonometry.Hypo(0, 0));

            [Test]
            public void TestExactTriplet() =>
                Assert.AreEqual(5, Trigonometry.Hypo(3, 4));
        }

        [TestFixture]
        public class Distance
        {
            [Test]
            public void TestZero() =>
                Assert.Zero(Trigonometry.Distance((42, 69), (42, 69)));

            [Test]
            public void TestExactTripletDistance() =>
                Assert.AreEqual(5, Trigonometry.Distance((4, 2), (1, 6)));
        }
    }
}
