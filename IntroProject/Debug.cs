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
            Button plus = new ButtonImaged(Properties.Resources.Plus_icon);
            plus.Size = new Size(50, 50);
            plus.Location = new Point(1800 - 66, 5);
            plus.Click += (object o, EventArgs ea) => { dropMenu.Show(); plus.Hide(); };
            this.Controls.Add(plus);

            this.Size = new Size(1800, 1200);
            debugscr = new DebugScreen(1800, 1200);
            settingsMenu = new SettingsMenu(Size.Width, Size.Height);
            dropMenu = new DropMenu(Size.Width/10, Size.Height, (object o, EventArgs ea) => { settingsMenu.Show(); settingsMenu.BringToFront(); }, plus);
            dropMenu.Dock = DockStyle.Right;
            this.Controls.Add(dropMenu);
            this.Controls.Add(debugscr);
            this.Controls.Add(settingsMenu);
            dropMenu.Hide();
            settingsMenu.Hide();

            Resize += (object o, EventArgs ea) => 
            {
                int maxim = Math.Max((int)(Size.Width / 36), (int)(Size.Height / 36));
                dropMenu.Size = new Size(Size.Width / 10, Size.Height);
                plus.Location = new Point(Size.Width - maxim - 16, 5);
                plus.Size = new Size(maxim, maxim);
            };
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
            Button play = new ButtonImaged(Properties.Resources.Play_icon);
            Button stop = new ButtonImaged(Properties.Resources.Stop_icon);
            Button pause = new ButtonImaged(Properties.Resources.Pause_icon);
            strip.Dock = DockStyle.Right;
            ToolStripTextBox box = new ToolStripTextBox();

            strip.Items.Add(box);

            Size middel = new Size(60, 60);
            
            pause.Size = middel;
            play.Size = middel;
            stop.Size = middel;

            pause.Location = new Point(40, 5);
            play.Location = new Point(40, 5);
            stop.Location = new Point(105, 5);

            play.Click += (object o, EventArgs ea) => { play.Hide(); };
            pause.Click += (object o, EventArgs ea) => { play.Show(); };

            this.Controls.Add(play); 
            this.Controls.Add(pause);
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
    internal class ButtonImaged : Button
    {
        public ButtonImaged(Image bgI)
        {
            BackgroundImage = bgI;
            BackgroundImageLayout = ImageLayout.Stretch;
            Padding = Padding.Empty;
            FlatStyle = FlatStyle.Flat;

            FlatAppearance.MouseDownBackColor = Color.Transparent;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }
    }

    class DropMenu : UserControl
    {
        public DropMenu(int w, int h, EventHandler settingsMenuOnClick, Button plus)
        {
            // FlatStyle to flat, and the colors of FlatAppearance MouseDownBackColor & MouseOverBackColor to transparent
            ButtonImaged settings = new ButtonImaged(Properties.Resources.Settings_icon);
            ButtonImaged statistics = new ButtonImaged(Properties.Resources.Graph_icon);
            ButtonImaged help = new ButtonImaged(Properties.Resources.Help_icon);
            ButtonImaged min = new ButtonImaged(Properties.Resources.Minus_icon);

            statistics.Location = new Point(Size.Width/10, (int) (Size.Height*.45));
            settings.Location = new Point(Size.Width / 10, (int)(Size.Height * .6));
            help.Location = new Point(Size.Width / 10, (int)(Size.Height * .75));
            min.Location = new Point(5, 5);
            min.BringToFront();

            settings.Click += settingsMenuOnClick;
            statistics.Click += (object o, EventArgs ea) => { }; //click events van statistics en help
            help.Click += (object o, EventArgs ea) => { };
            min.Click += (object o, EventArgs ea) => { this.Hide(); plus.Show(); };

            this.Controls.Add(min);
            this.Controls.Add(statistics);
            this.Controls.Add(settings);
            this.Controls.Add(help);

            this.BackColor = Color.DimGray;
            this.Size = new Size(w, h);

            Size groot = new Size((int)(Size.Width / 1.2), (int)(Size.Width / 1.2));

            Resize += (object o, EventArgs ea) =>
            {
                int edge = Math.Max((int)(Size.Width / 1.2), (int)Size.Height / 7);
                groot = new Size(edge, edge);
                statistics.Size = groot;
                settings.Size = groot;
                help.Size = groot;
                min.Size = groot / 3;

                statistics.Location = new Point(Size.Width / 10, (int)(Size.Height * .45));
                settings.Location = new Point(Size.Width / 10, (int)(Size.Height * .6));
                help.Location = new Point(Size.Width / 10, (int)(Size.Height * .75));
            };

        }
    }

    class SettingsMenu : UserControl
    {
        public SettingsMenu(int w, int h)
        {
            this.BackColor = Color.FromArgb(0, 20, 99);
            this.Size = new Size(w, h);

            Button exit = new ButtonImaged(Properties.Resources.X_icon);
            exit.Location = new Point(2, 2);
            exit.Size = new Size(50, 50);
            exit.FlatAppearance.BorderColor = Color.FromArgb(0, 20, 99);
            exit.Click += (object o, EventArgs ea) => { this.Hide(); };

            this.Controls.Add(exit);
        }
    }
}
