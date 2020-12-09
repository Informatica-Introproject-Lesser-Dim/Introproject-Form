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
    public enum EntityType
    {
        Herbivore,
        Plant,
        Carnivore,
        Entity,
        Creature
    }
    public abstract class Entity
    {
        protected int id; //Vincent: Hoe kan je er voor zorgen dat de id niet overlapt met id's van andere entities?
        public int x,y;
        protected const int r = 10;
        public Hexagon chunk;
        protected Color color;

        public void Sterven()
        {
            //Haal entity weg uit de chunk entitie lijst, door entity met hetzelfde id als deze uit de lijst te halen.
        }

        public virtual void draw(Graphics g, int hexX, int hexY) {
            g.FillEllipse(new SolidBrush(color), hexX + x - r, hexY + y - r, r * 2, r * 2);
        }
    }
}
