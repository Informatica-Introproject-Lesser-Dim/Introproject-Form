namespace IntroProject
{
    public class Carnivore : Creature
    {
        public Carnivore() : base() { }

        public Carnivore(Creature parentA, Creature parentB) : base(parentA, parentB) { }

        //public override Creature MateWith(Creature other)
        //{
        //    base.MateWith(other);
        //    return new Carnivore(this, other);
        //}
    }
}
