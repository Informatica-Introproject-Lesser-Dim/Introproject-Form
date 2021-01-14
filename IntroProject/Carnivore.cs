namespace IntroProject
{
    public class Carnivore : Creature
    {
        public Carnivore() : base() { }

        public Carnivore(Creature parentA, Creature parentB) : base(parentA, parentB) { }

        public Carnivore(Gene gene, double energy) : base(gene, energy) { }
        public override Creature FromParentInfo(Gene gene, double energy) =>
            new Carnivore(gene, energy);
    }
}
