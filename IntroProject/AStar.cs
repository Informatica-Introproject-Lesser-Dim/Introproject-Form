using System;
using System.Collections.Generic;
using System.Drawing;

using IntroProject.Core.Math;

namespace IntroProject
{
    class AStar //this is where every entity saves it's jumpheight and speed etc 
    { 
        private RouteList routeList;
        private Route result;
        private Route best;
        private Grass goal;
        protected Gene gene;
        protected Route current;
        private static int Tag = 0; //every time you check a hexagon: give it a tag so that if you enter it again you'll know it's already been used in a route
        private double maxCost, energy;
        private int maxLength = 200;

        //when you initialize an AStar object it starts calculating the best route and then you're able to ask for the Route
        public AStar(Point2D loc, Hexagon chunck, Gene gene, int size, double energy) =>
            this.InitializeEverything(loc, chunck, gene, size, energy);

        protected AStar() { }//just a default constructor that should not normally be used

        protected void InitializeEverything(Point2D loc, Hexagon chunck, Gene gene, int size, double energy)
        {
            Tag++;
            this.energy = energy;
            this.gene = gene;
            maxCost = energy; //default value for now

            //add the starting point
            mark(chunck);
            Route temp = new Route(loc, size, chunck);
            routeList = new RouteList();
            expandPoint(temp);

            //start with the few base routes
            Route current;
            while ((current = routeList.Pop()) != null) //add a test wether the current route is null aka no route has been found
            { 
                if (isDone(current))
                    break;
                if (best == null)
                    best = current;
                else if (current.quality > best.quality)//save it if it's better than the best route
                    best = current;
                expandPoint(current);
            }

            //now put the end point into current and have it as a result
            if (current == null)
            {
                if (best == null)
                    return;
                current = best;
            }
            this.current = current.Clone();
            

            result = addEnd(current, size);
        }

        public virtual Route addEnd(Route r, int size) 
        {
            if (r.endHex.FoodValue() > 0)
            {
                goal = r.endHex.bestFood(Hexagon.CalcSide(size, (r.lastDir + 3) % 6));
                Point2D end = goal;
                current.addEnd(end);
            }
            return current;
        }

        public virtual Route getResult() => result;

        public Grass getTarget() => goal;

        private void addDir(Route r, int dir)
        {
            //test if you can even go there etc and then add it to the route list
            Hexagon goal = r.endHex[dir];
            
            if (goal == null || goal.heightOfTile - r.endHex.heightOfTile > gene.JumpHeight || goal.Tag == Tag)
                return;
            goal.Tag = Tag;//tag it so we dont use it again
            Route result = r.addAndClone(dir);
            float cost = calcCost(result);
            if (cost > maxCost || r.Length > maxLength)
                return;
            routeList.Add(new RouteElement(cost, result));
        }

        protected virtual float calcCost(Route r) //lowest cost route = best route
        { 
            //distance squared to the closest bit of food
            double quality = calcQuality(r);

            //Creature.calcDistancePow2(EntityType.Plant, r.endHex, new Point(r.endHex.x, r.endHex.y));

            //current cost is only based on energy cost for now, will need more things such as fear later on
            float current = r.Length*Calculator.EnergyPerMeter(gene) + r.jumpCount*Calculator.JumpCost(gene) + r.amountWaterTiles*30;

            //later on we also need to add a "reward" amount so that the entity targets the best bit of food/a partner to procreate with
            if (current > maxCost)
                return current;
            return current - (float)quality; //note that expected distance is still squared at this point
        }

        private void expandPoint(Route r)
        {
            //call addDir for every direction except the one you came from
            int not = (r.lastDir + 3) % 6;
            if (r.lastDir == -1)
                not = -1;
            for (int i = 0; i < 6; i++)
                if (i != not)
                    addDir(r, i);
        }

        protected virtual bool isDone(Route r)
        {
            double val = r.quality;
            if (val < gene.ActivePreference)
                return false;
            if (r.endHex.vegetation.FoodLocations().Count < 1)
                return false;
            return true;
        }

        protected virtual double calcQuality(Route r)
        {
            double val = r.endHex.active(gene.ActiveBias, (gene.Size - energy) * gene.HungerBias);
            r.quality = val;
            return val;
        }

        private void mark(Hexagon hex) => hex.Tag = Tag;
    }

    class SingleTargetAStar : AStar  //the same mechanism except you're aiming towards a specific target now
    {
        private Entity theTarget;

        public SingleTargetAStar(Point2D loc, Hexagon chunck, Gene gene, int size, double energy, Entity theTarget) : base()
        {
            this.theTarget = theTarget;
            InitializeEverything(loc, chunck, gene, size, energy*3);
        }

        public override Route addEnd(Route r, int size) {
            return r;
        }

        public Entity GetCreature() => theTarget;

        public override Route getResult()
        {
            //test of het result wel naar het einde gaat
            if (current == null)
                return null;
            if (!isDone(current))
                return null;
            
            current.addEnd(theTarget.ChunckRelLoc);
            return current;
        }

        protected override float calcCost(Route r)
        {
            float current = r.Length * Calculator.EnergyPerMeter(gene) + r.jumpCount * Calculator.JumpCost(gene);
            double expected = Calculator.EnergyPerMeter(gene) * Trigonometry.Distance(theTarget, r.endHex);
            return (float)expected + current;
        }

        protected override bool isDone(Route r) =>
            r.endHex == theTarget.chunk; //done when you land in  the same chunk
    }

    class CarnivoreAStar : AStar
    {
        private DeathPile theTarget = null;


        public override Route addEnd(Route r, int size)
        {
            List<Entity> entities = current.endHex.getByType(EntityType.Plant);

            if (entities.Count > 0)
            {
                theTarget = (DeathPile)entities[0];
                Point2D end = theTarget;
                current.addEnd(end);
            }
            return current;
        }

        public CarnivoreAStar(Point2D loc, Hexagon chunck, Gene gene, int size, double energy) : base(loc,chunck,gene,size,energy)
        {

        }

        protected override double calcQuality(Route r) //calculates the quality and also saves it in the route
        {   
            double val = r.endHex.CarnivoreActive(gene.ActiveBias, ((CarnivoreGene)gene).LivingTargetBias);
            r.quality = val;
            return val;
        }

        protected override bool isDone(Route r) //is done when you're in the chunck of a deathpile, or when you're 3 chunks from a creature
        {
            if (r.endHex.getByType(EntityType.Plant).Count > 0)
                return true;
            for (int i = 0; i < 3; i++)
                if (r.endHex.searchPoint(i, EntityType.Herbivore).Count > 0)
                    return true;
            return false;
        }

        public override Route getResult()
        {
            //test of het result wel naar het einde gaat
            if (current == null)
                return null;


            return current;
        }

        public DeathPile getFood() 
        {
            return (DeathPile) theTarget;
            //it is extremely rare for there to be multiple deathpiles in one chunck, therefore the first one is chosen
        }
    }


    //How to use: every time you want to add a route to the list you must initialize a routeElement and add the cost
    class RouteList
    {
        public int Length = 0;
        private RouteElement first;

        public RouteList(){ }

        public void Add(RouteElement n)
        {
            Length++;
            if (first == null) { first = n; return; } //become the first element if there are no elements
            if (first.cost < n.cost) { first.Add(n); return; } //if you have a higher cost then you get passed down the list
            n.next = first; //replacing the first element
            first = n;
        }

        public Route Pop() //pop the best route from the list
        { 
            if (first == null)
                return null;
            Length--;
            Route result = first.route;
            first = first.next;
            return result;
        }
    }

    class RouteElement
    {
        public RouteElement next;
        public Route route;
        public float cost;

        public RouteElement(float cost, Route r)
        {
            this.cost = cost;
            route = r;
        }

        public RouteElement(float cost, Route r, RouteElement next)
        {
            this.cost = cost;
            this.next = next;
            route = r;
        }

        public void Add(RouteElement n) //options: append, continue or insert
        { 
            if (next == null) { next = n; return; } 
            if (next.cost < n.cost) { next.Add(n); return; }
            n.next = next;
            next = n;
        }
    }
}
