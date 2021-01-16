using System;
using System.Drawing;

using IntroProject.Core.Math;

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
    public abstract class Entity : Point2D
    {
        protected Guid id = Guid.NewGuid();

        protected const int r = 10;
        public Hexagon chunk;
        protected Color color;
        public bool dead = false;
        public double energyVal = 100;
        public int gender = -1;
        public bool isAlive { get => energyVal > 0; }
        private int disp = 4;
        public bool selected = false;
        public bool eaten = false;

        public Point GlobalLoc
        {
            get
            {
                return new Point(chunk.x +(int) this.x, chunk.y + (int)this.y);
            }
        }

        public Point ChunckRelLoc
        {
            get
            {
                return new Point(this.x, this.y);
            }
        }

        public void PerishToDeathPile()
        {
            dead = true;
            if (chunk != null)
            {
                chunk.removeEntity(this);
                chunk.addEntity(new DeathPile(this.x, this.y, 150));
            }
        }

        public double BeingEaten()
        {
            eaten = true;
            dead = true;
            return energyVal;
        }


        public virtual void draw(Graphics g, int hexX, int hexY, Entity e) {
            if (e is DeathPile)
            {
                g.FillEllipse(new SolidBrush(color), hexX + x - r, hexY + y - r, r, r);
                g.FillEllipse(new SolidBrush(color), hexX + x - r + disp, hexY + y - r + disp, r, r);
                g.FillEllipse(new SolidBrush(color), hexX + x - r + disp, hexY + y - r - disp, r, r);
                g.FillEllipse(new SolidBrush(color), hexX + x - r - disp, hexY + y - r + disp, r, r);
                g.FillEllipse(new SolidBrush(color), hexX + x - r - disp, hexY + y - r - disp, r, r);
            }
            else {
                g.FillEllipse(new SolidBrush(color), hexX + x - r, hexY + y - r, r * 2, r * 2);
                if (selected)
                    g.DrawEllipse(Pens.LightGreen, hexX + x - r, hexY + y - r, r * 2, r * 2);
            } 
        }
    }
}
