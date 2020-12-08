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
        SettingsMenu settingsMenu;

        public Debug() 
        {
            Button plus = new Button();
            plus.BackgroundImage = Properties.Resources.Plus_icon;
            plus.BackgroundImageLayout = ImageLayout.Stretch;
            plus.Size = new Size(50, 50);
            plus.Location = new Point(1800 - 66, 5);
            plus.Click += (object o, EventArgs ea) => { dropMenu.Show(); plus.Hide(); };
            this.Controls.Add(plus);

            this.Size = new Size(1800, 1200);
            debugscr = new DebugScreen(1800, 1200);
            settingsMenu = new SettingsMenu(Size.Width, Size.Height);
            dropMenu = new DropMenu(Size.Width/10, Size.Height, settingsMenu, plus);
            dropMenu.Dock = DockStyle.Right;
            this.Controls.Add(debugscr);
            this.Controls.Add(dropMenu);
            this.Controls.Add(settingsMenu);
            plus.BringToFront();
            dropMenu.BringToFront();
            dropMenu.Hide();
            settingsMenu.BringToFront();
            settingsMenu.Hide();
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
            strip.Dock = DockStyle.Right;
            ToolStripTextBox box = new ToolStripTextBox();

            strip.Items.Add(box);

            play.BackgroundImage = Properties.Resources.Play_icon;
            play.BackgroundImageLayout = ImageLayout.Stretch;
            stop.BackgroundImage = Properties.Resources.Stop_icon;
            stop.BackgroundImageLayout = ImageLayout.Stretch;
            pause.BackgroundImage = Properties.Resources.Pause_icon;
            pause.BackgroundImageLayout = ImageLayout.Stretch;

            Size middel = new Size(60, 60);
            
            pause.Size = middel;
            play.Size = middel;
            stop.Size = middel;

            pause.Location = new Point(40, 5);
            play.Location = new Point(100, 5);
            stop.Location = new Point(160, 5);


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
        public DropMenu(int w, int h, SettingsMenu settingsMenu, Button plus)
        {
            // FlatStyle to flat, and the colors of FlatAppearance MouseDownBackColor & MouseOverBackColor to transparent
            Button settings = new Button();
            Button statistics = new Button();
            Button help = new Button();
            Button min = new Button();

            settings.BackgroundImage = Properties.Resources.Settings_icon;
            settings.BackgroundImageLayout = ImageLayout.Stretch;
            statistics.BackgroundImage = Properties.Resources.Graph_icon;
            statistics.BackgroundImageLayout = ImageLayout.Stretch;
            help.BackgroundImage = Properties.Resources.Help_icon;
            help.BackgroundImageLayout = ImageLayout.Stretch;
            min.BackgroundImage = Properties.Resources.Minus_icon;
            min.BackgroundImageLayout = ImageLayout.Stretch;

            Size groot = new Size(150, 150);
            
            statistics.Size = groot;
            settings.Size = groot;
            help.Size = groot;
            min.Size = new Size(50, 50);

            statistics.Padding = Padding.Empty;
            settings.Padding = Padding.Empty;
            help.Padding = Padding.Empty;
            min.Padding = Padding.Empty;

            statistics.FlatStyle = FlatStyle.Flat;
            settings.FlatStyle = FlatStyle.Flat;
            help.FlatStyle = FlatStyle.Flat;
            min.FlatStyle = FlatStyle.Flat;

            statistics.FlatAppearance.MouseDownBackColor = Color.Transparent;
            settings.FlatAppearance.MouseDownBackColor = Color.Transparent;
            help.FlatAppearance.MouseDownBackColor = Color.Transparent;
            min.FlatAppearance.MouseDownBackColor = Color.Transparent;

            statistics.FlatAppearance.MouseOverBackColor = Color.Transparent;
            settings.FlatAppearance.MouseOverBackColor = Color.Transparent;
            help.FlatAppearance.MouseOverBackColor = Color.Transparent;
            min.FlatAppearance.MouseOverBackColor = Color.Transparent;

            statistics.FlatAppearance.BorderColor = Color.DimGray;
            settings.FlatAppearance.BorderColor = Color.DimGray;
            help.FlatAppearance.BorderColor = Color.DimGray;
            min.FlatAppearance.BorderColor = Color.DimGray;


            statistics.Location = new Point(20, 550);
            settings.Location = new Point(20, 720);
            help.Location = new Point(20, 890);
            min.Location = new Point(5, 5);
            min.BringToFront();

            settings.Click += (object o, EventArgs ea) => { settingsMenu.Show(); };
            min.Click += (object o, EventArgs ea) => { this.Hide(); plus.Show(); };

            this.Controls.Add(settings);
            this.Controls.Add(statistics);
            this.Controls.Add(help);
            this.Controls.Add(min);

            this.BackColor = Color.DimGray;
            this.Size = new Size(w, h);
        }

    }

    class SettingsMenu : UserControl
    {
        public SettingsMenu(int w, int h)
        {
            this.BackColor = Color.FromArgb(0, 20, 99);
            this.Size = new Size(w, h);

            Button exit = new Button();
            exit.BackgroundImage = Properties.Resources.X_icon;
            exit.BackgroundImageLayout = ImageLayout.Stretch;
            exit.Location = new Point(2, 2);
            exit.Size = new Size(50, 50);
            exit.Padding = Padding.Empty;
            exit.FlatStyle = FlatStyle.Flat;
            exit.FlatAppearance.MouseDownBackColor = Color.Transparent;
            exit.FlatAppearance.MouseOverBackColor = Color.Transparent;
            exit.FlatAppearance.BorderColor = Color.FromArgb(0, 20, 99);
            exit.Click += (object o, EventArgs ea) => { this.Hide(); };

            this.Controls.Add(exit);
        }
    }
}
