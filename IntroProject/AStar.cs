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
            //changing the Tag so that it's different each time you create an AStar object
            Tag++;

            //setting the normal values
            this.energy = energy;
            this.gene = gene;
            maxCost = energy; 

            //add the starting point
            mark(chunck);
            Route temp = new Route(loc, size, chunck);
            routeList = new RouteList();
            expandPoint(temp);


            Route current;
            //keep increasing all the routes
            //untill you're out of routes => current route == null
            while ((current = routeList.Pop()) != null)
            { 
                //stop searching through the routes once you find one that reaches the end
                if (isDone(current))
                    break;

                //save the best route (in case we dont find any that reach a satisfactory end)
                if (best == null)
                    best = current;
                else if (current.quality > best.quality)
                    best = current;

                //adding new routes that expand from the current one
                expandPoint(current);
            }

            //if you ran out of routes
            if (current == null)
            {
                //too bad if there also wasnt a best route
                if (best == null)
                    return;

                //otherwise make the best route our final choice
                current = best;
            }
            this.current = current.Clone();
            
            //add the end point to our best route
            result = addEnd(current, size);
        }

        public virtual Route addEnd(Route r, int size) 
            //vitual addEnd method because it's different for each version of AStar
        {
            //if there is food in the final hexagon
            if (r.endHex.FoodValue() > 0)
            {
                //set your end location to the location of the food
                goal = r.endHex.bestFood(Hexagon.CalcSide(size, (r.lastDir + 3) % 6));
                Point2D end = goal;
                current.addEnd(end);
            }
            return current;
        }

        public virtual Route getResult() => result;

        public Grass getTarget() => goal;

        private void addDir(Route r, int dir)
            //add a clone of this route that is increased in this direction
            //to our list of all routes
        {
            //get the next hexagon
            Hexagon goal = r.endHex[dir];
            
            //if either: the tile doesnt exist
            //or the jump is too high
            //or you've already been on this tile (already set the tag once)
            //then dont go to this hexagon
            if (goal == null || goal.heightOfTile - r.endHex.heightOfTile > gene.JumpHeight || goal.Tag == Tag)
                return;

            //give the hexagon a tag so we dont enter it a second time
            goal.Tag = Tag;

            //adding the direction to the route and then calculate a cost for it
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
            //a route is done if it's quality exceeds your preference
            //and there must be food in the hexagon
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

    class SingleTargetAStar : AStar  
        //the same mechanism except you're aiming towards a specific target now
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
            //only return it if the result actually goes towards the chunck of our target
            if (current == null)
                return null;
            if (!isDone(current))
                return null;
            
            //add the targetlocation to the route
            current.addEnd(theTarget.ChunckRelLoc);
            return current;
        }

        protected override float calcCost(Route r)
        {
            //cost is now decided on the current route cost + the distance to our target 
            float current = r.Length * Calculator.EnergyPerMeter(gene) + r.jumpCount * Calculator.JumpCost(gene);
            double expected = Calculator.EnergyPerMeter(gene) * Trigonometry.Distance(theTarget.GlobalLoc, r.endHex);
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
            //add the location of a deathpile if there is any in this chunck
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
            //just normal initialisation
        {

        }

        protected override double calcQuality(Route r) 
            //calculates the quality and also saves it in the route
        {   
            double val = r.endHex.CarnivoreActive(gene.ActiveBias, ((CarnivoreGene)gene).LivingTargetBias);
            r.quality = val;
            return val;
        }

        protected override bool isDone(Route r) 
        {
            //ís done if you're in the same chunck as a deathpile
            if (r.endHex.getByType(EntityType.Plant).Count > 0)
                return true;

            //is also done if you're 3 tiles from a herbivore
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
