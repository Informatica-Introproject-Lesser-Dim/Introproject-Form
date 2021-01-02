using IntroProject.Core.Error;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace IntroProject
{
    enum Goal {
        Food,
        Mate,
        Water, //might be implemented later
        Nothing
    }

    public class Creature : Entity
    {
        public Gene gene { get; protected set; }
        public int isAlive;
        public bool isReadyToMate = true;
        private Route route;
        private Creature target;

        private Grass myFood;
        private int sleep = 0;
        private Goal goal = Goal.Nothing;
        private bool passive = false;
        public int coolDown = 200; //cooldown so the creature doesnt continuously attempt mating

        public Point GlobalLoc {
            get {
                return new Point(chunk.x + this.x, chunk.y + this.y);
            }
        }


        public Creature()
        {
            energyVal = 50;
            gene = new Gene();

            gene.@class = this.GetType().Name;
        }

        public Creature(Gene gene, int energy) {
            this.gene = gene;
            this.energyVal = energy;

            gene.@class = this.GetType().Name;
        }

        public static int calcDistance2(EntityType type, Hexagon place, Point point) { //enter the world relative position for point
            List<Entity> targets = new List<Entity>();
            for (int l = 0; targets.Count == 0 && l < 10; l++)
                targets = place.searchPoint(l, type);
            if (targets.Count == 0)
                return 10000000; //just a big number so stuff doesnt break
            int dist = calcDist2(targets[0], point);
            foreach (Entity e in targets)
                if (calcDist2(e, point) < dist)
                    dist = calcDist2(e, point);
            return dist;
        }

        public void calcFoodDist() {
            int dist = (int)(Math.Sqrt(calcDistance2(EntityType.Plant, this.chunk, new Point(x + this.chunk.x, y + this.chunk.y))));
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

        public Creature(Gene parentA, Gene parentB) { }

        public void eat(Entity entity) {
            if (entity == null)
                return;
            if (entity.dead)
                return;
            this.energyVal += entity.Die();
            this.sleep = 30;
        }

        // Assuming this is the same type as wezen: we don't want Herbivores mating Carnivores
        //public virtual void MatingSuccess()
        //{
        //    isReadyToMate = false;
        //}

        //public virtual Creature? MateWith(Creature other)
        //{
        //    if (!this.isReadyToMate || !other.isReadyToMate)
        //        throw new UnreadyForMating();

        //    this.MatingSuccess();
        //    other.MatingSuccess();
        //    return this;
        //}

        private void MateWith(Creature other) { //this is the mating method called by the males
            //create a new creature, remove energy accordingly and set a "cooldown timer" for both the creatures
            this.coolDown = 1000;
            other.coolDown = 1000;
            other.Mate(this);
            this.energyVal -= (int) (this.gene.Size * 0.05); //the males barely lose any energy
            this.goalReset();
        }

        public void Mate(Creature other) {//this is the female mating method, will be called from within the male mating method
            int transferredEnergy = (int) (this.energyVal * this.gene.energyDistribution);
            energyVal -= transferredEnergy;
            Creature child = new Creature(this.gene * other.gene, transferredEnergy);
            child.x = this.x;
            child.y = this.y;
            this.chunk.EntityBirth(child);
            //Now we only need to put this child in the entity list

            this.goalReset();
        }

        public void activate() {
            if (this.coolDown > 0)
                coolDown--;

            if (sleep > 0) {
                sleep--;
                if (goal != Goal.Mate)
                    this.color = Color.FromArgb(50, 50, 150);
                return;
            }

            if (this.goal == Goal.Mate) 
                this.mateActive();

            if (route != null) //just move along the road if you have one, otherwise search for a new route
                this.move();
            else {
                passiveSearch();
            }
        }

        private void mateActive() {
            if (this.gene.Gender != 1) { //if a female reaches this point then that means her "sleep" hasnt continuously been resetted, aka no one's interested anymore
                this.goalReset();
                return;
            }
            if (target == null) {
                this.goalReset();
                return;
            }
            if (target.goal != Goal.Mate) //if the female has moved on he will do something else
                this.goalReset();

            this.target.sleep = 10; //ensures the female keeps patiently waiting for their mate

            return;
        }

        private void goalReset() {
            this.route = null;
            this.goal = Goal.Nothing;
        }

        public void passiveSearch() {
            //check wether the place you are is ok
            goal = Goal.Food;
            if (passiveCheck(this.chunk))
            {
                Route route = new Route(new Point(this.x, this.y), this.chunk.size, this.chunk);
                myFood = chunk.bestFood(new Point(this.x, this.y));

                route.addEnd(new Point(myFood.loc.X, myFood.loc.Y));
                this.route = route;
                this.goal = Goal.Food;
                Color.FromArgb(50, 100, 50);
                //set it as your target and make the route towards it your own
                return;
            }

            //check wether any of the places around you are ok
            int dir = -1;
            double val = 0;
            double temp;
            for (int i = 0; i < 6; i++) //try to find one of the neighbors that's good enough
                if (this.chunk[i] != null)
                {
                    if (this.gene.JumpHeight < this.chunk[i].heightOfTile - this.chunk.heightOfTile)
                        continue; //check wether you can even move there
                    if (passiveCheck(this.chunk[i]))
                    {
                        temp = passiveVal(this.chunk[i]);
                        if (dir == -1 || temp > val)
                        {
                            dir = i;
                            val = temp;
                            continue;
                        }


                    }
                }
            if (dir != -1) {
                //if so go there and eat in there
                Route route = new Route(new Point(this.x, this.y), this.chunk.size, this.chunk);
                route.addDirection(dir);

                myFood = chunk[dir].bestFood(Hexagon.calcSide(chunk.size, (dir + 3) % 6));

                route.addEnd(new Point(myFood.loc.X, myFood.loc.Y));
                this.route = route;
                this.goal = Goal.Food;
                this.color = Color.FromArgb(50, 100, 50);
                return;
            }
            //if the passive search has failed
            activeSearch();
        }

        private bool passiveCheck(Hexagon hex) {
            if (hex.vegetation.FoodLocations().Count < 1)
                return false;
            return gene.PassiveBias < passiveVal(hex);
        }

        private bool matingSearch() { //returns true if it's succesfull
            if (this.coolDown > 0)
                return false;
            if (this.gene.Gender == 1)
                return false;
            if (this.energyVal < this.gene.energyDistribution * this.gene.Size / 2 + 50)
                return false;

            List<Entity> herbivores = new List<Entity>();//find all creatures
            for (int i = 0; i < 8; i++)
                herbivores =  herbivores.Concat(chunk.searchPoint(i, EntityType.Herbivore)).ToList();


            List<Creature> creatures = new List<Creature>(); 

            foreach (Entity e in herbivores) //select the few that exceed your preference 
                if (((Creature)e).gene.Gender == 1 && ((Creature)e).energyVal > this.gene.sexualPreference)
                    creatures.Add((Creature)e);

            if (creatures.Count == 0)
                return false;

            bool temp = false;
            foreach (Entity e in herbivores)
                temp = temp || ((Creature) e).available(this); //turns into true if at least one says yes
            if (!temp)
                return false;

            this.goal = Goal.Mate;
            this.color = Color.Pink;
            this.sleep = 10;
            return true;
        }

        public bool available(Creature creature) { // call this method to "spread your feromones"
            //both tells you wether the entity is interested and makes the entity go towards you
            if (this.coolDown > 0)
                return false;

            if (this.gene.Gender != 1) //females arent interested
                return false;

            //does need to be someone of your preference
            if (creature.energyVal < this.gene.sexualPreference)
                return false;

            //do you have enough energy yourself
            Point a = creature.GlobalLoc;
            Point b = this.GlobalLoc;
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy); //straight distance towards creature
            if (energyVal - dist * Calculator.EnergyPerMeter(this.gene.Velocity) < this.gene.sexualPreference)
                return false;

            //Pathfinding...
            SingleTargetAStar aStar = new SingleTargetAStar(new Point(this.x, this.y), this.chunk, this.gene, this.chunk.size, this.energyVal, creature);
            route = aStar.getResult();

            if (route != null) {
                goal = Goal.Mate;
                this.color = Color.Pink;
                this.target = creature;
            }
            return true;
        }

        private double passiveVal(Hexagon hex) {
            float hunger = (gene.Size - energyVal) * gene.HungerBias;

            return hex.Passive(hunger, gene.DistanceBias);
        }

        public void activeSearch() { //right now this is still normal Astar, this needs to change to the new vegetation update
            if (matingSearch())
                return;
            //check if it's possible to mate first
            AStar aStar = new AStar(new Point(this.x, this.y), this.chunk, this.gene, this.chunk.size,this.energyVal);
            route = aStar.getResult();

            if (route != null)
            {
                myFood = aStar.getTarget();
                goal = Goal.Food;
                this.color = Color.FromArgb(150, 50, 50);
                if (route.quality < gene.ActivePreference) {
                    this.color = Color.FromArgb(50, 50, 50);
                    goal = Goal.Nothing;
                }
                    
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
                if (goal == Goal.Mate) {
                    if (target.goal != Goal.Mate) {
                        this.route = null;
                        this.goal = Goal.Nothing;
                        return;
                    }
                }
                else if (myFood != null)
                {
                    if (!myFood.visible) {
                        route = null;
                        return;
                    }
                }

                Point temp;
                if (route.move(gene.Velocity)) {
                    temp = route.getPos();
                    x = temp.X;
                    y = temp.Y;
                    if (route.isDone()) {
                        route = null; //put any functions wich are to activate when the route is done here
                        if (goal == Goal.Food)
                            eat(myFood);
                        if (goal == Goal.Mate)
                            MateWith(target);
                        myFood = null;
                        sleep += 30;
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
        private void eat(Grass grass)
        {
            if (grass == null)
                return;
            grass.visible = false;
            this.energyVal += grass.getVal(this.chunk.vegetation.currentTime);
        }
    }

}
