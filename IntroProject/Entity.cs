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

        public Hexagon chunk;
        protected Color color;
        public double energyVal = 100;
        protected const int r = 10;
        public int gender = -1;
        private int disp = 4;
        public bool selected = false;
        public bool eaten = false;
        public bool dead = false;

        public bool isAlive { get => energyVal > 0; }
        public Point2D GlobalLoc { get => chunk + this; }
        public Point2D ChunckRelLoc { get => this; }

        public Entity() : base() {}

        public void PerishToDeathPile()
        {
            dead = true;
            if (chunk != null)
            {
                chunk.removeEntity(this);
                chunk.addEntity(new DeathPile(x, y, 150));
            }
        }

        public double BeingEaten()
        {
            eaten = true;
            dead = true;
            return energyVal;
        }

        public virtual void draw(Graphics g, int hexX, int hexY, Entity e)
        {
            if (e is DeathPile)
            {
                g.FillEllipse(new SolidBrush(color), hexX + x - r, hexY + y - r, r, r);
                g.FillEllipse(new SolidBrush(color), hexX + x - r + disp, hexY + y - r + disp, r, r);
                g.FillEllipse(new SolidBrush(color), hexX + x - r + disp, hexY + y - r - disp, r, r);
                g.FillEllipse(new SolidBrush(color), hexX + x - r - disp, hexY + y - r + disp, r, r);
                g.FillEllipse(new SolidBrush(color), hexX + x - r - disp, hexY + y - r - disp, r, r);
            }
            else
            {
                g.FillEllipse(new SolidBrush(color), hexX + x - r, hexY + y - r, r * 2, r * 2);
                if (selected)
                    g.DrawEllipse(Pens.LightGreen, hexX + x - r, hexY + y - r, r * 2, r * 2);
            } 
        }
    }
}
