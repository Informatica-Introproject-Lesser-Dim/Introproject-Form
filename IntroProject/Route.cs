using System.Collections.Generic;
using System.Drawing;

using IntroProject.Core.Math;

namespace IntroProject
{
    public class Route
    {
        List<int> points; //all the different exits from the hexagons
        Point2D start = new Point2D(), end = new Point2D();
        List<float> distances; //length of the path untill this point
        Hexagon EndHex;

        private int hex, size; //number and size of current hexagon in the list
        private float pos; //position within current hex

        public int amountWaterTiles = 0, lastDir = -1;
        public double quality = 0;

        //the last hexagon you arrive at in the route
        public Hexagon endHex { get { return EndHex; } set { if (value.heightOfTile < Hexagon.seaLevel) amountWaterTiles++; EndHex = value; } }
        
        //just the amount of times you change hexagons
        public float jumpCount { get { return distances.Count; } }

        public float Length { get { if (distances.Count == 0) return 0; return distances[^1]; } }

        public Route(Point2D start, int size, Hexagon startHex)
        {
            this.start.SetPosition(start.X, start.Y);
            pos = 0;
            hex = 0;
            this.size = size;
            endHex = startHex;
            points = new List<int>();
            distances = new List<float>();
        }

        public void addDirection(int n) 
            //you're in a tile, choose a direction from which to exit the tile
        { 
            //setting a new final point
            endHex = endHex[n]; 
            lastDir = n;

            //if it's the first direction you need to calculate how to move from your start point to the edge
            if (points.Count == 0)
            { 
                points.Add(n);

                //Adding the correct distance to the edge
                Point2D go = Hexagon.CalcSide(size, n);
                double dist = Trigonometry.Distance(go, start);
                distances.Add((float)dist);
                return;
            }

            //not the first direction
            int entrance = points[^1];
            points.Add(n);

            //add the length of the correct curve
            Curve curve = Path.getCurve((entrance + 3) % 6, n);
            distances.Add((float) curve.Length);
        }

        public Route(Route clonable) 
            //clones everything except the position on the route and the end position
        { 
            start = clonable.start;
            endHex = clonable.endHex;
            size = clonable.size;
            points = new List<int>();
            distances = new List<float>();

            foreach (int p in clonable.points)
                points.Add(p);
            foreach (float f in clonable.distances)
                distances.Add(f);
        }

        public Route Clone() => new Route(this);

        public Route addAndClone(int dir) 
            //to be used for when you need to add multiple different possible routes to one route
        { 
            //cloning yourself and adding a direction to the clone
            Route result = new Route(this);
            result.addDirection(dir);
            return result;
        }

        public void addEnd(Point2D end)
            //Adding a final point, relative to the middle of the hexagon
        {
            float dist;
            if (points.Count == 0) //if it's in the same hexagon as the start
                dist = (float)Trigonometry.Distance(end, start);
            else
            {
                //otherwise calculate the distance from one of the sides
                Point2D temp = Hexagon.CalcSide(size, (points[^1] + 3) % 6);
                dist = (float)Trigonometry.Distance(temp, end);
            }
            distances.Add(dist);
            this.end = end;
        }

        //how to use a route class: call move every time you want to move, call getPos to get the position and if move returns true
        //then you call isDone, if it isnt done then you continue the loop
        public bool move(float amount)
        { 
            //if you're not at the end yet
            if (hex != distances.Count)
            {
                //add the amount you want to move with
                pos += amount;

                //check whether you're changing hexagon
                if (pos < distances[hex])
                    return false;
                pos -= distances[hex];
                hex++;
            }

            return true;
        }

        public int getDir() => points[hex - 1];

        public Point2D GetPos() 
            //we know the hexagon you're in and how far you've travelled in it
            //now calculate x and y position relative to the middle of the hexagon
        {

            //if you're at the end
            if (hex == distances.Count)
                return end; 

            //if you're in the first hexagon of the route
            if (hex == 0)
            {//start of the route

                //calculate the vector (delta) from the start to the second point
                Point2D delta;
                if (distances.Count == 1)
                    delta = end - start;
                else
                    delta = Hexagon.CalcSide(size, points[0]) - start;

                //scale it with the length of the vector
                double scale = pos / distances[0];

                //add it to the start of the route to get your current position
                return start + delta * scale;
            }

            //last part of the route
            if (hex == distances.Count - 1)
            {
                //calculate the vector from the edge of the hexagon to the end point
                Point2D temp = Hexagon.CalcSide(size, (points[hex - 1] + 3) % 6);
                Point2D delta = end - temp;

                //scale it with the length of the vector
                double scale = pos / distances[^1];

                //add it to the location of the edge to get the current position
                return temp + delta * scale;
            }
            
            //get the correct curve belonging to our point of entry and point of exit
            Curve curve = Path.getCurve((points[hex - 1] + 3)%6, points[hex]);

            //now we can just ask the preset cuve where our location is
            return curve.Go(pos);
        }

        public bool isDone() => hex == distances.Count;
    }
}
