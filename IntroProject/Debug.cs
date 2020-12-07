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
        DropMenu dropMenu;

        public Debug() 
        {
            this.Size = new Size(1800, 1200);
            debugscr = new DebugScreen(1800, 1200);
            dropMenu = new DropMenu(Size.Width/10, Size.Height);
            this.Controls.Add(debugscr);
            this.Controls.Add(dropMenu);
        }
    }

    class DebugScreen : UserControl
    {
        Map kaart;
        int[] pos = new int[2] { 0, 0 };
        Font font = new Font("Arial", 12);

        public DebugScreen(int w, int h)
        {
            ToolStrip strip = new ToolStrip();
            Button play = new Button();
            Button stop = new Button();
            Button pause = new Button();
            Button settings = new Button();
            Button statistics = new Button();
            Button exit = new Button();
            Button help = new Button();
            strip.Dock = DockStyle.Right;
            ToolStripTextBox box = new ToolStripTextBox();

            strip.Items.Add(box);

            play.BackgroundImage = Properties.Resources.Play_icon;
            play.BackgroundImageLayout = ImageLayout.Stretch;
            stop.BackgroundImage = Properties.Resources.Stop_icon;
            stop.BackgroundImageLayout = ImageLayout.Stretch;
            pause.BackgroundImage = Properties.Resources.Pause_icon;
            pause.BackgroundImageLayout = ImageLayout.Stretch;
            settings.BackgroundImage = Properties.Resources.Settings_icon;
            settings.BackgroundImageLayout = ImageLayout.Stretch;
            statistics.BackgroundImage = Properties.Resources.Graph_icon;
            statistics.BackgroundImageLayout = ImageLayout.Stretch;
            exit.BackgroundImage = Properties.Resources.X_icon;
            exit.BackgroundImageLayout = ImageLayout.Stretch;
            help.BackgroundImage = Properties.Resources.Help_icon;
            help.BackgroundImageLayout = ImageLayout.Stretch;


            pause.Size = new Size(40, 40);
            play.Size = new Size(40, 40);
            stop.Size = new Size(40, 40);


            pause.Location = new Point(40, 40);
            play.Location = new Point(80, 40);
            stop.Location = new Point(120, 40);

            pause.AutoSize = true;


            this.Controls.Add(pause);
            this.Controls.Add(play);
            this.Controls.Add(stop);

            this.Size = new Size(w, h);
            this.Paint += drawScreen;
            kaart = new Map(200, 150, 5, 0);
            this.MouseClick += Klik;
        }

        public void Klik(object o, MouseEventArgs mea) {
            int[] adress = kaart.PosToHexPos(mea.X - 50, mea.Y - 50);
        }

        public void drawScreen(object o, PaintEventArgs pea) 
        {
            pea.Graphics.FillRectangle(new SolidBrush(Color.DarkGray), 0, 0, this.Width, this.Height);
            kaart.draw(pea.Graphics, 50, 50);
            pea.Graphics.DrawString(pos[0].ToString() + "," + pos[1].ToString(), font,Brushes.Black, 0, 0);                  
        }
    }
    class DropMenu : UserControl
    {
        public DropMenu(int w, int h)
        {

        }

    }
}
