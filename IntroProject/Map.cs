using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

using LibNoise.Primitive;

namespace IntroProject
{
    public class Map
    {
        private List<Entity> entities;
        private List<Entity> children;
        private List<Entity> deaths;
        private List<Entity> eaten;
        private double time = 0;
        public double HerbivoreVelocity;
        public double CarnivoreVelocity;
        public double HerbivoreSize;
        public double CarnivoreSize;
        public int HerbivoreCount = 0;
        public int CarnivoreCount = 0;
        public int malesAdded = 0;
        public int femalesAdded = 0;
        public int width;
        public int height;
        private int n = 4;
        private int size;
        private int margin;

        private double msPerTick { get { return 30 / Settings.StepSize; } }

        public Hexagon[,] tiles;
        public Hexagon this[int x, int y] { get { return getHex(x, y); } }

        Random random = new Random();
        SimplexPerlin[] perlin;
        private float biasRange = 20;

        Bitmap mapBase;

        public Map(int width, int height, int size, int margin)
        {
            this.width = width;
            this.height = height;
            this.size = size;
            this.margin = margin;
            entities = new List<Entity>();
            children = new List<Entity>();
            deaths = new List<Entity>();
            tiles = new Hexagon[width, height];
            perlin = new SimplexPerlin[4];
            Hexagon.calcHeight(Settings.MiddleHeight);

            for (int i = 0; i < n; i++)
                perlin[i] = new SimplexPerlin(random.Next(), LibNoise.NoiseQuality.Best);

            for (int x = 0; x < width; x++) //creating all the tiles and calculating the correct perlin noise for it
            {
                int xPos = (int)(x * (size * 3 + margin * Hexagon.sqrt3) / 2);
                int yOff = 0;
                if (x % 2 == 1)
                    yOff = (int)((size * Hexagon.sqrt3 + margin) / 2);
                for (int y = 0; y < height; y++)
                {
                    int yPos = (int)((margin + Hexagon.sqrt3 * size) * y) + yOff;

                    tiles[x, y] = new Hexagon(size, calcNoise((xPos * 1.0f) / (size + margin), (yPos * 1.0f) / (size + margin)), xPos, yPos, (double)y / height, this);
                }
            }

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    tiles[x, y].setNeighbors(calcNeighbors(x, y));

            drawBase();
        }

        public void EntityForceAdd(Entity e)
        {
            children.Add(e);
            if (e.gender == 1)
                malesAdded++;
            else if (e.gender == 0)
                femalesAdded++;
        }

        public void placeEntity(Entity e, int x, int y) //no randomness, just normally placing it here
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return;
            tiles[x, y].addEntity(e);
            entities.Add(e);
            if (e is Herbivore)
            {
                HerbivoreCount++;
                HerbivoreVelocity += ((Creature)e).gene.Velocity;
                HerbivoreSize += ((Creature)e).gene.Size;
            }
            if (e is Carnivore)
            {
                CarnivoreCount++;
                CarnivoreVelocity += ((Creature)e).gene.Velocity;
                CarnivoreSize += ((Creature)e).gene.Size;
            }
        }



        private void activateEntities(double dt) {
            
            if (deaths.Count > 0)
            {
                foreach (Entity deadEntity in deaths)
                {
                    if (deadEntity is Herbivore)
                    {
                        HerbivoreCount--;
                        HerbivoreVelocity -= ((Creature)deadEntity).gene.Velocity;
                        HerbivoreSize -= ((Creature)deadEntity).gene.Size;
                    }
                    else if (deadEntity is Carnivore)
                    {
                        CarnivoreCount--;
                        CarnivoreVelocity -= ((Creature)deadEntity).gene.Velocity;
                        CarnivoreSize -= ((Creature)deadEntity).gene.Size;
                    }
                    if (!deadEntity.eaten)
                        deadEntity.PerishToDeathPile();
                    else 
                        deadEntity.chunk.removeEntity(deadEntity);
                    entities.Remove(deadEntity);
                }
                deaths.Clear();
            }

            if (children.Count > 0)
            {
                foreach (Entity Child in children)
                {
                    if (Child is Herbivore)
                    {
                        HerbivoreCount++;
                        HerbivoreVelocity += ((Creature)Child).gene.Velocity;
                        HerbivoreSize += ((Creature)Child).gene.Size;
                    }
                    else
                    {
                        CarnivoreCount++;
                        CarnivoreVelocity += ((Creature)Child).gene.Velocity;
                        CarnivoreSize += ((Creature)Child).gene.Size;
                    }
                    entities.Add(Child);
                }
                children.Clear();
            }

            foreach (Entity e in entities) {
                if (e is Creature creature)
                {
                    creature.activate(dt / msPerTick);
                    if (creature.dead)
                        deaths.Add(creature);
                }
                else if (e is DeathPile deathPile && e.dead)
                    deaths.Add(deathPile);
            }  
        }

        public void TimeStep(double dt)
        {
            Statistics CurrentStats = new Statistics(time, HerbivoreCount, CarnivoreCount, HerbivoreVelocity, CarnivoreVelocity, HerbivoreSize, CarnivoreSize);
            StatisticsValues.AddStats(CurrentStats);
            time += dt; //the map keeps the time so that not each hexagon has to keep the time for itself
            this.activateEntities(dt); //activating all the entities
            foreach (Hexagon hexagon in tiles)
                hexagon.activate(time / msPerTick); //letting each hexagon grow it's plants
        }


        private Hexagon[] calcNeighbors(int x, int y) {
            Hexagon[] result = new Hexagon[6];
            int a = x % 2; //bias for when you're in one of the lower colums
            result[0] = getHex(x, y - 1); //specifying the specific hex adress for the neighbor in each direction
            result[1] = getHex(x + 1, y - 1 + a);
            result[2] = getHex(x + 1, y + a);
            result[3] = getHex(x, y + 1);
            result[4] = getHex(x - 1, y + a);
            result[5] = getHex(x - 1, y - 1 + a); //a for loop would be longer than this code itself here (all the neighbors)
            return result;
        }

        private Hexagon getHex(int x, int y) //return hex based on hex adress
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
                return tiles[x, y];
            return null;
        }

        public int[] countMalesAndFemales()
        {
            int males = 0;
            int females = 0;

            foreach (Entity e in entities)
                if (e.gender == 1)
                    males++;
                else
                    females++;
            return new int[2] { males, females };
        }
        public int[] countHerbivoresAndCarnivores()
        {
            int herbivores = HerbivoreCount;
            int carnivores = CarnivoreCount;

            return new int[2] { herbivores, carnivores };
        }

        private float calcNoise(float x, float y) //adding multiple perlin noise functions on top of eachother to make it look more natural
        {
            //each layer has a smaller size and a contributes less to the overall perlin noise
            float factor = 0.3f;
            Func<int, float> scaled = index => (float)Math.Pow(factor, index);
            float[] factors = Enumerable.Range(0, n + 1).Select(scaled).ToArray();
            float result = 0f;

            for (int i = 0; i < n; i++)
            {
                float scaledPerlinNoise = factors[i] * perlin[i].GetValue(x * factors[n - i - 1], y * factors[n - i - 1]);
                if (i == n - 1)
                    result += factor * scaledPerlinNoise;
                result += (1 - factor) * scaledPerlinNoise;
            }

            return Bias(result, x, y);
        }

        public Entity GetCreature(int x, int y, float range)
        {
            int[] hexPos = PosToHexPos(x, y);

            if (hexPos[0] < 0 || hexPos[0] >= width || hexPos[1] < 0 || hexPos[1] >= height)
                return null;

            Hexagon hex = tiles[hexPos[0], hexPos[1]];
            List<Entity> entities = hex.searchPoint(0, EntityType.Entity);
            entities = entities.Concat(hex.searchPoint(1, EntityType.Entity)).ToList();
            if (entities.Count == 0)
                return null;

            double val = Calculator.distanceSquared(x, y, entities[0].x, entities[0].y);
            Entity best = entities[0];
            foreach (Entity entity in entities)
            {
                double temp = Calculator.distanceSquared(x, y, entity.GlobalLoc.X, entity.GlobalLoc.Y);
                if (temp < val)
                {
                    val = temp;
                    best = entity;
                }
            }

            if (val > range * range)
                return null;
            return best;
        }

        private float Bias(float f, float x, float y) //a bias is needed so that the edges of the map are always water
        {
            //not much strange happens here, just deciding wich edge you're closest to and then deciding a bias
            //based on how far you are from this edge
            x *= size + margin;
            y *= size + margin;
            int w = HexAdressToXY(width - 1, height - 1)[0];
            int h = HexAdressToXY(width - 1, height - 1)[1];

            if (w - x < x)
                x = w - x;
            if (h - y < y)
                y = h - y;

            float val = Math.Min(x, y);
            if (val < biasRange * size)
                return Math.Max(-1, f - (1.0f * (biasRange * size - val)) / (biasRange * size));
            return f;
        }


        public void drawBase() //just basic going through all the pixels and deciding in wich hexagon they reside
        {
            mapBase = new Bitmap(tiles[width - 1, height - 1].x + 2 * size, tiles[width - 1, height - 1].y + (int)(size * Hexagon.sqrt3));

            if (margin == 0)
            {
                for (int x = 0; x < mapBase.Width; x++)
                    for (int y = 0; y < mapBase.Height; y++)
                        mapBase.SetPixel(x, y, drawPixel(x - size, (int)(y - size * 0.5 * Hexagon.sqrt3)));
                return;
            }

            Graphics g = Graphics.FromImage(mapBase);
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    tiles[x, y].draw(g, size + tiles[x, y].x, (int)(size * Hexagon.sqrt3 / 2) + tiles[x, y].y);
        }

        private Color drawPixel(int x, int y) //returns the color of the hexagon at this position
        {
            int[] pos = PosToHexPos(x, y);
            if (pos[0] < 0 || pos[0] >= width || pos[1] < 0 || pos[1] >= height)
                return Color.FromArgb(0, 0, 0, 0);
            return tiles[pos[0], pos[1]].color;
        }

        public void draw(Graphics g, int xPos, int yPos, int scrWidth, int scrHeight)
        {
            g.DrawImage(mapBase, xPos - size, yPos - (int)(size * Hexagon.sqrt3 / 2)); //sticks a copy of the base on the graphics
            int[] start = PosToHexPos(-xPos, -yPos);
            start[0]--;
            start[1]--;
            int[] end = PosToHexPos(-xPos + scrWidth, -yPos + scrHeight);
            end[0]++;
            end[1]++;
            for (int x = start[0]; x <= end[0]; x++)//for each hexagon within a certain range: draw all the entities within it
                for (int y = start[1]; y <= end[1]; y++)
                    if (x >= 0 && y >= 0 && x < width && y < height)
                        tiles[x, y].drawEntities(g, xPos, yPos);

        }

        public void placeRandom(Entity e) //place an entity on a random chunck above sea level
        {
            int x, y;
            do
            {
                x = random.Next(0, width);
                y = random.Next(0, height);
            } while (tiles[x, y].heightOfTile < Hexagon.seaLevel); //just keep trying untill you find a position that works

            this.placeEntity(e, x, y);
        }

        public int[] HexAdressToXY(int x, int y)
        {
            int xPos = (int)(x * (size * 3 + margin * Hexagon.sqrt3) / 2);//just the amount of times you go a hexWidth to the right times hexWidth
            int yOff = 0;
            if (x % 2 == 1)
                yOff = (int)((size * Hexagon.sqrt3 + margin) / 2); //the even colums are slightly lower therefore an offset is needed

            int yPos = (int)((margin + Hexagon.sqrt3 * size) * y) + yOff;//the amount of heights you go down + the offset
            return new int[2] { xPos, yPos };
        }

        public int[] PosToHexPos(int x, int y)
        {
            //first finds the general area and then specifies more and more
            int column = (int)(x / (size * 3 + margin * Hexagon.sqrt3)); // 2-wide column which includes this point
            if (x < 0)
                column--;
            int row = (int)(y / (size * Hexagon.sqrt3 + margin));

            if (y < 0)
                row--;

            int relXPos = x - (int)((column + 0.5) * (size * 3 + margin * Hexagon.sqrt3)); // x center-relative
            int relYPos = y - (int)((row + 0.5) * (size * Hexagon.sqrt3 + margin)); // y relative to the middle hexagon
            int rXPos = Math.Abs(relXPos);//generalizing the positive and negative versions into one
            int rYPos = Math.Abs(relYPos);


            int mSize = (int)(size + (margin) / Hexagon.sqrt3);
            if (rXPos < mSize - (rYPos / Hexagon.sqrt3)) //if you are in the middle hexagon
                return new int[2] { column * 2 + 1, row };
            if (relXPos > 0)//outside of the middle hex, two positives mean it's in the top right hex rest of the hexagons work the same as this
                if (relYPos > 0)
                    return new int[2] { column * 2 + 2, row + 1 };
                else
                    return new int[2] { column * 2 + 2, row };

            if (relYPos > 0)
                return new int[2] { column * 2, row + 1 };
            return new int[2] { column * 2, row };
        }

        public void Update()
        {
            Creature.Update();
            Hexagon.Update();

            foreach (Hexagon tile in tiles)
                tile.UpdateColor();
            drawBase();
        }
    }
}
