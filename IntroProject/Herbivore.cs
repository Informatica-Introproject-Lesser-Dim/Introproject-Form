using System.Drawing;

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
