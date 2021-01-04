using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntroProject
{
    public enum EntityType
    {
        Herbivore,
        Plant,
        Carnivore,
        Entity,
        Creature
    }
    public abstract class Entity
    {
        protected int id; //Vincent: Hoe kan je er voor zorgen dat de id niet overlapt met id's van andere entities?
        public int x,y;
        protected const int r = 10;
        public Hexagon chunk;
        protected Color color;
        public bool dead = false;
        public double energyVal = 100;
        public bool selected = false;

        public Point GlobalLoc
        {
            get
            {
                return new Point(chunk.x + this.x, chunk.y + this.y);
            }
        }

        public double PerishToEnergyPile()
        {
            dead = true;
            if (chunk != null)
                chunk.removeEntity(this);
            return energyVal;
        }

        public virtual void draw(Graphics g, int hexX, int hexY) {
            g.FillEllipse(new SolidBrush(color), hexX + x - r, hexY + y - r, r * 2, r * 2);
            if (selected)
                g.DrawEllipse(Pens.LightGreen, hexX + x - r, hexY + y - r, r * 2, r * 2);
        }
    }
}
