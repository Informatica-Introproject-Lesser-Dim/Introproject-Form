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
    public partial class Scherm
    {
        Ecosystem ecosystem;
        Overlay overlay;
        public Scherm(Ecosystem eco)
        {
            ecosystem = eco;
            overlay = new Overlay(eco.settings);
            // (mogelijk) na informatie van de user is verkregen maak een processor classe, om het ecosysteem op te zetten.
        }

        public void Draw(Graphics g) {
            ecosystem.Draw(g, 0, 0);
            overlay.Draw(g);

        }
    }
}
