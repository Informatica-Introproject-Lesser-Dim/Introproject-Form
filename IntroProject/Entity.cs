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
        protected const int r = 25;
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
            
            chunk.removeEntity(this);

            if (this is DeathPile)
                return energyVal;
            return 0.2 * energyVal;
        }

        public virtual void draw(Graphics g, int hexX, int hexY, Entity e)
        {
            if (selected)
                g.FillEllipse(Brushes.Navy, hexX + x - r, hexY + y - r, r * 2, r * 2);
        }
    }

    public class Ghost : Entity
    {

        public Ghost(int x, int y, Hexagon hex) {
            this.x = x;
            this.y = y;
            this.chunk = hex;
        }
    }
}
