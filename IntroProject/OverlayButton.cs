using System;
using System.Collections.Generic;
using System.Text;
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
    public class OverlayButton
    {
        int x, y, w, h;
        public OverlayButton(int x, int y, int w, int h) {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public bool Clik(int xPos, int yPos) {
            return (xPos > x && xPos < x + w) && (yPos > y && yPos < y + h);
        }

        public void Draw(Graphics g) {
            g.FillRectangle(new SolidBrush(Color.DarkBlue), x, y, w, h);
        }


    }
}
