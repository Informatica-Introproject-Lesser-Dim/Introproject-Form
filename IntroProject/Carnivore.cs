using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using IntroProject.Core.Math;

namespace IntroProject
{
    public class Carnivore : Creature
    {
        public Entity targetFood;
        public Carnivore() 
        {
            energyVal = 600; //start with quite a lot so they dont die too quickly
            gene = new CarnivoreGene();
            maxEnergy = this.gene.Size;

            gene.@class = this.GetType().Name;
            stamina = gene.SprintDuration;
        }

        public override void passiveSearch()
        {
            List<Entity> herbivores = new List<Entity>();
            List<Entity> deathPiles = new List<Entity>();
            for(int i = 0; i < 5; i++)
                herbivores =  herbivores.Concat(chunk.searchPoint(i, EntityType.Herbivore)).ToList();
            for (int i = 0; i < 6; i++)
                deathPiles = deathPiles.Concat(chunk.searchPoint(i, EntityType.Plant)).ToList();

            //preference to deathPiles
            if (deathPiles.Count > 0)
            {
                Entity deathPile = findClosest(deathPiles);
                
                //make a route to this
                SingleTargetAStar aStar = new SingleTargetAStar(this, chunk, gene, chunk.size, energyVal, deathPile);
                route = aStar.getResult();

                if (route != null)
                {
                    goal = Goal.Food;
                    color = Color.Pink;
                    target = deathPile;
                    return;
                }
            }
            if (herbivores.Count > 0)
            {
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
            if (target == null)
                return true;
            if (target.dead)
                return true;
            if (stamina <= 0)
                return true;

            Point2D delta = target - GlobalLoc;

            double dist = Trigonometry.Distance(target, GlobalLoc);
            if (dist < 5)
            {
                eat(target);
                return true;
            }

            delta.X *= 1 / dist;
            delta.Y *= 1 / dist;
            delta.X *= dt * gene.SprintSpeed;
            delta.Y *= dt * gene.SprintSpeed;
            X += delta.X;
            Y += delta.Y;

            energyVal -= Calculator.SprintEnergyPerTic(gene);
            stamina -= 2*dt;

            int[] hexPos = chunk.parent.PosToHexPos(GlobalLoc.x, GlobalLoc.y);
            Hexagon newHex = chunk.parent[hexPos[0], hexPos[1]];
            if (newHex != chunk) 
            {
                energyVal -= Calculator.JumpCost(gene);
                chunk.moveEntity(this, newHex);
            }
            
            return false;
        }

        protected override void getActiveRoute()
        {
            CarnivoreAStar aStar = new CarnivoreAStar(this, chunk, gene, chunk.size, energyVal);
            route = aStar.getResult();

            if (route != null)
            {
                targetFood = aStar.getFood();
                if (targetFood != null)
                {
                    goal = Goal.Food;
                    color = Color.FromArgb(150, 50, 50);
                }
                else
                {
                    goal = Goal.Nothing;
                    color = Color.FromArgb(50, 50, 50);
                }
            }
            else
            {
                sleep = 50;
                this.color = Color.Purple;
            }
        }

        public override void draw(Graphics g, int hexX, int hexY, Entity e)
        {
            g.FillEllipse(new SolidBrush(Color.LimeGreen), hexX + x - r, hexY + y - r, r * 2, r * 2);
            if (selected)
                g.DrawEllipse(Pens.LightGreen, hexX + x - r, hexY + y - r, r * 2, r * 2);
        }

        public Carnivore(Gene gene, double energy) : base(gene, energy) { }

        public override Creature FromParentInfo(Gene gene, double energy) =>
            new Carnivore(gene, energy);
    }
}
