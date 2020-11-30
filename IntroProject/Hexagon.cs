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
        public double h;

        public int width {
            get { return size * 2; }
        }
        public int height {
            get { return (int) (size * sqrt3); }
        }

        private static Color[,] heightColors = new Color[5, 2] {{Color.FromArgb(108, 116, 150), Color.FromArgb(108, 116, 150) } ,
                                                                {Color.FromArgb(108, 116, 150), Color.FromArgb(184, 204, 222) },
                                                                {Color.FromArgb(227, 225, 191), Color.FromArgb(212, 208, 171)},
                                                                {Color.FromArgb(155, 184, 147) ,Color.FromArgb(109, 135, 105) },
                                                                {Color.FromArgb(158, 163, 157), Color.FromArgb(223, 227, 222)} };
        private static float[] heights = new float[6] {-1f,-0.4f, -0.15f, 0.1f, 0.7f, 1f};


        public Hexagon(int size, double c, int x, int y) //size is the length of each side
        {
            h = c;
            calcColor((float)c);
            this.size = size;

            this.x = x;
            this.y = y;

        }

        private Color calcAvrColor(int n, double d) {
            int R = (int)(heightColors[n, 0].R * (1 - d) + heightColors[n,1].R * d);
            int B = (int)(heightColors[n, 0].B * (1 - d) + heightColors[n, 1].B * d);
            int G = (int)(heightColors[n, 0].G * (1 - d) + heightColors[n, 1].G * d);
            return Color.FromArgb(R, G, B);
        }

        private void calcColor(float f) {
            int n = 0;
            for (int i = 1; i < heights.Length - 1; i++)
                if (heights[i] < f)
                    n = i;
            this.color = calcAvrColor(n, (f - heights[n]) / (heights[n + 1] - heights[n]));
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
