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

        private Grass myFood;
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
            if (passiveCheck(this.chunk))
            {
                Route route = new Route(new Point(this.x, this.y), this.chunk.size, this.chunk);
                myFood = chunk.bestFood(new Point(this.x, this.y));

                route.addEnd(new Point(myFood.loc.X, myFood.loc.Y));
                this.route = route;
                //set it as your target and make the route towards it your own
                return;
            }

            //check wether any of the places around you are ok
            int dir = -1;
            double val = 0;
            double temp;
            for (int i = 0; i < 6; i++) //try to find one of the neighbors that's good enough
                if (this.chunk[i] != null)
                    if (passiveCheck(this.chunk[i])) {
                        temp = passiveVal(this.chunk[i]);
                        if (dir == -1 || temp > val) {
                            dir = i;
                            val = temp;
                            continue;
                        }
                         
                            
                    }
            if (dir != -1) {
                //if so go there and eat in there
                Route route = new Route(new Point(this.x, this.y), this.chunk.size, this.chunk);
                route.addDirection(dir);

                myFood = chunk[dir].bestFood(Hexagon.calcSide(chunk.size, (dir + 3)%6));

                route.addEnd(new Point(myFood.loc.X, myFood.loc.Y));
                this.route = route;
                return;
            }
            activeSearch();
        }

        private bool passiveCheck(Hexagon hex) {
            if (hex.vegetation.FoodLocations().Count < 1)
                return false;
            return gene.PassiveBias < passiveVal(hex);
        }

        private double passiveVal(Hexagon hex) {
            float hunger = (gene.Size - energyVal) * gene.HungerBias;

            return hex.Passive(hunger, gene.DistanceBias);
        }

        public void activeSearch() { //right now this is still normal Astar, this needs to change to the new vegetation update
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
