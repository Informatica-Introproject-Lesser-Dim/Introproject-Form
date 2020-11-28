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
            this.Size = new Size(800, 650);
            debugscr = new DebugScreen(800, 650);
            this.Controls.Add(debugscr);
        }
    }

    class DebugScreen : UserControl
    {
        Kaart kaart;
        int[] pos = new int[2] { 0, 0 };
        Font font = new Font("Arial", 12);

        public DebugScreen(int w, int h) 
        {
            this.Size = new Size(w, h);
            this.Paint += drawScreen;
            kaart = new Kaart(8, 5, 50, 10);
            this.MouseClick += Klik;
        }

        public void Klik(object o, MouseEventArgs mea) {
            pos = kaart.PosToHexPos(mea.X - 50, mea.Y - 100);
            this.Invalidate();
        }

        public void drawScreen(object o, PaintEventArgs pea) 
        {
            pea.Graphics.FillRectangle(new SolidBrush(Color.DarkGray), 0, 0, this.Width, this.Height);
            kaart.draw(pea.Graphics, 50, 100);
            pea.Graphics.DrawString(pos[0].ToString() + "," + pos[1].ToString(), font,Brushes.Black, 0, 0);
            Bitmap bm = new Bitmap(this.Width, this.Height);
            for (int x = 0; x < bm.Width; x++)
                for (int y = 0; y < bm.Height; y++) {
                    pos = kaart.PosToHexPos(x - 50, y - 100);
                    int r = 0;
                    if (pos[0] % 2 == 0)
                        r = 125;
                    int g = 0;
                    if (pos[1] % 2 == 0)
                        g = 125;
                    int a = 0;
                    if (r != 0 || g != 0)
                        a = 70;
                    Color c = Color.FromArgb(a, r, g, 0);
                    bm.SetPixel(x, y, c);
                }
            pea.Graphics.DrawImage(bm, 0, 0);
                    
        }
    }
}
