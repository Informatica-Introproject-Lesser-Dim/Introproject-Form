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
    class Route
    {
        List<int> points;
        Point start;
        Point end;
        List<int> distances;
        int pos;
        int Length;
        int size;

        public Route(Point start, int size) {
            this.start = start;
            pos = 0;
            Length = 0;
        }

        public void addDirection(int n) {
            if (points == null) {
                points.Add(n);
                //find the correct edge position and calc the distance towards it   
                //return
            }
            //add one of the points normally and look up the distance
        }

    }
}
