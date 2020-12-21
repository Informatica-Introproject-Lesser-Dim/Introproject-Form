using System;
using System.Drawing;
using System.Windows.Forms;

namespace IntroProject
{
    public partial class Screen : Form
    {
        MapScreen debugscr;
        DropMenu dropMenu;
        SettingsMenu settingsMenu;
        HelpMenu helpMenu;
        StatisticsMenu statisticsMenu;

        public Screen()
        {
            InitializeComponent();

            Button plus = new ButtonImaged(Properties.Resources.Plus_icon);
            plus.Size = new Size(50, 50);
            plus.Location = new Point(1800 - 66, 5);
            plus.Click += (object o, EventArgs ea) => { dropMenu.Show(); plus.Hide(); };
            this.Controls.Add(plus);

            this.Size = new Size(1800, 1200);
            debugscr = new MapScreen(this.Size);
            settingsMenu = new SettingsMenu(Size.Width, Size.Height);
            helpMenu = new HelpMenu(Size.Width, Size.Height);
            statisticsMenu = new StatisticsMenu(Size.Width, Size.Height);
            dropMenu = new DropMenu(Size.Width/10, Size.Height, 
                                   (object o, EventArgs ea) => { settingsMenu.Show(); settingsMenu.BringToFront(); }, 
                                   (object o, EventArgs ea) => { helpMenu.Show(); helpMenu.BringToFront(); }, 
                                   (object o, EventArgs ea) => { statisticsMenu.Show(); statisticsMenu.BringToFront(); },
                                    plus);
            dropMenu.Dock = DockStyle.Right;
            this.Controls.Add(dropMenu);
            this.Controls.Add(debugscr);
            this.Controls.Add(settingsMenu);
            this.Controls.Add(helpMenu);
            this.Controls.Add(statisticsMenu);
            dropMenu.Hide();
            settingsMenu.Hide();
            statisticsMenu.Hide();
            helpMenu.Hide();

            Resize += (object o, EventArgs ea) => 
            {
                int maxim = Math.Max((int)(Size.Width / 36), (int)(Size.Height / 36));
                debugscr.Size = new Size(Size.Width, Size.Height);
                dropMenu.Size = new Size(Size.Width / 10, Size.Height);
                settingsMenu.Size = new Size(Size.Width, Size.Height);
                helpMenu.Size = new Size(Size.Width, Size.Height);
                statisticsMenu.Size = new Size(Size.Width, Size.Height);
                plus.Location = new Point(Size.Width - maxim - 16, 5);
                plus.Size = new Size(maxim, maxim);
            };
        }
    }

    class MapScreen : UserControl
    {
        Map kaart;
        int[] pos = new int[2] { 0, 0 };
        Font font = new Font("Arial", 12);
        int n = 0;
        int xCam = 50;
        int yCam = 50;
        

        public MapScreen(Size size) : this(size.Width, size.Height) { }
        public MapScreen(int w, int h)
        {
            Button play = new ButtonImaged(Properties.Resources.Play_icon);
            Button stop = new ButtonImaged(Properties.Resources.Stop_icon);
            Button pause = new ButtonImaged(Properties.Resources.Pause_icon);

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
            kaart = new Map(50, 30, 20, 0);
            Path.initializePaths(20);
            for (int i = 0; i < 0; i++) {
                kaart.placeRandom(new Planten(-10, 0, 5));
            }

            for (int i = 0; i < 2; i++)
            {
                Herbivore herbivore = new Herbivore();
                herbivore.x = 0;
                kaart.placeRandom(herbivore);
                herbivore.calcFoodDist();
            }

            this.MouseClick += Klik;
            
        }

        public void Klik(object o, MouseEventArgs mea) {
            int[] adress = kaart.PosToHexPos(mea.X - 50, mea.Y - 50);
            this.Invalidate();
        }

        public void drawScreen(object o, PaintEventArgs pea) 
        {
            if (n > 4) {
                kaart.activateEntities();
                n = 0;

            }
            pea.Graphics.FillRectangle(new SolidBrush(Color.DarkGray), 0, 0, this.Width, this.Height);
            kaart.draw(pea.Graphics, xCam, yCam, this.Width, this.Height);
            pea.Graphics.DrawString(pos[0].ToString() + "," + pos[1].ToString(), font, Brushes.Black, 0, 0);
            n++;
            this.Invalidate();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handeleparam = base.CreateParams;
                handeleparam.ExStyle |= 0x02000000;
                return handeleparam;
            }
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
        public DropMenu(int w, int h, EventHandler settingsMenuOnClick, EventHandler helpMenuOnClick, EventHandler statisticsMenuOnClick, Button plus)
        {
            // FlatStyle to flat, and the colors of FlatAppearance MouseDownBackColor & MouseOverBackColor to transparent
            ButtonImaged settings = new ButtonImaged(Properties.Resources.Settings_icon);
            ButtonImaged statistics = new ButtonImaged(Properties.Resources.Graph_icon);
            ButtonImaged help = new ButtonImaged(Properties.Resources.Help_icon);
            ButtonImaged min = new ButtonImaged(Properties.Resources.Minus_icon);

            int edge = Math.Min((int)(Size.Width / 1.2), (int)Size.Height / 7);
            Size groot = new Size(edge, edge);
            int locY = (int)(Size.Height * .40);
            int marge = (int)(edge * .2);
            
            statistics.Location = new Point(Size.Width/10, locY);
            settings.Location = new Point(Size.Width / 10, locY + groot.Width + marge);
            help.Location = new Point(Size.Width / 10, locY + 2*groot.Width + 2 *marge);
            min.Location = new Point(5, 5);
            min.BringToFront();

            settings.Click += settingsMenuOnClick;
            statistics.Click += statisticsMenuOnClick;
            help.Click += helpMenuOnClick;
            min.Click += (object o, EventArgs ea) => { this.Hide(); plus.Show(); };

            this.Controls.Add(min);
            this.Controls.Add(statistics);
            this.Controls.Add(settings);
            this.Controls.Add(help);

            this.BackColor = Color.DimGray;
            this.Size = new Size(w, h);

            Resize += (object o, EventArgs ea) =>
            {
                edge = Math.Min((int)(Size.Width / 1.2), (int)Size.Height / 7);

                groot = new Size(edge, edge);
                statistics.Size = groot;
                settings.Size = groot;
                help.Size = groot;
                min.Size = groot / 3;
                
                locY = (int)(Size.Height * .40);
                marge = (int)(edge * .2);

                statistics.Location = new Point(Size.Width / 10, locY);
                settings.Location = new Point(Size.Width / 10, locY + groot.Width + marge);
                help.Location = new Point(Size.Width / 10, locY + 2*groot.Width + 2 * marge);
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

    class HelpMenu : UserControl
    {

        public HelpMenu(int w, int h)
        {
            this.BackColor = Color.FromArgb(88, 79, 37);
            this.Size = new Size(w, h);

            Button exit = new ButtonImaged(Properties.Resources.X_icon);
            exit.Location = new Point(2, 2);
            exit.Size = new Size(50, 50);
            exit.FlatAppearance.BorderColor = Color.FromArgb(0, 20, 99);
            exit.Click += (object o, EventArgs ea) => { this.Hide(); };

            this.Controls.Add(exit);
        }
    }

    class StatisticsMenu : UserControl
    {

        public StatisticsMenu(int w, int h)
        {
            this.BackColor = Color.FromArgb(4, 34, 8);
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
