using System;
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
    //basicly a list of points wich you can ask the length and position of
    class Curve
    {
        private List<Point> points;
        private double length;
        private bool reversed;
        public Curve(List<Point> points, double length, bool reversed) {
            this.points = points;
            this.length = length;
            this.reversed = reversed;
        }

        public double Length { get { return length; } }
        public Point go(double place) {
            int num = (int)((points.Count * (place / length)) + 0.5f);
            if (num >= points.Count)
                num = points.Count - 1;
            if (reversed)
                num = points.Count - num - 1;
            return points[num];
        }
    }
}
