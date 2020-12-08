using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace IntroProject
{
    class AStar
    { //this is where every entity saves it's jumpheight and speed etc you 
        private RouteList routeList;
        public static Route search(Point loc, Hexagon chunck, Gene gene) {
            //start with the few base routes
            return null;
            //while you arent at the end point
            //pop the lowest cost route from the list
            //expand it
            //mark the newly reached places as reached
            //put the new routes in the list
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
        public RouteElement(float cost) {
            this.cost = cost;
        }
        public RouteElement(float cost, RouteElement next) {
            this.cost = cost;
            this.next = next;
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
