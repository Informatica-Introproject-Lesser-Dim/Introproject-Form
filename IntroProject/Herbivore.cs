namespace IntroProject
{
    public class Herbivore : Creature
    {
        public Herbivore() : base() {}
        public Herbivore(Creature parentA, Creature parentB) : base(parentA, parentB) { }

        public override Creature MateWith(Creature other)
        {
            base.MateWith(other);
            return new Herbivore(this, other);
        }
    }
}
