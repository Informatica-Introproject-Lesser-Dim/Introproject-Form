using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace IntroProject
{
    public class Carnivore : Creature
    {
        private Entity targetFood;
        public Carnivore() : base() { }

        public Carnivore(Creature parentA, Creature parentB) : base(parentA, parentB) { }

        public override void passiveSearch()
        {
            List<Entity> herbivores = new List<Entity>();
            List<Entity> deathPiles = new List<Entity>();
            for(int i = 0; i < 4; i++)
                herbivores =  herbivores.Concat(chunk.searchPoint(i, EntityType.Herbivore)).ToList();
            for (int i = 0; i < 6; i++)
                deathPiles = deathPiles.Concat(chunk.searchPoint(i, EntityType.Plant)).ToList();

            //preference to deathPiles
            if (deathPiles.Count > 0) {
                Entity deathPile = findClosest(deathPiles);
                //make a route to this

                SingleTargetAStar aStar = new SingleTargetAStar(new Point(this.x, this.y), this.chunk, this.gene, this.chunk.size, this.energyVal, deathPile);
                route = aStar.getResult();

                if (route != null)
                {
                    goal = Goal.Food;
                    this.color = Color.Pink;
                    this.target = deathPile;
                    return;
                }
            }
            if (herbivores.Count > 0) {
                Entity herbivore = findClosest(herbivores);
                //make a straight line to this
                target = herbivore;
                goal = Goal.Creature;
                return;
            }

            //if the passive search has failed
            activeSearch();
        }

        protected override bool SprintToCreature(double dt)
        {
            //move to the creature
            //return true if you're still busy
            //eat at the end
            //return false when you run out of stamina or if you eat the creature
            return true;
        }
        protected override void getActiveRoute()
        {
            CarnivoreAStar aStar = new CarnivoreAStar(new Point(this.x, this.y), this.chunk, this.gene, this.chunk.size, this.energyVal);
            route = aStar.getResult();

            if (route != null)
            {
                targetFood = aStar.getFood();
                if (targetFood != null)
                {
                    goal = Goal.Food;
                    this.color = Color.FromArgb(150, 50, 50);
                }
                else
                {
                    goal = Goal.Nothing;
                    this.color = Color.FromArgb(50, 50, 50);
                }

            }
            else
            {
                sleep = 20;
                this.color = Color.Purple;
            }
        }


        public Carnivore(Gene gene, double energy) : base(gene, energy) { }
        public override Creature FromParentInfo(Gene gene, double energy) =>
            new Carnivore(gene, energy);
    }
}
