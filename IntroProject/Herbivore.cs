using System;
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
    public class Herbivore : Creature
    {
        public Herbivore() : base() {
            color = Color.Red;
        }
        public Herbivore(Creature parentA, Creature parentB) : base(parentA, parentB) { }

        //public override Creature MateWith(Creature other)
        //{
        //    base.MateWith(other);
        //    return new Herbivore(this, other);
        //}
    }
}
