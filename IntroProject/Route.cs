using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

using IntroProject.Core.Math;

namespace IntroProject
{
    public class Route
    {
        List<int> points; //all the different exits from the hexagons
        Point2D start = new Point2D();
        Point2D end = new Point2D();
        List<float> distances; //length of the path untill this point
        public int amountWaterTiles = 0;
        public float Length { get { if (distances.Count == 0) return 0; return distances[distances.Count - 1]; } }
        int hex; //number of current hexagon in the list
        float pos; //position within current hex
        int size; //hex size
        private Hexagon EndHex;
        public Hexagon endHex { get { return EndHex; } set { if (value.heightOfTile < Hexagon.seaLevel) amountWaterTiles++; EndHex = value; } }
        public int lastDir = -1;
        public double quality = 0;
        public float jumpCount { get { return distances.Count; } }


        public Route(Point start, int size, Hexagon startHex) {
            this.start.SetPosition(start.X, start.Y);
            pos = 0;
            hex = 0;
            this.size = size;
            endHex = startHex;
            points = new List<int>();
            distances = new List<float>();
        }

        public void addDirection(int n) { //breaks if you enter in invalid direction or go off the map

            endHex = endHex[n]; //neighbor of the last hexagon becomes the new last
            lastDir = n;
            if (points.Count == 0) { //if it's the first direction you need to calculate how to move from your start point to the edge
                points.Add(n);
                Point go = Hexagon.calcSide(size, n);
                double dist = Trigonometry.Distance((go.X, go.Y), (start.X, start.Y));
                distances.Add((float)dist);
                return;
            }
            //not the first direction so just using one of the preset curves
            int entrance = points[points.Count - 1];
            points.Add(n);
            Curve curve = Path.getCurve((entrance + 3) % 6, n);
            distances.Add((float) curve.Length);
        }

        public Route(Route clonable) { //clones everything except the position on the route and the end position
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

        public Route Clone()
        {
            return new Route(this);
        }

        public Route addAndClone(int dir) { //to be used for when you need to add multiple different possible routes to one route
            Route result = new Route(this);
            result.addDirection(dir);
            return result;
        }

        public void addEnd(Point endP) {
            Point2D end = new Point2D().SetPosition(endP.X, endP.Y);

            float dist;
            Point2D delta;
            if (points.Count == 0) { //if it's in the same hexagon as the start
                dist = (float)Trigonometry.Distance(end, start);
            }
            else
            {
                //otherwise calculate the distance from one of the sides
                Point temp = Hexagon.calcSide(size, (points[points.Count - 1] + 3) % 6);
                dist = (float)Trigonometry.Distance((temp.X, temp.Y), (end.X, end.Y));
            }
            distances.Add(dist);
            this.end = end;
        }

        //how to use a route class: call move every time you want to move, call getPos to get the position and if move returns true
        //then you call isDone, if it isnt done then you continue the loop
        public bool move(float amount) { 
            if (hex == distances.Count)
                return true;
            pos += amount;
            if (pos < distances[hex])
                return false;
            pos -= distances[hex];
            hex++;
            return true;
        }

        public int getDir() {
            return points[hex - 1];
        }

        public Point getPos() {
            Func<Point2D, Point> toPoint = (Point2D p) => new Point((int)p.X, (int)p.Y);
            if (hex == distances.Count)
                return toPoint(end); //already done with the route
            if (hex == 0)
            {//start of the route
                Point delta;
                if (distances.Count == 1)
                {
                    delta = toPoint(end) - (Size)toPoint(start);
                }
                else
                {
                    Point temp = Hexagon.calcSide(size, points[0]);
                    delta = temp - (Size)toPoint(start);
                }
                double scale = pos / distances[0];
                return toPoint(start) + new Size((int)(delta.X * scale), (int)(delta.Y * scale));
            }
            if (hex == distances.Count - 1)
            {//last part of the route
                Point temp = Hexagon.calcSide(size, (points[hex - 1] + 3) % 6);
                Point delta = toPoint(end) - (Size)temp;
                double scale = pos / distances[distances.Count - 1];
                return temp + new Size((int)(delta.X * scale), (int)(delta.Y * scale));
            }
            //otherwise: just use one of the preset curves
            Curve curve = Path.getCurve((points[hex - 1] + 3)%6, points[hex]);
            return curve.go(pos);
        }

        public bool isDone() {
            return hex == distances.Count;
        }

    }
}
