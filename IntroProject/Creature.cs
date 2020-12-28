using IntroProject.Core.Error;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace IntroProject
{
    enum Goal {
        Food,
        Mate,
        Water, //might be implemented later
        Nothing
    }

    public abstract class Creature : Entity
    {
        public Gene gene { get; protected set; }
        public int isAlive;
        public bool isReadyToMate = true;
        private Route route;
        private Entity target;
        private int sleep = 0;
        private Goal goal = Goal.Nothing;
        private bool passive = false;
        

        public Creature()
        {
            gene = new Gene();
            
            gene.@class = this.GetType().Name;
            // gene should be random at first
        }

        public static int calcDistance2(EntityType type, Hexagon place, Point point) { //enter the world relative position for point
            List<Entity> targets = new List<Entity>();
            for (int l = 0; targets.Count == 0 && l < 10; l++)
                targets = place.searchPoint(l, type);
            if (targets.Count == 0)
                return 10000000; //just a big number so stuff doesnt break
            int dist = calcDist2(targets[0], point);
            foreach (Entity e in targets)
                if (calcDist2(e,point) < dist)
                    dist = calcDist2(e,point);
            return dist;
        }

        public void calcFoodDist() {
            int dist = (int) (Math.Sqrt(calcDistance2(EntityType.Plant, this.chunk, new Point(x + this.chunk.x,y + this.chunk.y))));
            if (dist > 255)
                dist = 255;
            this.color = Color.FromArgb(255 - dist, 50, 50);
        }

        public static int calcDist2(Entity e, Point p) {
            int x2 = e.x + e.chunk.x;
            int y2 = e.y + e.chunk.y;
            int dx = p.X - x2;
            int dy = p.Y - y2;
            return dx * dx + dy * dy;
        }
        
        public Creature(Creature parentA, Creature parentB) : this(parentA.gene, parentB.gene) { }

        public Creature(Gene parentA, Gene parentB) {}

        public void eat(Entity entity) {
            if (entity == null)
                return;
            if (entity.dead)
                return;
            this.energyVal += entity.Die();
            this.sleep = 30;
        }

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

        public void search() {
            
                
        }

        public void activate() {
            if (sleep > 0) {
                sleep--;
                return;
            }

            if (route != null) //just move along the road if you have one, otherwise search for a new route
                this.move();
            else {
                passiveSearch();
            }
        }

        public void passiveSearch() {
            //check wether the place you are is ok
            //if so go to one of the food bits in it
            //check wether any of the places around you are ok
            //if so go there and eat in there
            activeSearch();
        }

        public void activeSearch() {
            AStar aStar = new AStar(new Point(this.x, this.y), this.chunk, this.gene, this.chunk.size);
            route = aStar.getResult();

            if (route != null)
            {
                target = aStar.getTarget();
            }
            else { sleep = 20; }
        }

        public void checkSurroundings() {
            //check if target plant has been eaten

            //check if predators are near

            //check if there's a new possible route (if it doesnt have one yet)
        }


        public void move() { //needs a rework cus the target wont be an entity
            if (route != null) {
                if (target.dead)
                {   route = null;
                    return;
                }

                Point temp;
                if (route.move(gene.Velocity)) {
                    temp = route.getPos();
                    x = temp.X;
                    y = temp.Y;
                    if (route.isDone()) {
                        route = null; //put any functions wich are to activate when the route is done here
                        eat(target);
                        return;
                    }
                    this.chunk.moveEntity(this, route.getDir());
                    return;
                }
                temp = route.getPos();
                x = temp.X;
                y = temp.Y;
                return;
            }
            //do whatever and entity should do when it doesnt have a route
        }
    }
}
