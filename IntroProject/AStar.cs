using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace IntroProject
{
    class AStar
    { //this is where every entity saves it's jumpheight and speed etc 
        private RouteList routeList;
        private Route result;
        protected Route current;
        private static int Tag = 0; //every time you check a hexagon: give it a tag so that if you enter it again you'll know it's already been used in a route
        private Grass goal;
        protected Gene gene;
        private double maxCost;
        private double energy;
        private Route best;

        //when you initialize an AStar object it starts calculating the best route and then you're able to ask for the Route
        public AStar(Point loc, Hexagon chunck, Gene gene, int size, double energy) {
            Tag++;
            this.energy = energy;
            maxCost = Math.Pow(10,3); //default value for now

            this.gene = gene;
            //add the starting point
            mark(chunck);
            Route temp = new Route(loc, size, chunck);
            routeList = new RouteList();
            routeList.Add(new RouteElement(0, temp));

            //start with the few base routes
            Route current;
            while ((current = routeList.Pop()) != null) { //add a test wether hte current route is null aka no route has been found
                if (isDone(current))
                    break;
                if (best == null)
                    best = current;
                else if (current.quality > best.quality)//save it if it's better than the best route
                    best = current;
                expandPoint(current);
            }
            //now put the end point into current and have it as a result
            if (current == null) {
                if (best == null)
                    return;
                current = best;
            }
            this.current = current.Clone();
            Point end = new Point(0,0);
            if (current.endHex.vegetation.FoodLocations().Count > 0) {
                goal = current.endHex.bestFood(Hexagon.calcSide(size, (current.lastDir + 3)%6));
                end = goal.loc;
            }
            current.addEnd(end);
            result = current;
        }
        public virtual Route getResult() {
            return result;
        }

        public Grass getTarget() {
            return goal;
        }

        private void addDir(Route r, int dir) {
            //test if you can even go there etc and then add it to the route list
            Hexagon goal = r.endHex[dir];
            

            if (goal == null)
                return;
            if (goal.heightOfTile - r.endHex.heightOfTile > gene.JumpHeight) //if the jumpheight is too high
                return;
            if (goal.Tag == Tag)
                return;
            goal.Tag = Tag;//tag it so we dont use it again
            if (goal.heightOfTile < Hexagon.seaLevel) //this needs to be changed if we want to add swimming
                return;
            Route result = r.addAndClone(dir);
            float cost = calcCost(result);
            if (cost > maxCost)
                return;
            routeList.Add(new RouteElement(cost, result));
        }

        protected virtual float calcCost(Route r) { //lowest cost = best route
            //distance squared to the closest bit of food
            double quality = calcQuality(r);

            //Creature.calcDistance2(EntityType.Plant, r.endHex, new Point(r.endHex.x, r.endHex.y));

            //current cost is only based on energy cost for now, will need more things such as fear later on
            float current = r.Length*Calculator.EnergyPerMeter(gene.Velocity) + r.jumpCount*Calculator.JumpCost(gene.JumpHeight);

            //later on we also need to add a "reward" amount so that the entity targets the best bit of food/a partner to procreate with
            if (current > maxCost)
                return current;
            return current - (float)quality; //note that expected distance is still squared at this point
        }

        private void expandPoint(Route r) {
            //call addDir for every direction except the one yo came from
            int not = (r.lastDir + 3) % 6;
            if (r.lastDir == -1)
                not = -1;
            for (int i = 0; i < 6; i++)
                if (i != not)
                    addDir(r, i);
        }

        protected virtual bool isDone(Route r) {
            double val = r.quality;
            if (val < gene.ActivePreference)
                return false;
            if (r.endHex.vegetation.FoodLocations().Count < 1)
                return false;
            return true;
        }

        private double calcQuality(Route r) {
            double val = r.endHex.active(gene.ActiveBias, (gene.Size - energy) * gene.HungerBias);
            r.quality = val;
            return val;
        }

        private void mark(Hexagon hex) {
            hex.Tag = Tag;
        }
    }

    class SingleTargetAStar : AStar  //the same mechanism except you're aiming towards a specific target now
    {
        private Creature theTarget;
        public SingleTargetAStar(Point loc, Hexagon chunck, Gene gene, int size, double energy, Creature theTarget) : base(loc,chunck,gene,size,energy){
            this.theTarget = theTarget;
        }

        public Creature GetCreature() 
        {
            return theTarget; 
        }

        public override Route getResult()
        {
            //test of het result wel naar het einde gaat
            if (!isDone(current))
                return null;
            this.current.addEnd(new Point(theTarget.x, theTarget.y));

            return this.current;
        }

        protected override float calcCost(Route r)
        {
            float current = r.Length * Calculator.EnergyPerMeter(gene.Velocity) + r.jumpCount * Calculator.JumpCost(gene.JumpHeight);
            float dx = theTarget.x - r.endHex.x;
            float dy = theTarget.y - r.endHex.y;
            double expected = Calculator.EnergyPerMeter(gene.Velocity) * Math.Sqrt(dx * dx + dy * dy);
            return (float)expected + current;
        }

        protected override bool isDone(Route r)
        {
            return r.endHex == theTarget.chunk; //done when you land in  the same chunk
        }
    }


    //How to use: every time you want to add a route to the list you must initialize a routeElement and add the cost
    class RouteList
    {
        public int Length = 0;
        private RouteElement first;
        public RouteList() {
        
        }

        public void Add(RouteElement n) {
            Length++;
            if (first == null) { first = n; return; } //become the first element if there are no elements
            if (first.cost < n.cost) { first.Add(n); return; } //if you have a higher cost then you get passed down the list
            n.next = first; //replacing the first element
            first = n;

        }

        public Route Pop() { //pop the best route from the list
            if (first == null)
                return null;
            Length--;
            Route result = first.route;
            first = first.next;
            return result;
        }
    }

    class RouteElement {

        public RouteElement next;
        public Route route;
        public float cost;
        public RouteElement(float cost, Route r) {
            this.cost = cost;
            route = r;
        }
        public RouteElement(float cost, Route r, RouteElement next) {
            this.cost = cost;
            this.next = next;
            route = r;
        }

        public void Add(RouteElement n)
        { //options: append, continue or insert
            if (next == null) { next = n; return; } 
            if (next.cost < n.cost) { next.Add(n); return; }
            n.next = next;
            this.next = n;
        }
    }
}
