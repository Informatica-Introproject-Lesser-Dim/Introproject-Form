﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using IntroProject.Core.Math;

namespace IntroProject
{
    
    public class Hexagon : Point2D
    {
        private Hexagon[] neighbors;
        public List<Entity> entities;
        public Vegetation vegetation;
        public Map parent;
        public Color warmth, color;
        public int size;
        public int Tag = -1;
        public static double sqrt3 = Math.Sqrt(3);
        public static double sqrt2 = Math.Sqrt(2);
        public double heightOfTile; //the height between -1  and 1
        public double longitudeOnMap;
        
        public Hexagon this[int a, int b] //the a is wether you want the neighbor to the left or right, b is wether you want the neighbour up or down
        { 
            get
            {
                if (a == 0 && b == 0)
                    return this;
                if (!(b == 1 || b == -1))
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

        public Hexagon this[int a] { get { return neighbors[a]; } }

        private int width { get { return size * 2; } }//width of a single tile

        private int height { get { return (int)(size * sqrt3); } } //height of a tile

        private struct ColorScale
        {
            public Color startColor, endColor;

            public ColorScale(Color startColor, Color endColor)
            {
                this.startColor = startColor;
                this.endColor = endColor;
            }

            private static int PickValueOnColorChannel(int colorScaleChannelStart, int colorScaleChannelEnd, double fractionPlaceOnScale) =>
                Math.Min(255, Math.Max(0, (int)(colorScaleChannelStart * (1 - fractionPlaceOnScale) + colorScaleChannelEnd * fractionPlaceOnScale)));

            public static Color PickValue(ColorScale colorScale, double fractionPlaceOnScale)
            {
                int R = PickValueOnColorChannel(colorScale.startColor.R, colorScale.endColor.R, fractionPlaceOnScale);
                int G = PickValueOnColorChannel(colorScale.startColor.G, colorScale.endColor.G, fractionPlaceOnScale);
                int B = PickValueOnColorChannel(colorScale.startColor.B, colorScale.endColor.B, fractionPlaceOnScale);
                return Color.FromArgb(R, G, B);
            }
        }

        //just an array of different colours, tied into the "heights" private array
        private static ColorScale[] heightColors = new ColorScale[5]{ new ColorScale(Color.FromArgb(108, 116, 150), Color.FromArgb(108, 116, 150))
                                                                    , new ColorScale(Color.FromArgb(108, 116, 150), Color.FromArgb(184, 204, 222))
                                                                    , new ColorScale(Color.FromArgb(227, 225, 191), Color.FromArgb(212, 208, 171))
                                                                    , new ColorScale(Color.FromArgb(155, 184, 147), Color.FromArgb(109, 135, 105))
                                                                    , new ColorScale(Color.FromArgb(109, 135, 105), Color.FromArgb(62, 89, 63))
                                                                    };
        public static float seaLevel, deepSea, sand; //multi references //Corresponds to multiple levels of height represented by different colors
        private static float[] heights;

        public Hexagon(int size, double c, int x, int y, double longitudeOnMap, Map map) //size is the length of each side
        {
            parent = map;
            this.longitudeOnMap = longitudeOnMap;
            heightOfTile = c;
            calcColor();
            this.size = size;

            entities = new List<Entity>();
            neighbors = new Hexagon[6];

            this.x = x;
            this.y = y;
            vegetation = new Vegetation(this);
        }

        public void EntityBirth(Entity e)
        {
            e.chunk = this;
            entities.Add(e);
            parent.EntityForceAdd(e);
        }

        public void Grow() => vegetation.Grow();

        public void removeEntity(Entity e) => entities.Remove(e);

        public void moveEntity(Entity e, int dir)
        {
            if (entities.Contains(e))
            {
                this[dir].addEntity(e);
                entities.Remove(e);
            }
        }

        public void moveEntity(Entity e, Hexagon hex)
        {
            if (entities.Contains(e))
            {
                (e.X, e.Y) = e - (hex - this);

                hex.addEntity(e);
                entities.Remove(e);
            }
        }

        private static (int, int) _calcSide(int size, int dir)
        {
            int x, y;

            switch (dir % 3)
            {
                case 0: //through the middle
                    x = 0; y = -(int)(size * Hexagon.sqrt3 / 2);
                    break;
                case 1: //up right or down left
                    x = 3 * size / 4; y = -(int)(size * Hexagon.sqrt3 / 4);
                    break;
                case 2: //down right or up left
                    x = -3 * size / 4; y = -(int)(size * Hexagon.sqrt3 / 4);
                    break;
                default:
                    x = -5; y = -6;
                    break;
            }
            if (dir >= 2 && dir <= 4) //if it's one of these direction: mirror it
            {
                x *= -1; y *= -1;
            }
            return (x, y);
        }

        public static Point calcSide(int size, int dir)
        {
            (int x, int y) = _calcSide(size, dir);
            return new Point(x, y);
        }

        public static Point2D CalcSide(int size, int dir)
        {
            (int x, int y) = _calcSide(size, dir);
            return new Point2D().SetPosition(x, y);
        }

        private Func<Entity, bool> entityIsType<T>() =>
            (Entity entity) => (entity is T);

        public List<Entity> getByType(EntityType type) //get a list of all the entities of this type within the hexagon
        { 
            List<Entity> result = new List<Entity>();

            switch (type)
            {
                case EntityType.Entity:
                    return entities;
                case EntityType.Creature:
                    result.AddRange(entities.Where(entityIsType<Creature>()));
                    return result;
                case EntityType.Herbivore:
                    result.AddRange(entities.Where(entityIsType<Herbivore>()));
                    return result;
                case EntityType.Plant:
                    result.AddRange(entities.Where(entityIsType<DeathPile>()));
                    return result;
                case EntityType.Carnivore:
                    result.AddRange(entities.Where(entityIsType<Carnivore>()));
                    return result;
                default: return result;
            }
        }

        public int FoodValue() => vegetation.FoodValue();

        public Grass bestFood(Point2D point) //gives you the number of the piece of food you should aim for first
        {
            List<Grass> grass = vegetation.FoodLocations();
            if (grass.Count == 0)
                return null;
            Grass result = grass[0];
            double val = calcGrassVal(point, result, vegetation.currentTime);
            double newVal;

            for (int i = 1; i < grass.Count; i++)
            {
                newVal = calcGrassVal(point, grass[i], vegetation.currentTime);
                if (newVal > val)
                {
                    result = grass[i];
                    val = newVal;
                }
            }

            return result;
        }

        private double calcGrassVal(Point2D point, Grass grass, double time)
        {
            double dist = Trigonometry.Distance(point, grass);
            return grass.getVal(time) / dist;
        }

        //bias has to be within 1 and 0, hunger can be anything bigger than 0
        public double Passive(double bias, double hunger) => searchPoint(.5 + .5 * bias, hunger, 1);

        public double active(double bias, double hunger) => searchPoint(bias, hunger, 3);

        public double CarnivoreActive(double bias, double livingBias)
        {
            double result = 0;

            for (int i = 10; i >=0 ; i--)
                result = result*bias + searchPoint(i, EntityType.Herbivore).Count*livingBias + (2- livingBias) * searchPoint(i, EntityType.Plant).Count;
            
            return result;
        }

        private double searchPoint(double bias, double hunger, int j) //later on account for distance of different creatures
        { 
            double result = FoodValue();

            if (j != 0)
                for (int i = 0; i < 6; i++)
                    if (this[i] != null)
                        result += bias * this[i].searchLine(i, j - 1, bias);

            return result * hunger;
        }

        public double searchLine(int dir, int j, double bias) //new version of searchline that specifically searches for plants
        { 
            double result = FoodValue();

            if (j > 0)
            {
                if (dir % 2 == 1)
                    if (this[dir] != null)
                        return result + bias * this[dir].searchLine(dir, j - 1, bias);
                for (int i = dir + 5; i <= dir + 7; i++)
                    if (this[i % 6] != null)
                        result += bias * this[i % 6].searchLine(i % 6, j - 1, bias);
            }

            return result;
        }

        public List<Entity> searchLine(int dir, int j, EntityType type) //continue the search line in this direction
        {
            if (j == 0) //end of the line
                return this.getByType(type);
            if (dir % 2 == 1) //one of the normal lines
                if (neighbors[dir] != null)
                    return neighbors[dir].searchLine(dir, j - 1, type);
            List<Entity> result = new List<Entity>();
            for (int i = dir + 5; i <= dir + 7; i++) //split off in 3 directions
                if (neighbors[i%6] != null)
                    result = result.Concat(neighbors[i%6].searchLine(i%6, j - 1, type)).ToList();
            return result;
        }

        public List<Entity> searchPoint(int j, EntityType type) //from this point onward search for the closest entity
        { 
            if (j == 0)
                return getByType(type);

            List<Entity> result = new List<Entity>();

            //search all three possible directions
            for (int i = 0; i < 6; i++) 
                if (neighbors[i] != null)
                    result = result.Concat(neighbors[i].searchLine(i, j - 1, type)).ToList();
            return result;
        }

        public void activate(double time) => vegetation.prime(time);

        //start at the top and go around through all the neighbors
        public void setNeighbors(Hexagon[] h) => neighbors = h;

        public void addEntity(Entity e)
        {
            e.chunk = this;
            entities.Add(e);
        }

        public void drawEntities(Graphics g, int xPos, int yPos)
        {
            vegetation.draw(g, x + xPos, y + yPos);
            foreach (Entity e in entities)
                e.draw(g, x + xPos, y + yPos, e);
        }

        private void calcColor()
        {
            color = getHeightColor();

            if(Settings.AddHeatMap)
                addHeatColor();
        }

        /// <summary>
        /// Should be used when color needs recalculation (f.e. when toggling heat map)
        /// </summary>
        public void UpdateColor() => calcColor();

        private Color getHeightColor() => getHeightColor((float)heightOfTile);

        private Color getHeightColor(float f)
        {
            int layer = 0;

            for (int i = 1; i < heights.Length - 1; i++)
              if (heights[i] < f)
                layer = i;
            return ColorScale.PickValue(heightColors[layer], (f - heights[layer]) / (heights[layer + 1] - heights[layer]));
        }

        /// <summary>
        ///  Blends an heat hue to this.color
        /// </summary>
        /// <returns></returns>
        private void addHeatColor()
        {
            var warmthScale = new ColorScale(Color.FromArgb(0x55aa1111), Color.FromArgb(0x660033dd));
            var snowScale = new ColorScale(Color.FromArgb(0x66003377), Color.FromArgb(0x55ffff));
            double relHeight = Math.Max(0, heightOfTile) / 2 + Math.Abs(.5 - longitudeOnMap);
            ColorScale scale;

            if (relHeight > .95)
                scale = snowScale;
            else
                scale = warmthScale;

            warmth = ColorScale.PickValue(scale, relHeight);
            color = Color.FromArgb((color.R + warmth.R) >> 1, (color.G + warmth.G) >> 1, (color.B + warmth.B) >> 1);
        }

        public void draw(Graphics g, int sx, int sy) //center of the hexagon is at sx,sy
        { 
            //goes through all pixels around the hexagon and colours the ones within it
            Bitmap bm = new Bitmap(width + 1, height + 1);

            for (int x = 0; x < bm.Width; x++)
                for (int y = 0; y < bm.Height; y++)
                    if (inHex(x - width / 2, y - height / 2))
                        bm.SetPixel(x, y, color); 
            
            g.DrawImage(bm, -width /2 + sx, -height/2 + sy);
        }

        public bool inHex(int x, int y) //x and y are relative to the center of the hexagon
        {
            x = Math.Abs(x);
            y = Math.Abs(y);

            if (y > height / 2 || x > width / 2)
                return false;

            return x < size - (size * y) / height;
        }

        public static void calcHeight(float nullLevel)
        {
            float pos = 1f - nullLevel;
            deepSea = nullLevel + (pos - 2f) * 0.4f;
            seaLevel = nullLevel + (pos - 2f) * 0.15f;
            sand = nullLevel + pos * 0.1f;
            float grass = nullLevel + pos * 0.7f;
            heights = new float[6] { -1f, deepSea, seaLevel, sand, grass, 1f };
        }

        public static void Update() => Vegetation.UpdateGrass();
    }
}
