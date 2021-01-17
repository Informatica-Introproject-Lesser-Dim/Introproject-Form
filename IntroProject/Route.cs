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

        public Hexagon endHex { get { return EndHex; } set { if (value.heightOfTile < Hexagon.seaLevel) amountWaterTiles++; EndHex = value; } }
        
        public float jumpCount { get { return distances.Count; } }

        public float Length { get { if (distances.Count == 0) return 0; return distances[distances.Count - 1]; } }

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

        public void addDirection(int n) //breaks if you enter an invalid direction or go off the map
        { 
            endHex = endHex[n]; //neighbor of the last hexagon becomes the new last
            lastDir = n;

            //if it's the first direction you need to calculate how to move from your start point to the edge
            if (points.Count == 0)
            { 
                points.Add(n);
                Point2D go = Hexagon.CalcSide(size, n);
                double dist = Trigonometry.Distance(go, start);
                distances.Add((float)dist);
                return;
            }

            //not the first direction so just using one of the preset curves
            int entrance = points[points.Count - 1];
            points.Add(n);
            Curve curve = Path.getCurve((entrance + 3) % 6, n);
            distances.Add((float) curve.Length);
        }

        public Route(Route clonable) //clones everything except the position on the route and the end position
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

        public Route addAndClone(int dir) //to be used for when you need to add multiple different possible routes to one route
        { 
            Route result = new Route(this);
            result.addDirection(dir);
            return result;
        }

        public void addEnd(Point2D end)
        {
            float dist;
            if (points.Count == 0) //if it's in the same hexagon as the start
                dist = (float)Trigonometry.Distance(end, start);
            else
            {
                //otherwise calculate the distance from one of the sides
                Point2D temp = Hexagon.CalcSide(size, (points[points.Count - 1] + 3) % 6);
                dist = (float)Trigonometry.Distance(temp, end);
            }
            distances.Add(dist);
            this.end = end;
        }

        //how to use a route class: call move every time you want to move, call getPos to get the position and if move returns true
        //then you call isDone, if it isnt done then you continue the loop
        public bool move(float amount)
        { 
            if (hex != distances.Count)
            {
                pos += amount;
                if (pos < distances[hex])
                    return false;
                pos -= distances[hex];
                hex++;
            }

            return true;
        }

        public int getDir() => points[hex - 1];

        public Point2D GetPos() {
            if (hex == distances.Count)
                return end; //already done with the route
            if (hex == 0)
            {//start of the route
                Point2D delta;
                if (distances.Count == 1)
                    delta = end - start;
                else
                    delta = Hexagon.CalcSide(size, points[0]) - start;

                double scale = pos / distances[0];
                return start + delta * scale;
            }

            //last part of the route
            if (hex == distances.Count - 1)
            {//last part of the route
                Point2D temp = Hexagon.CalcSide(size, (points[hex - 1] + 3) % 6);
                Point2D delta = end - temp;
                double scale = pos / distances[distances.Count - 1];
                return temp + delta * scale;
            }
            //otherwise: just use one of the preset curves
            Curve curve = Path.getCurve((points[hex - 1] + 3)%6, points[hex]);
            return curve.Go(pos);
        }

        public bool isDone() => hex == distances.Count;
    }
}
