using System.Collections.Generic;

using IntroProject.Core.Math;

namespace IntroProject
{
    //basicly a list of points wich you can ask the length and position of
    class Curve
    {
        //all the locations the curve consists of
        private List<Point2D> points;

        private double length;

        //boolean for whether you go through the curve normally or in the opposite direction
        private bool reversed;
        public Curve(List<Point2D> points, double length, bool reversed)
        {
            this.points = points;
            this.length = length;
            this.reversed = reversed;
        }

        public double Length { get { return length; } }
        public Point2D Go(double place) 
            //calculate which position you're at, depending on how far you've travelled
        { 
            //we receive a "place" between 0 and the route length
            //now we scale it to an int between 0 and our amount of locations
            int num = (int)((points.Count * (place / length)) + 0.5f);

            //if you're further than the end, receive the end
            if (num >= points.Count)
                num = points.Count - 1;

            //if you're going through the route reversed
            //get the opposite point in the list
            if (reversed)
                num = points.Count - num - 1;
            return points[num];
        }
    }
}
