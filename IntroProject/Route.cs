﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntroProject
{
    public class Route
    {
        List<int> points; //all the different exits from the hexagons
        Point start;
        Point end;
        List<float> distances; //length of the path untill this point
        int hex; //number of current hexagon in the list
        float pos; //position within current hex
        int size; //hex size

        public Route(Point start, int size) {
            this.start = start;
            pos = 0;
            hex = 0;
            this.size = size;
        }

        public void addDirection(int n) {
            if (points == null) {
                points.Add(n);
                Point go = Hexagon.calcSide(size, n);
                int dx = go.X - start.X;
                int dy = go.Y - start.Y;
                float dist = (float) Math.Sqrt(dx * dx + dy * dy);
                distances.Add(dist);
                return;
            }
            int entrance = points[points.Count - 1];
            points.Add(n);
            Curve curve = Path.getCurve((entrance + 3) % 6, n);
            distances.Add((float) curve.Length);
        }

        public void addEnd(Point end) {
            float dist;
            int dx, dy;
            if (points == null) {
                dx = end.X - start.X;
                dy = end.Y - start.Y;
                dist = (float)Math.Sqrt(dx * dx + dy * dy);
                distances.Add(dist);
                this.end = end;
                return;
            }
            Point temp = Hexagon.calcSide(size, (points[points.Count - 1] + 3) % 6);
            dx = temp.X - end.X;
            dy = temp.Y - end.Y;
            dist = (float)Math.Sqrt(dx * dx + dy * dy);
            distances.Add(dist);
            this.end = end;
        }

        //how to use a route class: call move every time you want to move, call getPos to get the position and if move returns true
        //then you call isDone, if it isnt then you continue the loop
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
            if (hex == 0)
            {
                int dx, dy;
                if (distances.Count == 1) {
                    dx = end.X - start.X;
                    dy = end.Y - start.Y;
                    return new Point((int) (start.X + (dx * pos) / distances[0]), (int) (start.Y + (dy * pos) / distances[0]) );
                }
                Point temp = Hexagon.calcSide(size, points[0]);
                dx = temp.X - start.X;
                dy = temp.Y - start.Y;
                return new Point((int)(start.X + (dx * pos) / distances[0]), (int)(start.Y + (dy * pos) / distances[0]));

            }
            if (hex == distances.Count - 1) {
                Point temp = Hexagon.calcSide(size, (points[hex - 1] + 3) % 6);
                int dx = temp.X - start.X;
                int dy = temp.Y - start.Y;
                return new Point((int)(start.X + (dx * pos) / distances[0]), (int)(start.Y + (dy * pos) / distances[0]));

            }
            if (hex == distances.Count) {
                return end;
            }
            Curve curve = Path.getCurve(points[hex - 1], points[hex]);
            return curve.go(pos);
        }

        public bool isDone() {
            return hex == distances.Count;
        }

    }
}