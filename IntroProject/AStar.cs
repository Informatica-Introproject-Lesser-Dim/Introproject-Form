using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace IntroProject
{
    class AStar
    { //this is where every entity saves it's jumpheight and speed etc you 
        private RouteList routeList;
        private Route result;
        private static int Tag = 0; //every time you check a hexagon: give it a tag so that if you enter it again you'll know it's already been used in a route
        private Entity target;
        //when you initialize an AStar object it starts calculating hte best route and then you're able to ask for the Route
        public AStar(Point loc, Hexagon chunck, Gene gene, int size) {
            Tag++;
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
                expandPoint(current);
            }
            //now put the end point into current and have it as a result
            if (current == null)
                return;
            Point end = new Point(-100,-100);
            foreach (Entity e in current.endHex.entities)
                if (e is Planten)
                {
                    target = e;
                    end = new Point(e.x, e.y);
                    break;
                }
                    
            current.addEnd(end);
            result = current;
        }

        public Route getResult() {
            return result;
        }

        public Entity getTarget() {
            return target;
        }

        private void addDir(Route r, int dir) {
            //test if you can even go there etc and then add it to the route list
            Hexagon goal = r.endHex[dir];
            if (goal == null)
                return;
            if (goal.Tag == Tag)
                return;
            goal.Tag = Tag; //tag it so we dont use it again
            Route result = r.addAndClone(dir);
            routeList.Add(new RouteElement(calcCost(result), result));
        }

        private float calcCost(Route r) { //lowest cost = best route
            int expected = Creature.calcDistance2(EntityType.Plant, r.endHex, new Point(r.endHex.x, r.endHex.y));
            float current = r.Length;
            return current + expected; //note that expected distance is still squared at this point
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

        private bool isDone(Route r) {
            Hexagon end = r.endHex;
            foreach (Entity e in end.entities)
                if (e is Planten)
                    return true;
            return false;
        }

        private void mark(Hexagon hex) {
            hex.Tag = Tag;
        }
    }

    //How to use: every time you want to add a route to the list you must initialize a routeElement and 
    class RouteList
    {
        public int Length = 0;
        private RouteElement first;
        public RouteList() {
        
        }

        public void Add(RouteElement n) {
            Length++;
            if (first == null) { first = n; return; }
            if (first.cost < n.cost) { first.Add(n); return; }
            n.next = first;
            first = n;

        }

        public Route Pop() {
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
