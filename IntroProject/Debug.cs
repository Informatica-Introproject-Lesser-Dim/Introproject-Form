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

    //this class is for when you need to test something that doesnt end up in the final code
    class Debug : Form
    {
        DebugScreen debugscr;

        public Debug() 
        {
            this.Size = new Size(800, 600);
            debugscr = new DebugScreen(800, 600);
            this.Controls.Add(debugscr);
        }
    }

    class DebugScreen : UserControl
    {
        Kaart kaart;

        public DebugScreen(int w, int h) 
        {
            this.Size = new Size(w, h);
            this.Paint += drawScreen;
            kaart = new Kaart(8, 5, 50, 10);
        }

        public void drawScreen(object o, PaintEventArgs pea) 
        {
            pea.Graphics.FillRectangle(new SolidBrush(Color.DarkGray), 0, 0, this.Width, this.Height);
            kaart.draw(pea.Graphics, 50, 50);
        }
    }
}