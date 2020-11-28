using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace IntroProject
{
    class Hexagon
    {
        private int size;
        public static double sqrt3 = Math.Sqrt(3);
        public static double sqrt2 = Math.Sqrt(2);
        public int x, y;
        public Color color;

        public int width {
            get { return size * 2; }
        }
        public int height {
            get { return (int) (size * sqrt3); }
        }


        public Hexagon(int size, int c, int x, int y) //size is the length of each side
        {
            this.size = size;
            color = Color.FromArgb(c,c,c);

            this.x = x;
            this.y = y;

        }

        public void draw(Graphics g, int sx, int sy) { //center of the hexagon is at sx,sy
            Bitmap bm = new Bitmap(width + 1, height + 1);
            for (int x = 0; x < bm.Width; x++)
                for (int y = 0; y < bm.Height; y++)
                    if (inHex(x - width / 2, y - height / 2))
                        bm.SetPixel(x, y, color);
            
            g.DrawImage(bm, -width /2 + sx, -height/2 + sy);
        }

        public bool inHex(int x, int y) { //x and y are relative to the center of the hexagon
            if (x < 0)
                x = -x;
            if (y < 0)
                y = -y;
            if (y > height / 2)
                return false;
            if (x > width / 2)
                return false;
            return x < size - (size * y) / height;
        }
    }
}
