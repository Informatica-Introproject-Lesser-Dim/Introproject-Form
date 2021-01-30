using System;
using System.Diagnostics;
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
            base.draw(g, hexX, hexY, e);
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
            try 
            {
                g.DrawImageUnscaled(img, hexX + x - r, hexY + y - r);
            }
            catch (Exception exept)
            {
                Debug.WriteLine("exeption found", exept);
            }
        }
    }
}
