using System;
using System.Data;
using System.Drawing;
using System.Linq;

using LibNoise.Primitive;

namespace IntroProject
{
    public class Map
    {
        private int width;
        private int height;
        private int size;
        private int margin;

        private Hexagon[,] tiles;
        public Hexagon this[int x, int y] {
            get { return getHex(x, y); }
        }

        Random random = new Random();
        SimplexPerlin[] perlin;
        int n = 4;
        private float biasRange = 20;

        Bitmap mapBase;

        public Map(int width, int height, int size, int margin) {
            this.width = width;
            this.height = height;
            this.size = size;
            this.margin = margin;
            tiles = new Hexagon[width, height];
            perlin = new SimplexPerlin[4];
            for(int i = 0; i < n; i++)
                perlin[i] = new SimplexPerlin(random.Next(), LibNoise.NoiseQuality.Best);

            for (int x = 0; x < width; x++)
            {
                int xPos = (int) (x*(size * 3 + margin * Hexagon.sqrt3)/2);
                int yOff = 0;
                if (x % 2 == 1)
                    yOff = (int)((size * Hexagon.sqrt3 + margin) / 2);
                for (int y = 0; y < height; y++)
                {
                    int yPos = (int)((margin + Hexagon.sqrt3 * size)*y) + yOff;

                    tiles[x, y] = new Hexagon(size, calcNoise((xPos*1.0f)/(size + margin),(yPos*1.0f )/(size + margin)), xPos, yPos);
                }
                
            }

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    tiles[x, y].setNeighbors(calcNeighbors(x,y));

            this.drawBase();
        }

        private Hexagon[] calcNeighbors(int x, int y) {
            Hexagon[] result = new Hexagon[6];
            int a = x % 2;
            result[0] = getHex(x, y - 1);
            result[1] = getHex(x + 1, y - 1 + a);
            result[2] = getHex(x + 1, y + a);
            result[3] = getHex(x, y + 1);
            result[4] = getHex(x - 1, y + a);
            result[5] = getHex(x - 1, y - 1 + a); //a for loop would be longer than this code itself here (all the neighbors)
            return result;
        }

        private Hexagon getHex(int x, int y) {
            if (x >= 0 && x < width && y >= 0 && y < height)
                return tiles[x, y];
            return null;
        }

        private float calcNoise(float x, float y) {
            float factor = 0.3f;

            Func<int, float> scaled = index => (float)Math.Pow(factor, index);
            float[] factors = Enumerable.Range(0, n + 1).Select(scaled).ToArray();

            float result = 0;
            for (int i = 0; i < n; i++)
            {
                float scaledPerlinNoise = factors[i] * perlin[i].GetValue(x * factors[n - i - 1], y * factors[n - i - 1]);
                if (i == n - 1)
                    result += factor * scaledPerlinNoise;
                result += (1 - factor) * scaledPerlinNoise;
            }


            return Bias(result,x,y);
        }

        private float Bias(float f, float x, float y) {
            x *= size + margin;
            y *= size + margin;
            int w = HexAdressToXY(width - 1, height - 1)[0];
            int h = HexAdressToXY(width - 1, height - 1)[1];

            if (w - x < x)
                x = w - x;
            if (h - y < y)
                y = h - y;
            float val = x;
            if (y < x)
                val = y;
            if (val < biasRange * size)
                return Math.Max(-1, f - (1.0f * (biasRange * size - val)) / (biasRange * size));
            return f;
        }


        public void drawBase() {
            mapBase = new Bitmap(tiles[width - 1, height - 1].x + 2 * size, tiles[width - 1, height - 1].y + (int)(size * Hexagon.sqrt3));
            if (margin == 0) {
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

        private Color drawPixel(int x, int y) {
            int[] pos = PosToHexPos(x, y);
            if (pos[0] < 0 || pos[0] >= width || pos[1] < 0 || pos[1] >= height)
                return Color.FromArgb(0, 0, 0, 0);
            return tiles[pos[0], pos[1]].color;
        }

        public void draw(Graphics g, int xPos, int yPos) {
            g.DrawImage(mapBase, xPos - size, yPos - (int) (size*Hexagon.sqrt3/2));
        }

        public int[] HexAdressToXY(int x, int y) {
            int xPos = (int)(x * (size * 3 + margin * Hexagon.sqrt3) / 2);
            int yOff = 0;
            if (x % 2 == 1)
                yOff = (int)((size * Hexagon.sqrt3 + margin) / 2);
            
            int yPos = (int)((margin + Hexagon.sqrt3 * size) * y) + yOff;
            return new int[2] { xPos, yPos };
        }

        public int[] PosToHexPos(int x, int y) 
        {
            int column = (int) (x / (size * 3 + margin * Hexagon.sqrt3)); // 2-wide column which includes this point
            if (x < 0)
                column--;
            int relXPos = x - (int)((column + 0.5) * (size * 3 + margin * Hexagon.sqrt3)); // x center-relative
            
            int row = (int)(y / (size * Hexagon.sqrt3 + margin));
            if (y < 0)
                row--;
            
            int relYPos = y - (int)((row + 0.5) * (size * Hexagon.sqrt3 + margin)); // y relative to the middle hexagon
            int rXPos = relXPos;
            int rYPos = relYPos;
            if (rXPos < 0)
                rXPos = -rXPos;
            if (rYPos < 0)
                rYPos = -rYPos;
            int mSize = (int)(size + (margin) / Hexagon.sqrt3);
            if (rXPos < mSize - (rYPos / Hexagon.sqrt3))
            {
                return new int[2] {column * 2 + 1, row };
            }
            if (relXPos > 0) {
                if (relYPos > 0)
                    return new int[2] { column * 2 + 2 , row + 1};
                return new int[2] { column * 2 + 2, row};
            }
            if (relYPos > 0)
                return new int[2] { column * 2, row + 1 };
            return new int[2] { column * 2, row};
        }
    }
}
