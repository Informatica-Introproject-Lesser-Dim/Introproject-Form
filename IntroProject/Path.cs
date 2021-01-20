using System;
using System.Collections.Generic;

using IntroProject.Core.Math;

namespace IntroProject
{
    static class Path
    {
        private static Curve[] ones, twos, threes; //the paths that have one, two or 3 difference(s)

        public static Curve getCurve(int start, int end) //start is the side you enter the hexagon from end is the side you exit from
        {
            if (start == end || start < 0 || start > 5 || end < 0 || end > 5)
                return null;

            int dx = start - end;

            switch (dx)
            {
                case 1:
                case -5:
                    return ones[end + 6];
                case -1:
                case 5:
                    return ones[start];
                case 2:
                case -4:
                    return twos[end + 6];
                case -2:
                case 4:
                    return twos[start];
                case 3:
                    return threes[end + 3];
                case -3:
                    return threes[start];
                default: 
                    return null;
            }
        }

        public static void initializePaths(int size)
        {
            ones = new Curve[12];
            twos = new Curve[12];
            threes = new Curve[6];

            //for each curve in "ones" calculate the appropriate start and end of the curve
            for (int i = 0; i < 6; i++)
            {
                Curve[] temp = calcCurve(size / 2, (int) (size * Math.Cos(Math.PI * (-1.0 / 3 + i / 3.0))), (int) (size *Math.Sin(Math.PI * (-1.0 / 3 + i / 3.0))), Math.PI * (1.0/3 + i/3.0), Math.PI * 2.0 / 3);
                ones[i] = temp[0];
                ones[i + 6] = temp[1];
            }
            for (int i = 0; i < 6; i++) {//for each curve in "twos" calculate the appropriate start and end of the the curve
                Curve[] temp = calcCurve(size * 3.0 / 2, (int)(size * Hexagon.sqrt3 * Math.Cos(Math.PI * (-1.0 / 6 + i / 3.0))), (int)(size * Hexagon.sqrt3 * Math.Sin(Math.PI * (-1.0 / 6 + i / 3.0))), Math.PI*(2.0/3 + i/3.0),Math.PI/3);
                twos[i] = temp[0];
                twos[i + 6] = temp[1];
            }
            for (int i = 0; i < 6; i++)
            {
                threes[i] = calcStraight((int)(size * Hexagon.sqrt3 / 2 * Math.Cos(Math.PI * (0.5 + i / 3.0))), 
                                         (int)(size * Hexagon.sqrt3 / 2 * Math.Sin(Math.PI * (0.5 + i / 3.0))),
                                         (int)(size * Hexagon.sqrt3 / 2 * Math.Cos(Math.PI * (-0.5 + i / 3.0))),
                                         (int)(size * Hexagon.sqrt3 / 2 * Math.Sin(Math.PI * (-0.5 + i / 3.0))));
            }
        }

        private static Curve[] calcCurve(double r, int x, int y, double start, double turn) //start and the amount it turns are in radians
        { 
            double length = r * turn; //just plain calculating circumfrence of a circle
            List<Point2D> points = new List<Point2D>();

            //calculate all the points in this curve
            for (double i = length; i >= 0; i -= 0.5)
            { 
                int xPos = x + (int)(r * Math.Cos(start + i / r));
                int yPos = y + (int)(r * Math.Sin(start + i / r));
                points.Add(new Point2D().SetPosition(xPos, yPos));
            }

            return new Curve[2] { new Curve(points, length, false), new Curve(points,length, true) };
        }

        private static Curve calcStraight(int x1, int y1, int x2, int y2)
        {
            int dx = x1 - x2; //xdistance
            int dy = y1 - y2; //ydistance
            double length = Math.Sqrt(dx * dx + dy * dy); //total distance
            double sx = dx / length;//step size
            double sy = dy / length;

            List<Point2D> points = new List<Point2D>();

            for (double i = 0; i <= length; i += 0.5) //move along the line with the correct step size
                points.Add(new Point2D().SetPosition((int)(x2 + sx * i), (int)(y2 + sy * i)));

            return new Curve(points, length, false); 
        }
    }
}
