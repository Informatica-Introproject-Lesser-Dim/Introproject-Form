using System.Drawing;//shouldbechanged loats of things from entity should be here

namespace IntroProject
{
    public class Herbivore : Creature
    {
        public Herbivore() : base() {
            color = Color.Red;
        }
        public Herbivore(Creature parentA, Creature parentB) : base(parentA, parentB) { }
    }
}
