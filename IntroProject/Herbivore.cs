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
        public override void draw(Graphics g, int hexX, int hexY, Entity e)
        {
            Image img;
            switch(goal)
            {
                case Goal.Food:
                  img = Properties.Resources.Zebra_Food;
                  break;
                case Goal.Mate:
                  img = Properties.Resources.Zebra_Mate;
                  break;
                default:
                  img = Properties.Resources.Zebra_Normal;
                  break;
            }
            g.DrawImageUnscaled(img, hexX + x - r, hexY + y - r);
            if (selected)
                g.DrawEllipse(Pens.LightGreen, hexX + x - r, hexY + y - r, r * 2, r * 2);
        }
    }
}
