using System.Drawing;

namespace IntroProject
{
    public class Herbivore : Creature
    {
        public Herbivore() : base() =>
            color = Color.Red;

        public Herbivore(Gene gene, double energy) : base(gene, energy) { }

        public override Creature FromParentInfo(Gene gene, double energy) =>
            new Herbivore(gene, energy);
    }
}
