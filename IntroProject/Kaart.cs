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
                    int yPos = (int)((margin + Hexagon.sqrt3 * size)*y) + yOff;
                    tiles[x, y] = new Hexagon(size, random.NextDouble(), xPos, yPos);
                }
            }
        }

        public void draw(Graphics g, int xPos, int yPos) {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    tiles[x, y].draw(g, xPos + tiles[x, y].x, yPos + tiles[x, y].y);
        }

        public int[] PosToHexPos(int x, int y) 
        {
            int kolom = (int) (x / (size * 3 + margin * Hexagon.sqrt3)); //2x brede kolom waar dit punt inzit
            if (x < 0)
                kolom--;
            int relXPos = x - (int)((kolom + 0.5) * (size * 3 + margin * Hexagon.sqrt3)); //de x relatief tot het midden van zijn kolom
            
            int rij = (int)(y / (size * Hexagon.sqrt3 + margin)); //de rij waar dit inzit ervanuitgaande dat we in de middelste rij hexagons in onze kolom zitten
            if (y < 0)
                rij--;
            
            int relYPos = y - (int)((rij + 0.5) * (size * Hexagon.sqrt3 + margin)); //de y relatief tot onze middelste hexagon
            int rXPos = relXPos;
            int rYPos = relYPos;
            if (rXPos < 0)
                rXPos = -rXPos;
            if (rYPos < 0)
                rYPos = -rYPos;
            int mSize = (int)(size + (margin) / Hexagon.sqrt3);
            if (rXPos < mSize - (rYPos / Hexagon.sqrt3))
            {
                return new int[2] {kolom * 2 + 1, rij };
            }
            if (relXPos > 0) {
                if (relYPos > 0)
                    return new int[2] { kolom * 2 + 2 , rij + 1};
                return new int[2] { kolom * 2 + 2, rij};
            }
            if (relYPos > 0)
                return new int[2] { kolom * 2, rij + 1 };
            return new int[2] { kolom * 2, rij};
        }
    }
}
