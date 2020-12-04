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

    //this class is for when you need to test something that doesnt end up in the final code
    class Debug : Form
    {
        DebugScreen debugscr;

        public Debug() 
        {
            this.Size = new Size(1800, 1200);
            debugscr = new DebugScreen(1800, 1200);
            this.Controls.Add(debugscr);
        }
    }

    class DebugScreen : UserControl
    {
        Map kaart;
        int[] pos = new int[2] { 0, 0 };
        Font font = new Font("Arial", 12);

        public DebugScreen(int w, int h) 
        {
            this.Size = new Size(w, h);
            this.Paint += drawScreen;
            kaart = new Map(10, 10, 20, 0);
            this.MouseClick += Klik;
        }

        public void Klik(object o, MouseEventArgs mea) {
            int[] adress = kaart.PosToHexPos(mea.X - 50, mea.Y - 50);
        }

        public void drawScreen(object o, PaintEventArgs pea) 
        {
            //pea.Graphics.FillRectangle(new SolidBrush(Color.DarkGray), 0, 0, this.Width, this.Height);
            //kaart.draw(pea.Graphics, 0, 0);
            //pea.Graphics.DrawString(pos[0].ToString() + "," + pos[1].ToString(), font, Brushes.Black, 0, 0);
            int x = 200;
            int y = 200;
            Path.initializePaths(200);
           
            for (int j = 3; j < 6; j++) {
                Curve curve = Path.getCurve(j, (j + 3)%6);
                Point p1 = curve.go(0);
                for (double i = 1; i < curve.Length; i += 0.0001)
                {
                    Point p2 = curve.go(i);
                    pea.Graphics.DrawLine(Pens.Black, p1.X + x, p1.Y + y, p2.X + x, p2.Y + y);
                    p1 = p2;
                }
            }
            
        }
    }
}
