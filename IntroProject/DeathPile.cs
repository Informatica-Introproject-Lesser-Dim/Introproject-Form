using System.Drawing;

namespace IntroProject
{
    public class DeathPile : Entity
    {
        protected double age;
        protected int nutritionValue;

        public DeathPile(int x, int y, int nutritionValue) //De input waarde zijn voor referentie.
        {
            this.x = x;
            this.y = y;
            this.nutritionValue = nutritionValue;
            age = 0;
            color = Color.SaddleBrown;
        }

        public override void draw(Graphics g, int hexX, int hexY, Entity e)
        {
            Image img = Properties.Resources.Skull;
            g.DrawImageUnscaled(img, hexX + x - r, hexY + y - r);
        }

        public void activate(double dt) 
        {
            age += dt;
            if (age > 100) 
            {
                this.BeingEaten();
            }
                
        }
    }
}
