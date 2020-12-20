﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace IntroProject
{
    public class Route
    {
        List<int> points; //all the different exits from the hexagons
        Point start;
        Point end;
        List<float> distances; //length of the path untill this point
        public float Length { get { if (distances.Count == 0) return 0; return distances[distances.Count - 1]; } }
        int hex; //number of current hexagon in the list
        float pos; //position within current hex
        int size; //hex size
        public Hexagon endHex;
        public int lastDir = -1;
        public float jumpCount { get { return distances.Count; } }


        public Route(Point start, int size, Hexagon startHex) {
            this.start = start;
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
                int dx = go.X - start.X;
                int dy = go.Y - start.Y;
                float dist = (float) Math.Sqrt(dx * dx + dy * dy);
                distances.Add(dist);
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

        public void addEnd(Point end) {
            float dist;
            Point delta;
            if (points.Count == 0) { //if it's in the same hexagon as the start
                delta = end - (Size)start;
                dist = new Vector2(delta.X, delta.Y).Length();
                distances.Add(dist);
                this.end = end;
                return;
            }
            //otherwise calculate the distance from one of the sides
            Point temp = Hexagon.calcSide(size, (points[points.Count - 1] + 3) % 6);
            delta = temp - (Size)end;
            dist = new Vector2(delta.X, delta.Y).Length();
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
            if (hex == distances.Count)
                return end; //already done with the route
            if (hex == distances.Count - 1)
            {//last part of the route
                Point temp = Hexagon.calcSide(size, (points[hex - 1] + 3) % 6);
                Point delta = end - (Size)temp;
                double scale = pos / distances[0];
                return temp + new Size((int)(delta.X * scale), (int)(delta.Y * scale));
            }
            if (hex == 0)
            {//start of the route
                Point delta;
                if (distances.Count == 1)
                {
                    delta = end + (Size)start;
                } else {
                    Point temp = Hexagon.calcSide(size, points[0]);
                    delta = temp + (Size)start;
                }
                double scale = pos / distances[0];
                return start + new Size((int)(delta.X * scale), (int)(delta.Y * scale));
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
