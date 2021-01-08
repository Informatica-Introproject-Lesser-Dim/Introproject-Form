using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace IntroProject
{

    //TO DO: drawing method
    //TO DO: way for eating the plants/grass (preferably in small bites at a time so that it isnt just instantly gone)
    public class Vegetation
    {
        //"Constants" (dont usually change while the program is running)
        int min = 50, max = 10000; //how much time it usually takes before a new bit of vegetation is grown

        int maxPlants = 8; 
        double fertillity = 1;

        double plantBoost = 0.2;
        public Grass this[int n] { get { return grass[n]; } }

        //normal variables
        int targetTime = 0;
        public int currentTime = 0;
        List<Grass> grass;
        Hexagon tile;

        public Vegetation(Hexagon tile) {
            this.tile = tile;

            matchSpawnStatsWithTileHeight();
            preGenGrassUnvisible();
            setSpawnTimer();
        }

        private void matchSpawnStatsWithTileHeight()
        {
            if (tile.heightOfTile < Hexagon.sand - 0.05)
            {
                maxPlants = 1;
                fertillity = 0.25;
            }
            if (tile.heightOfTile < Hexagon.seaLevel)
                maxPlants = 0;
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
        public void actvate(int time) {
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
            bool temp = maxPlants > 1 && x; //if this stays true then a berrybush will need to grow (if it's not already there)
            double boost = 1;
            foreach (Grass g in grass)
            {
                if (g.visible)
                {
                    boost += plantBoost;
                    continue;
                }

                g.visible = true;
                g.start(currentTime);
                temp = false;
                break;
            }

            Random random = new Random();

            if (temp)
            {
                //try to put grass on neighbor tiles
                Hexagon neighborTile = tile[random.Next(0, 6)];
                if (neighborTile != null)
                    neighborTile.Grow();

                boost = boost / 4; //putting grass on neighbor tiles happens but slower
            }

            targetTime = currentTime + (int)(random.Next((int)(min / fertillity), (int)(max / fertillity)) / boost); //when this is reached a new plant will grow
        }
        public void draw(Graphics g, int x, int y) {
            foreach (Grass gr in grass)
                if (gr.visible)
                    gr.draw(g, x, y);
        }//TO DO

        //general method for when you want to know an estamete of how "good" a chunck is without examining each individual piece of food in it
        public int FoodValue() {
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
    }

    //random creation of each piece of grass would probably take up a load of performance
    //therefore my idea was to create all the random locations at the beginning
    //and when the plants "grow" you change the visible variable to true
    public class Grass {//I suppose that since it can also grow in water it should be called algea or smthn else
        //constants
        private const int min = 50, max = 200;//these numbers are up for discussion, right now it's just temporary placeholders
        private const int growRange = 200; /* Settings.GrassGrowth */
        private const int growthTime = 3; //how much time to grow one bit
        private const int radius = 5; //temporary and used for drawing the grass (will later on probably be replaced by some actual pictures)
        //normal variables

        public Point loc;
        private int foodStart;
        private int time;
        public bool visible = false;
        
        public Grass(int size) { //random creation

            //random location is decided by a random radius and angle
            //because of this the points are on average closer to the middle than the edge
            Random random = new Random();
            double r = random.NextDouble() * size * Hexagon.sqrt3 * 0.5;
            double d = random.NextDouble() * 2 * Math.PI;
            loc = new Point( (int) (r * Math.Cos(d)), (int) (r * Math.Sin(d)));
            foodStart = random.Next(min, max);
        }
        public Grass(Point loc) {
            this.loc = loc;
            Random random = new Random();
            foodStart = random.Next(min, max); 
        }

        public Grass(Point loc, int foodValue) {
            this.loc = loc;
            this.foodStart = foodValue;
        }

        public void start(int time) {
            this.time = time;
        }

        public void draw(Graphics g, int x, int y) {
            if (!visible)
                return;
            g.FillEllipse(new SolidBrush(Color.FromArgb(70, 60, 255, 0)), loc.X + x - radius, loc.Y + y - radius, radius * 2, radius * 2);
        }

        public int getVal(int time) {
            int added = (time - this.time) / growthTime;
            if (added > growRange)
                added = growRange;
            return foodStart + added;
        }
    }
}
