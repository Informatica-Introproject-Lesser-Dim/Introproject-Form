﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace IntroProject
{
    public class Hexagon
    {
        private int size;
        public static double sqrt3 = Math.Sqrt(3);
        public static double sqrt2 = Math.Sqrt(2);
        public int x, y;
        public Color color;
        public double heightOfTile;
        public double longitudeOnMap;
        private Hexagon[] neighbors;
        public List<Entity> entities;
        public Hexagon this[int a, int b] { //the a is wether you want the neighbor to the left or right, b is wether you want the neighbour up or down
            get {
                if (a == 0 && b == 0)
                    return this;
                if (!(b == 1 || b == -1) || a < -1 || a > 1)
                    return null;
                if (a == 0)
                    return neighbors[(int)(1.5 + b * 1.5)];
                if (a == 1)
                    return neighbors[(int)(1.5 + b * 0.5)];
                if (a == -1) //this if statement is unneccessary but is kept to make the code clear
                    return neighbors[(int)(4.5 + b * -0.5)];
                return null;
            }
        }

        public int width {
            get { return size * 2; }
        }
        public int height {
            get { return (int) (size * sqrt3); }
        }

        private struct ColorScale
        {
            public Color startColor;
            public Color endColor;

            public ColorScale(Color startColor, Color endColor)
            {
                this.startColor = startColor;
                this.endColor = endColor;
            }

            private static int PickValueOnColorChannel(int colorScaleChannelStart, int colorScaleChannelEnd, double fractionPlaceOnScale) =>
                (int)(colorScaleChannelStart * (1 - fractionPlaceOnScale) + colorScaleChannelEnd * fractionPlaceOnScale);

            public static Color PickValue(ColorScale colorScale, double fractionPlaceOnScale)
            {
                int R = PickValueOnColorChannel(colorScale.startColor.R, colorScale.endColor.R, fractionPlaceOnScale);
                int G = PickValueOnColorChannel(colorScale.startColor.G, colorScale.endColor.G, fractionPlaceOnScale);
                int B = PickValueOnColorChannel(colorScale.startColor.B, colorScale.endColor.B, fractionPlaceOnScale);
                return Color.FromArgb(R, G, B);
            }
        }

        private static ColorScale[] heightColors = new ColorScale[5]{ new ColorScale(Color.FromArgb(108, 116, 150), Color.FromArgb(108, 116, 150))
                                                                    , new ColorScale(Color.FromArgb(108, 116, 150), Color.FromArgb(184, 204, 222))
                                                                    , new ColorScale(Color.FromArgb(227, 225, 191), Color.FromArgb(212, 208, 171))
                                                                    , new ColorScale(Color.FromArgb(155, 184, 147), Color.FromArgb(109, 135, 105))
                                                                    , new ColorScale(Color.FromArgb(158, 163, 157), Color.FromArgb(223, 227, 222))
                                                                    };
        private static float[] heights = new float[6] {-1f,-0.4f, -0.15f, 0.1f, 0.7f, 1f};


        public Hexagon(int size, double c, int x, int y, double longitudeOnMap) //size is the length of each side
        {
            this.longitudeOnMap = longitudeOnMap;
            heightOfTile = c;
            calcColor((float)c);
            this.size = size;

            entities = new List<Entity>();
            neighbors = new Hexagon[6];

            this.x = x;
            this.y = y;
        }

        public void setNeighbors(Hexagon[] h) { //start at the top and go around through all the neighbors
            neighbors = h;
        }

        public void addEntity(Entity e) {
            //be sure to change values in the entity like it's position and in wich chunck it's saved
            e.chunk = this;
            this.entities.Add(e);
        }

        private void calcColor(float f) {
            int layer = 0;
            for (int i = 1; i < heights.Length - 1; i++)
              if (heights[i] < f)
                layer = i;
            this.color = ColorScale.PickValue(heightColors[layer], (f - heights[layer]) / (heights[layer + 1] - heights[layer]));

            var warmthScale = new ColorScale(Color.FromArgb(0x55aa1111), Color.FromArgb(0x660033dd));
            var snowScale = new ColorScale(Color.FromArgb(0x66003377), Color.FromArgb(0x55ffff));

            double relHeight = Math.Max(0, heightOfTile) / 2 + Math.Abs(.5 - longitudeOnMap);

            ColorScale scale;
            if (relHeight > .95)
                scale = snowScale;
            else
                scale = warmthScale;

            var warmth = ColorScale.PickValue(scale, relHeight);
            this.color = Color.FromArgb((color.R + warmth.R) >> 1, (color.G + warmth.G) >> 1, (color.B + warmth.B) >> 1);
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
