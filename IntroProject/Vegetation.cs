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
        double height;
        int size;
        int min = 50, max = 10000; //how much time it usually takes before a new bit of vegetation is grown
        int maxPlants = 8; //

        //normal variables
        int targetTime = 0;
        int currentTime = 0;
        List<Grass> grass;
        BerryBush bush;

        public Vegetation(double height, int size) {
            this.height = height;
            this.size = size;
            if (height < Hexagon.seaLevel)
                maxPlants = 3;
            grass = new List<Grass>();
            for (int i = 0; i < maxPlants; i++) //all the plants are already created but not visible yet
                grass.Add(new Grass(size));
            bush = new BerryBush(size);
            Random random = new Random();
            targetTime = random.Next(min, max);
        }

        //just call this every step in "hexagon"
        public void actvate(int time) {
            currentTime = time;
            if (targetTime > currentTime)
                return; //you basicly dont have to do anything untill you hit the target time

            //adding one plant

            bool temp = grass.Count == 8; //if this stays true then a berrybush will need to grow (if it's not already there)
            foreach(Grass g in grass)
            {
                if (g.visible)
                    continue;
                g.visible = true;
                g.start(time);
                temp = false;
                break;
            }

            if (temp && !bush.visible)
            {
                bush.visible = true;
                bush.start(time);
            }

            Random random = new Random();
            targetTime = time + random.Next(min, max); //when this is reached a new plant will grow
        }
        public void draw(Graphics g, int x, int y) {
            foreach (Grass gr in grass)
                if (gr.visible)
                    gr.draw(g, x, y);
            if (bush.visible)
                bush.draw(g, x, y);
        
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
            if (bush.visible)
                result.Add(bush);
            return result;
        } 
    }

    //random creation of each piece of grass would probably take up a load of performance
    //therefore my idea was to create all the random locations at the beginning
    //and when the plants "grow" you change the visible variable to true
    public class Grass {
        //constants
        private const int min = 20, max = 60;//these numbers are up for discussion, right now it's just temporary placeholders
        private const int growRange = 50;
        private const int growthTime = 10; //how much time to grow one bit
        private const int radius = 5; //temporary and used for drawing the grass (will later on probably be replaced by some actual pictures)
        //normal variables

        public Point loc;
        private int foodStart;
        private int time;
        public bool visible = false;
        
        public Grass(int size) { //random creation
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

    class BerryBush : Grass
    {
        private const int min = 10, max = 30;
        private const int growRange = 200; //starts smaller but grows till much more
        public BerryBush(int size) : base(size)
        {
        }
    }
}
