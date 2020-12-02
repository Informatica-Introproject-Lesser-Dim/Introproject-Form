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
    public class Processor
    {
        //Hoofd processor die het ecosysteem zal maken, door Maaker aan te roepen. (misschien niet nodig)
        Scherm scherm;
        Ecosystem ecosystem;
        Settings settings;

        public Processor(int width, int height) {
            settings = new Settings();
            settings.camHeight = height;
            settings.camWidth = width;
            settings.mapHeight = 50;
            settings.mapWidth = 100;
            settings.tileSize = 20;
            ecosystem = new Ecosystem(settings);
            scherm = new Scherm(ecosystem);
        }

        public void Draw(Graphics g) {
            scherm.Draw(g);
        
        }
    }
}
