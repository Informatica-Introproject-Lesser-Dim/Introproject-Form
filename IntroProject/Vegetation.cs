using IntroProject.Core.Math;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace IntroProject
{

    public class Vegetation
    {
        //"Constants" (dont usually change while the program is running)
        int min = 50, max = 10000; //how much time it usually takes before a new bit of vegetation is grown

        int maxPlants = 8; 
        double fertillity = 1;

        double plantBoost = 0.2;
        public Grass this[int n] { get { return grass[n]; } }

        //normal variables
        double targetTime = 0;
        public double currentTime = 0;
        List<Grass> grass;
        Hexagon tile;

        public Vegetation(Hexagon tile) {
            this.tile = tile;

            matchSpawnStatsWithTileHeight();
            preGenGrassUnvisible();
            setSpawnTimer();
            Random random = new Random();
            if (random.NextDouble() < 0.1) //at the start a random chance to instantly
                this.Grow(true);
        }

        private void matchSpawnStatsWithTileHeight()
        {
            //depending on the height and heat of the tile a certain maximum amount of plants
            //and a certain fertillity is calculated
            if (tile.heightOfTile < Hexagon.sand - 0.05)
            {
                maxPlants = 1;
                fertillity = 0.25;
                if(Settings.AddHeatMap)
                {
                    fertillity = 0.25 + tile.warmth.R/500 - tile.warmth.B/500;
                }
            }
            if (tile.heightOfTile < Hexagon.seaLevel)
                maxPlants = 0;
            if (tile.heightOfTile > Hexagon.sand - 0.05)
            {
                if (Settings.AddHeatMap)
                    fertillity = 1 + tile.warmth.R/500 - tile.warmth.B/500;
            }
        }
        private void preGenGrassUnvisible()
        {
            grass = new List<Grass>();
            for (int i = 0; i < maxPlants; i++) //all the plants are already created but not visible yet
                grass.Add(new Grass(tile.size));
        }

        private void setSpawnTimer() =>
            targetTime = new Random().Next(min, max);

        //just call this every step in "hexagon"
        public void prime(double time) {
            currentTime = time;
            if (targetTime > currentTime)
                return; //you basicly dont have to do anything untill you hit the target time

            //adding one plant
            this.Grow(true);
        }

        public void Grow() {
            Grow(false);
        }

        private void Grow(bool x) {
            if (maxPlants == 0)
                return;

            bool temp = maxPlants > 1 && x; //whether plants are spread to neighbouring tiles

            double boost = 1;//a speed increase for the following plants after this
            foreach (Grass g in grass)
            {
                if (g.visible)
                {
                    boost += plantBoost; //increase the boost for each plant that has grown
                    continue;
                }

                //grow the first plant found that hasnt grown yet
                g.visible = true;
                g.start(currentTime);

                //if there's one plant that hasnt grown yet: dont spread to neighbouring tiles
                temp = false; 
                break; //we dont want multiple plants to grow, so exit the loop
            }

            Random random = new Random();

            //if you're spreading to neighbouring tiles
            if (temp) 
            {
                //attempt to put grass on a neighbouring tile
                Hexagon neighborTile = tile[random.Next(0, 6)];
                if (neighborTile != null)
                    neighborTile.Grow();

                boost /= 4; //growing on this tile takes slower now...
            }

            targetTime = currentTime + (random.Next((int)(min / fertillity), (int)(max / fertillity)) / boost); //when this is reached a new plant will grow
        }
        public void draw(Graphics g, int x, int y) 
        {
            foreach (Grass gr in grass)
                if (gr.visible)
                    gr.draw(g, x, y);
        }

        //general method for when you want to know an estamete of how "good" a chunck is without examining each individual piece of food in it
        public int FoodValue() 
            //just normal adding all the values together
        {

            int result = 0;
            foreach (Grass g in grass)
                if (g.visible)
                    result += g.getVal(currentTime);
            return result;
        } 

        //clones by reference so watch out with the result
        //use your own method to calculate wich piece of food would be the best
        public List<Grass> FoodLocations() {
            List<Grass> result = new List<Grass>();
            foreach (Grass g in grass)
                if (g.visible)
                    result.Add(g);
            return result;
        }
        public static void UpdateGrass()
        {
            Grass.Update();
        }
    }

    //random creation of each piece of grass would probably take up a load of performance
    //therefore my idea was to create all the random locations at the beginning
    //and when the plants "grow" you change the visible variable to true
    public class Grass : Point2D {//I suppose that since it can also grow in water it should be called algea or smthn else
        //constants
        private const int min = 50, max = 200;//these numbers are up for discussion, right now it's just temporary placeholders
        private const int growthTime = 3; //how much time to grow one bit
        private const int radius = 5; //temporary and used for drawing the grass (will later on probably be replaced by some actual pictures)
        //normal variables

        private int foodStart;
        private int time;
        public bool visible = false;
        private static int growRange = Settings.GrassGrowth;
        private static int feedMax = Settings.GrassMaxFeed;

        public Grass(int size) { //random creation

            //random location is decided by a random radius and angle
            //because of this the points are on average closer to the middle than the edge
            Random random = new Random();
            double r = random.NextDouble() * size * Hexagon.sqrt3 * 0.5;
            double d = random.NextDouble() * 2 * Math.PI;
            base.SetPosition((int) (r * Math.Cos(d)), (int) (r * Math.Sin(d)));
            foodStart = random.Next(min, max);
        }
        public Grass(Point2D loc) {
            SetPosition(loc.X, loc.Y);
            Random random = new Random();
            foodStart = random.Next(min, max); 
        }

        public Grass(Point2D loc, int foodValue) {
            SetPosition(loc.X, loc.Y);
            this.foodStart = foodValue;
        }

        public void start(double time) {
            this.time = (int)time;
        }

        public void draw(Graphics g, int x, int y) {
            if (!visible)
                return;
            g.FillEllipse(new SolidBrush(Color.FromArgb(70, 60, 255, 0)), this.x + x - radius, this.y + y - radius, radius * 2, radius * 2);
        }

        public int getVal(double time) 
            //compares received time to the time when it started growing
            //thene calculates which food value it should have by now
        {

            double added = (time - this.time) / growthTime;

            if (added > growRange)
                added = growRange;

            if (feedMax < foodStart + added)
                return feedMax;
            return (int)(foodStart + added);
        }

        public static void Update ()
        {
            growRange = Settings.GrassGrowth;
            feedMax = Settings.GrassMaxFeed;
        }
    }
}
