using NUnit.Framework;

using IntroProject;

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

                Wezen kind = wezen_hitsig.MateWith(wezen_wild);
            }
        }
    }
}
