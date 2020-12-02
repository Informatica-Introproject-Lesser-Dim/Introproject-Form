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
    public class Ecosystem {
        public Settings settings;
        public Map map;

        public Ecosystem(Settings settings) {
            this.settings = settings;
            map = new Map(settings.mapWidth, settings.mapHeight, settings.tileSize, 0);
        }

        public void Draw(Graphics g, int x, int y) {
            map.draw(g, x, y);
        }
    
    }
}
