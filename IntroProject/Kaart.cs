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
    public class Kaart
    {
        private int width;
        private int height;
        private int size;
        private int margin;

        Hexagon[,] tiles;
        Random random;

        public Kaart(int width, int height, int size, int margin) {
            this.width = width;
            this.height = height;
            this.size = size;
            this.margin = margin;
            tiles = new Hexagon[width, height];
            random = new Random();

            for (int x = 0; x < width; x++)
            {
                int xPos = (int) (x*(size * 3 + margin * Hexagon.sqrt3)/2);
                int yOff = 0;
                if (x % 2 == 1)
                    yOff = (int)((size * Hexagon.sqrt3 + margin) / 2);
                for (int y = 0; y < height; y++)
                {
                    int yPos = (int)(margin + Hexagon.sqrt3 * size)*y + yOff;
                    tiles[x, y] = new Hexagon(size, random.Next(0,255), xPos, yPos);
                }
            }
        }

        public void draw(Graphics g, int xPos, int yPos) {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    tiles[x, y].draw(g, xPos + tiles[x, y].x, yPos + tiles[x, y].y);
        }
    }
}
