using System.Collections.Generic;

using IntroProject.Core.Math;

namespace IntroProject
{
    //basicly a list of points wich you can ask the length and position of
    class Curve
    {
        private List<Point2D> points;
        private double length;
        private bool reversed;
        public Curve(List<Point2D> points, double length, bool reversed) {
            this.points = points;
            this.length = length;
            this.reversed = reversed;
        }

        public double Length { get { return length; } }
        public Point2D Go(double place) { //the percentage this "place" is of the length is rounded towards one of the points
            int num = (int)((points.Count * (place / length)) + 0.5f);
            if (num >= points.Count)
                num = points.Count - 1;
            if (reversed)
                num = points.Count - num - 1;
            return points[num];
        }
    }
}
