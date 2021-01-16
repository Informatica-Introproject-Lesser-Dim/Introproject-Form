using IntroProject.Core.Error;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace IntroProject
{
    public enum Goal {
        Food,
        Mate,
        Water, //might be implemented later
        Nothing,
        Creature
    }


    public class Creature : Entity
    {
        public Gene gene { get; protected set; }
        public int Alive;
        public bool isReadyToMate { get => coolDown == 0; }
        protected Route route;
        private Creature mateTarget;

        protected Entity target;

        protected static float MateWeight = Settings.MatingCost;
        private Grass myFood;
        protected double sleep = 0;
        protected Goal goal = Goal.Nothing;
        private bool passive = false;
        public double coolDown = 200; //cooldown so the creature doesnt continuously attempt mating
        protected float maxEnergy;
        public double stamina;

        public Creature()
        {
            energyVal = 600; //start with quite a lot so they dont die too quickly
            gene = new HerbivoreGene();
            maxEnergy = this.gene.Size;

            gene.@class = this.GetType().Name;
            stamina = gene.SprintDuration;
        }

        protected Entity findClosest(List<Entity> targets) {

            Entity result = targets[0];
            double dist = calcDistancePow2(result);

            foreach (Entity e in targets) {

                double newDist = calcDistancePow2(e);
                if (dist > newDist) {
                    result = e;
                    dist = newDist;
                }
                    

            }
            return result;
        }

        private double calcDistancePow2(Entity e) {
            double dx = e.GlobalLoc.X - this.GlobalLoc.X;
            double dy = e.GlobalLoc.Y - this.GlobalLoc.Y;
            return dx * dx + dy * dy;
        }

        private void TransferParentInfo(Gene gene, double energy)
        {
            this.gene = gene;
            this.energyVal = energy;
            maxEnergy = this.gene.Size;

            gene.@class = this.GetType().Name;
            stamina = gene.SprintDuration;
        }
        public Creature(Gene gene, double energy) =>
            TransferParentInfo(gene, energy);

        public virtual Creature FromParentInfo(Gene gene, double energy) =>
            new Creature(gene, energy);

        public static int calcDistancePow2(EntityType type, Hexagon place, Point point) { //enter the world relative position for point
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
            this.energyVal += entity.BeingEaten();
            this.sleep = 30;
        }

        protected void MateWithFemale(Creature other)
        {
            //create a new creature, remove energy accordingly and set a "cooldown timer" for both the creatures
            if (0 < (this.coolDown + other.coolDown))
                throw new UnreadyForMating();

            this.coolDown = 300;
            other.coolDown = 300;
            other.MateWithMale(this);
            this.energyVal -= (int) (this.gene.Size * (0.1 * MateWeight)); //the males barely lose any energy
            this.goalReset();
        }

        public virtual void MateWithMale(Creature other)
        {
            double transferredEnergy =  this.energyVal * (this.gene.energyDistribution * MateWeight);
            energyVal -= transferredEnergy;
            Creature child = null;
            if (this is Herbivore)
                child = other.FromParentInfo((HerbivoreGene)this.gene * (HerbivoreGene)other.gene, transferredEnergy);
            else if (this is Carnivore)
                child = other.FromParentInfo((CarnivoreGene)this.gene * (CarnivoreGene)other.gene, transferredEnergy);

            child.x = this.x;
            child.y = this.y;
            child.sleep = 20;
            this.chunk.EntityBirth(child);
            //Now we only need to put this child in the entity list

            this.goalReset();
        }

        public void activate(double dt) {
            if (stamina < gene.SprintDuration)
                stamina += dt;
            this.energyVal -= Calculator.StandardEnergyCost(gene)*dt;
            if (this.coolDown > 0)
                coolDown -= dt;

            if (this.energyVal <= 0)
                this.dead = true;

            if (sleep > 0)
            {
                sleep -= dt;
                if (goal != Goal.Mate)
                    this.color = Color.FromArgb(50, 50, 150);
                return;
            }

            if (this.goal == Goal.Creature) {
                if (SprintToCreature(dt))
                    goal = Goal.Nothing;
                return;
            }
                

            if (this.goal == Goal.Mate) 
                this.mateActive();

            if (route != null) //just move along the road if you have one, otherwise search for a new route
                this.move(dt);
            else {
                passiveSearch();
            }
        }

        protected virtual bool SprintToCreature(double dt) {
            return true;
        }

        private void mateActive() {
            if (this.gene.Gender != 1) { //if a female reaches this point then that means her "sleep" hasnt continuously been resetted, aka no one's interested anymore
                this.goalReset();
                return;
            }
            if (mateTarget == null) {
                this.goalReset();
                return;
            }
            if (mateTarget.goal != Goal.Mate) //if the female has moved on he will do something else
                this.goalReset();

            this.mateTarget.sleep = 10; //ensures the female keeps patiently waiting for their mate

            return;
        }

        private void goalReset() {
            this.route = null;
            this.goal = Goal.Nothing;
        }

        public virtual void passiveSearch() {
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

        protected virtual bool matingSearch() { //returns true if it's succesfull
            if (this.coolDown > 0)
                return false;
            if (this.gene.Gender == 1)
                return false;
            if (this.energyVal < this.gene.energyDistribution * this.gene.Size / 2 + 50)
                return false;

            EntityType targetType = EntityType.Herbivore;
            if (this is Carnivore)
                targetType = EntityType.Carnivore;

            List<Entity> creatures = new List<Entity>();//find all creatures
            for (int i = 0; i < 8; i++)
                creatures =  creatures.Concat(chunk.searchPoint(i, targetType)).ToList();


            List<Creature> accepted = new List<Creature>();


            

            foreach (Entity e in creatures) //select the few that exceed your preference 
                if (((Creature)e).gene.Gender == 1 && ((Creature)e).energyVal > this.gene.sexualPreference)
                    accepted.Add((Creature)e);

            if (accepted.Count == 0)
                return false;

            bool temp = false;
            foreach (Entity e in accepted)
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
            if (this.coolDown > 0 || this.goal == Goal.Mate)
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
            if (energyVal - dist * Calculator.EnergyPerMeter(this.gene) < this.gene.sexualPreference) 
                return false; //stopping if it costs too much energy

            //Pathfinding...
            this.color = Color.Pink;
            SingleTargetAStar aStar = new SingleTargetAStar(new Point(this.x, this.y), this.chunk, this.gene, this.chunk.size, this.energyVal, creature);
            route = aStar.getResult();

            if (route != null) {
                goal = Goal.Mate;
                this.color = Color.Pink;
                this.mateTarget = creature;
                return true;
            }
            return false;
        }

        private double passiveVal(Hexagon hex) {
            double hunger = (gene.Size - energyVal) * gene.HungerBias;

            return hex.Passive(hunger, gene.DistanceBias);
        }

        public void activeSearch() { 
            if (matingSearch())
                return;
            //check if it's possible to mate first

            this.getActiveRoute();
        }

        protected virtual void getActiveRoute() {
            AStar aStar = new AStar(new Point(this.x, this.y), this.chunk, this.gene, this.chunk.size, this.energyVal);
            route = aStar.getResult();

            if (route != null)
            {
                myFood = aStar.getTarget();
                if (myFood != null)
                {
                    goal = Goal.Food;
                    this.color = Color.FromArgb(150, 50, 50);
                }
                else
                {
                    goal = Goal.Nothing;
                }


                if (route.quality < gene.ActivePreference)
                {
                    this.color = Color.FromArgb(50, 50, 50);
                    goal = Goal.Nothing;
                }

            }
            else
            {
                sleep = 20;
                this.color = Color.Purple;
            }
        
        }

        public void checkSurroundings() {
            //check if target plant has been eaten

            //check if predators are near

            //check if there's a new possible route (if it doesnt have one yet)
        }


        public void move(double dt) { //needs a rework cus the target wont be an entity

            if (this.chunk.heightOfTile < Hexagon.seaLevel)
                this.energyVal -= Calculator.EnergyPerTic(gene) * gene.SwimCost * dt;
            
            else this.energyVal -= Calculator.EnergyPerTic(gene) * gene.WalkCost * dt;
            
            if (route != null) {
                if (goal == Goal.Mate) {
                    if (mateTarget.goal != Goal.Mate) {
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

                Point currentLoc;
                if (route.move((float)dt * gene.Velocity)) {
                    currentLoc = route.getPos();
                    x = currentLoc.X;
                    y = currentLoc.Y;
                    if (route.isDone()) {
                        route = null; //put any functions wich are to activate when the route is done here
                        if (goal == Goal.Food)
                            eat(myFood);
                        if (goal == Goal.Mate)
                            MateWithFemale(mateTarget);
                        myFood = null;
                        sleep += 30;
                        return;
                    }
                    this.chunk.moveEntity(this, route.getDir());
                    energyVal -= Calculator.JumpCost(this.gene);
                    return;
                }
                currentLoc = route.getPos();
                x = currentLoc.X;
                y = currentLoc.Y;
                return;
            }
            //do whatever and entity should do when it doesnt have a route
        }
        private void eat(Grass grass)
        {
            if (grass == null)
                return;
            grass.visible = false;
            int value = grass.getVal(this.chunk.vegetation.currentTime);
            int MaxValue = Settings.GrassMaxFeed;
            if (MaxValue < value)
            {
                if (energyVal + MaxValue > maxEnergy)
                    this.energyVal = maxEnergy;
                else this.energyVal += MaxValue;
            }
            else
            {
                if (energyVal + grass.getVal(this.chunk.vegetation.currentTime) > maxEnergy)
                    this.energyVal = maxEnergy;
                else this.energyVal += grass.getVal(this.chunk.vegetation.currentTime);
            }
        }
        public static void Update()
        {
            MateWeight = Settings.MatingCost;
        }
    }
}
