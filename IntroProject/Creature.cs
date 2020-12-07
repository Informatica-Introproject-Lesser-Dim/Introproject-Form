using IntroProject.Core.Error;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace IntroProject
{
    public abstract class Creature : Entity
    {
        public Gene gene { get; protected set; }
        public int isAlive;
        public bool isReadyToMate = true;

        public Creature()
        {
            gene = new Gene();
            gene.@class = this.GetType().Name;
            // gene should be random at first
        }

        public int calcDistance2(EntityType type) {
            List<Entity> targets = new List<Entity>();
            for (int l = 0; targets.Count == 0; l++)
                targets = this.chunk.searchPoint(l, type);
            int dist = calcDist2(targets[0]);
            foreach (Entity e in targets)
                if (calcDist2(e) < dist)
                    dist = calcDist2(e);
            return dist;
        }

        public void calcFoodDist() {
            int dist = (int) (Math.Sqrt(calcDistance2(EntityType.Plant)));
            if (dist > 255)
                dist = 255;
            this.color = Color.FromArgb(255 - dist, 50, 50);
        }

        public int calcDist2(Entity e) {
            int x1 = this.x + this.chunk.x;
            int y1 = this.y + this.chunk.y;
            int x2 = e.x + e.chunk.x;
            int y2 = e.y + e.chunk.y;
            int dx = x1 - x2;
            int dy = y1 - y2;
            return dx * dx + dy * dy;
        }

        public Creature(Creature parentA, Creature parentB) : this(parentA.gene, parentB.gene) { }

        public Creature(Gene parentA, Gene parentB) {}

        public void eat(Entity entity) { }

        // Assuming this is the same type as wezen: we don't want Herbivores mating Carnivores
        public virtual void MatingSuccess()
        {
            isReadyToMate = false;
        }

        public virtual Creature? MateWith(Creature other)
        {
            if (!this.isReadyToMate || !other.isReadyToMate)
                throw new UnreadyForMating();

            this.MatingSuccess();
            other.MatingSuccess();
            return this;
        }
    }
}
