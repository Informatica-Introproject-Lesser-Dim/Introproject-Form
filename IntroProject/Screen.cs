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


            this.Size = new Size(1800, 1200);
            debugscr = new MapScreen(this.Size);
            settingsMenu = new SettingsMenu(Size.Width, Size.Height, (object o, EventArgs ea) => { settingsMenu.Warning(); settingsMenu.Hide(); debugscr.clicked = false; });
            helpMenu = new HelpMenu(Size.Width, Size.Height, (object o, EventArgs ea) => { helpMenu.Hide(); debugscr.clicked = false; });
            statisticsMenu = new StatisticsMenu(Size.Width, Size.Height, (object o, EventArgs ea) => { statisticsMenu.Hide(); debugscr.clicked = false; });
            dropMenu = new DropMenu(Size.Width/10, Size.Height, 
                                   (object o, EventArgs ea) => { settingsMenu.Prep(); settingsMenu.Show(); settingsMenu.BringToFront(); debugscr.clicked = true; }, 
                                   (object o, EventArgs ea) => { helpMenu.Show(); helpMenu.BringToFront(); debugscr.clicked = true; }, 
                                   (object o, EventArgs ea) => { statisticsMenu.Show(); statisticsMenu.BringToFront(); debugscr.clicked = true; },          
                                    debugscr.plus);
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
                debugscr.plus.Location = new Point(Size.Width - maxim - 16, 5);
                debugscr.plus.Size = new Size(maxim, maxim);
            };

            debugscr.plus.Click += (object o, EventArgs ea) => { dropMenu.Show(); debugscr.plus.Hide(); };
        }
    }

    class MapScreen : UserControl
    {
        Map map;
        public bool clicked = false;
        bool camAutoMove = false;
        int[] pos = new int[2] { 0, 0 };
        Font font = new Font("Arial", 12);
        int n = 0;
        int xCam = 0;
        int yCam = 0;
        public const int size = 35;
        public Button plus = new ButtonImaged(Properties.Resources.Plus_icon);

        private Entity selected;


        public MapScreen(Size size) : this(size.Width, size.Height) { }
        public MapScreen(int w, int h)
        {
            Path.initializePaths(size); //THIS NEEDS TO BE CALLED BEFORE ANY CREATURES ARE MOVED OR PATHS CREATED ETC

            Button play = new ButtonImaged(Properties.Resources.Play_icon);
            Button stop = new ButtonImaged(Properties.Resources.Stop_icon);
            Button pause = new ButtonImaged(Properties.Resources.Pause_icon);

            plus.Size = new Size(50, 50);
            plus.Location = new Point(1800 - 66, 5);
            this.Controls.Add(plus);

            Size middel = new Size(60, 60);
            
            pause.Size = middel;
            play.Size = middel;
            stop.Size = middel;

            pause.Location = new Point(40, 5);
            play.Location = new Point(40, 5);
            stop.Location = new Point(105, 5);

            play.Click += (object o, EventArgs ea) => { play.Hide(); clicked = true; };
            pause.Click += (object o, EventArgs ea) => { play.Show(); clicked = false; };

            this.Controls.Add(play); 
            this.Controls.Add(pause);
            this.Controls.Add(stop);

            this.Size = new Size(w, h);
            this.Paint += drawScreen;

            map = new Map(100, 70, size, 0);
            
            for (int i = 0; i < 20; i++)
            {
                Herbivore herbivore = new Herbivore();
                herbivore.x = 0;
                map.placeRandom(herbivore);
                herbivore.calcFoodDist();
            }

            this.MouseClick += MapClick;
        }

        public void MapClick(object o, MouseEventArgs mea) {
            if (selected != null)
            { 
                selected.selected = false;
                camAutoMove = false;
            }
            selected = null;

            Entity newE = map.GetCreature(mea.X - xCam, mea.Y - yCam, 40);
            if (newE == null)
                return;
            
            newE.selected = true;
            selected = newE;
            camAutoMove = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData) {
                case Keys.Up:
                    this.yCam += (int) (size * (0.5*Hexagon.sqrt3));
                    break;
                case Keys.Down:
                    this.yCam -= (int)(size * (0.5 * Hexagon.sqrt3));
                    break;
                case Keys.Left:
                    this.xCam += (int) (size*(3.0/2));
                    break;
                case Keys.Right:
                    this.xCam -= (int) (size*(3.0/2));
                    break;
            }
            camAutoMove = false;

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void drawScreen(object o, PaintEventArgs pea) 
        {
            if (n > 4 && clicked == false) {
                map.TimeStep();
                n = 0;
            }
            pea.Graphics.FillRectangle(new SolidBrush(Color.DarkGray), 0, 0, this.Width, this.Height);
            if (camAutoMove)
            {
                xCam = this.Width / 2 - selected.GlobalLoc.X;
                yCam = this.Height / 2 - selected.GlobalLoc.Y;
            }
            map.draw(pea.Graphics, xCam, yCam, this.Width, this.Height);
            n++;
            
            if (selected != null)
            {
                pea.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0, 0)), 1500, 100, 200, 100);
                pea.Graphics.DrawString(selected.GetType().ToString().Substring(13, selected.GetType().ToString().Length - 13) + "\n" + 
                                        "Energy: " + Math.Round(selected.energyVal, 2), 
                                        font, new SolidBrush(Color.Black), 
                                        1510, 110);
            }
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
        public bool warned = true;
        private int TE,SE, MiT, MaT;
        private double SL;
        public SettingsMenu(int w, int h, EventHandler exitMenu) 

        {
            this.BackColor = Color.FromArgb(123, 156, 148);
            this.Size = new Size(w, h);

            Button exit = new ButtonImaged(Properties.Resources.X_icon);
            exit.Location = new Point(2, 2);
            exit.Size = new Size(50, 50);
            exit.FlatAppearance.BorderColor = Color.FromArgb(123, 156, 148);
            exit.Click += exitMenu;
            
            this.Controls.Add(exit);

            int n = 1, d = 10, c = 100, m = 1000; //Scale variables for ease in the transitions.


            /* Each triple sentence is an slider group, First the construnction, where after the TrackBar specific event and textbox equivelent handlers.
               Don't forget, slider values are integers. So there is a scale in every thing
               If the scale is a visual addition, don't forget to remove it from the trackbar value.
            */
            (TrackBar TrackBarSpeed, TextBox TextBoxSpeed, ToolTip ToolTipSpeed) = MakeSlider(40, 60, "Speed", 1, 25, 200, c, "bijbehorende uitleg");
            TrackBarSpeed.ValueChanged += (object o, EventArgs ea) => { Settings.StepSize = TrackBarSpeed.Value/c; };
            TextBoxSpeed.Leave += (object o, EventArgs ea) => { Settings.StepSize = double.Parse(TextBoxSpeed.Text); };

            (TrackBar TrackBarTotalEntities, TextBox TextBoxTotalEntities, ToolTip ToolTipTotalEntities) = MakeSlider(40, 140, "Total number of Entities", 20, 10, 50, 1, "bijbehorende uitleg");
            TrackBarTotalEntities.ValueChanged += (object o, EventArgs ea) => { Settings.TotalEntities = TrackBarTotalEntities.Value; };
            TextBoxTotalEntities.Leave += (object o, EventArgs ea) => { Settings.TotalEntities = int.Parse(TextBoxTotalEntities.Text); };

            (TrackBar TrackBarStartEntities, TextBox TextBoxStartEntities, ToolTip ToolTipSTartEntities) = MakeSlider(40, 220, "Begin number of Entities", 20, 10, 40, 1, "bijbehorende uitleg");
            TrackBarStartEntities.ValueChanged += (object o, EventArgs ea) => { Settings.StartEntities = TrackBarStartEntities.Value; };
            TextBoxStartEntities.Leave += (object o, EventArgs ea) => { Settings.StartEntities = int.Parse(TextBoxStartEntities.Text); };

            (TrackBar TrackBarHatchSpeed, TextBox TextBoxHatchSpeed, ToolTip ToolTipHatchSpeed) = MakeSlider(40, 300, "Hatch Speed", 20, 10, 100, 1, "bijbehorende uitleg");
            TrackBarHatchSpeed.ValueChanged += (object o, EventArgs ea) => { Settings.HatchSpeed = TrackBarHatchSpeed.Value; };
            TextBoxHatchSpeed.Leave += (object o, EventArgs ea) => { Settings.HatchSpeed = int.Parse(TextBoxHatchSpeed.Text); };

            (TrackBar TrackBarSeaLevelHeight, TextBox TextBoxSeaLevelHeight, ToolTip ToolTipSeaLevelHeight) = MakeSlider(40, 380, "Sea Level", 8, 0, 8, 1, "bijbehorende uitleg");
            TrackBarSeaLevelHeight.ValueChanged += (object o, EventArgs ea) => { Settings.SeaLevel = TrackBarSeaLevelHeight.Value; };
            TextBoxSeaLevelHeight.Leave += (object o, EventArgs ea) => { Settings.SeaLevel = double.Parse(TextBoxSeaLevelHeight.Text); };

            (TrackBar TrackBarMatingCost, TextBox TextBoxMatingCost, ToolTip ToolTipMatingCost) = MakeSlider(40, 500, "Mating Cost", 500, 100, 1000, n, "bijbehorende uitleg");
            TrackBarMatingCost.ValueChanged += (object o, EventArgs ea) => { Settings.MatingCost = TrackBarMatingCost.Value; };
            TextBoxMatingCost.Leave += (object o, EventArgs ea) => { Settings.MatingCost = int.Parse(TextBoxMatingCost.Text); };

            (TrackBar TrackBarGrassGrowth, TextBox TextBoxGrassGrowth, ToolTip ToolTipGrassGrowth) = MakeSlider(40, 580, "Grass Growth Speed", 200, 100, 500, n, "bijbehorende uitleg");
            TrackBarGrassGrowth.ValueChanged += (object o, EventArgs ea) => { Settings.GrassGrowth = TrackBarGrassGrowth.Value; };
            TextBoxGrassGrowth.Leave += (object o, EventArgs ea) => { Settings.GrassGrowth = int.Parse(TextBoxGrassGrowth.Text); };

            (TrackBar TrackBarGrassMaxFeed, TextBox TextBoxGrassMaxFeed, ToolTip ToolTipGrassMaxFeed) = MakeSlider(40, 660, "Grass Feed Scale", 1500, 1000, 3000, n, "bijbehorende uitleg");
            TrackBarGrassMaxFeed.ValueChanged += (object o, EventArgs ea) => { Settings.GrassMaxFeed = TrackBarGrassMaxFeed.Value; };
            TextBoxGrassMaxFeed.Leave += (object o, EventArgs ea) => { Settings.GrassMaxFeed = int.Parse(TextBoxGrassMaxFeed.Text); };

            (TrackBar TrackBarTemperatureMin, TextBox TextBoxTemperatureMin, ToolTip ToolTipTemperatureMin) = MakeSlider(500, 60, "Temperature Min", 15, 10, 20, n, "bijbehorende uitleg");
            TrackBarTemperatureMin.ValueChanged += (object o, EventArgs ea) => { Settings.MinTemp = TrackBarTemperatureMin.Value; };
            TextBoxTemperatureMin.Leave += (object o, EventArgs ea) => { Settings.MinTemp = int.Parse(TextBoxTemperatureMin.Text); };

            (TrackBar TrackBarTemperatureMax, TextBox TextBoxTemperatureMax, ToolTip ToolTipTemperatureMax) = MakeSlider(500, 140, "Temperature Max", 25, 20, 30, n, "bijbehorende uitleg");
            TrackBarTemperatureMax.ValueChanged += (object o, EventArgs ea) => { Settings.MaxTemp = TrackBarTemperatureMax.Value; };
            TextBoxTemperatureMax.Leave += (object o, EventArgs ea) => {  Settings.MaxTemp = int.Parse(TextBoxTemperatureMax.Text); };

            (TrackBar TrackBarWalkEnergy, TextBox TextBoxWalkEnergy, ToolTip ToolTipWalkEnergy) = MakeSlider(500, 220, "Walk Energy Cost", 1, 50, 300, c, "bijbehorende uitleg");
            TrackBarWalkEnergy.ValueChanged += (object o, EventArgs ea) => { Settings.WalkEnergy = TrackBarWalkEnergy.Value/c; };
            TextBoxWalkEnergy.Leave += (object o, EventArgs ea) => { Settings.WalkEnergy = float.Parse(TextBoxWalkEnergy.Text); };

            (TrackBar TrackBarJumpEnergy, TextBox TextBoxJumpEnergy, ToolTip ToolTipJumpEnergy) = MakeSlider(500, 300, "Jump Energy Cost", 1, 5000, 30000, 10000, "bijbehorende uitleg");
            TrackBarJumpEnergy.ValueChanged += (object o, EventArgs ea) => { Settings.JumpEnergy = TrackBarJumpEnergy.Value/10000; };
            TextBoxJumpEnergy.Leave += (object o, EventArgs ea) => { Settings.JumpEnergy = float.Parse(TextBoxJumpEnergy.Text); };

            (TrackBar TrackBarPassiveEnergy, TextBox TextBoxPassiveEnergy, ToolTip ToolTipPassiveEnergy) = MakeSlider(500, 380, "Passive Energy Cost", 1, 500000, 3000000, 1000000, "bijbehorende uitleg");
            TrackBarPassiveEnergy.ValueChanged += (object o, EventArgs ea) => { Settings.PassiveEnergy = TrackBarPassiveEnergy.Value/1000000; };
            TextBoxPassiveEnergy.Leave += (object o, EventArgs ea) => { Settings.PassiveEnergy = float.Parse(TextBoxPassiveEnergy.Text); };

            this.Controls.Add(exit);
        }
        private (TrackBar, TextBox, ToolTip) MakeSlider(int x, int y, String name, int basevalue, int minvalue, int maxvalue, int scale, String uitleg)
        {
            Size sliderTextBoxSize = new Size(30, 20);

            Label label = new Label();
            label.Size = new Size(200, 20);
            label.Text = name;
            label.ForeColor = Color.White;
            label.Location = new Point(x, y);
            label.AutoSize = false;

            TrackBar trackBar = new CustomTrackbar(x, (y+20), minvalue, maxvalue);
            TextBox textBox = new TextBox();
            textBox.BackColor = Color.FromArgb(123, 156, 148);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.ForeColor = Color.White;
            trackBar.Value = (basevalue * scale);
            textBox.Text = basevalue.ToString();
            trackBar.ValueChanged += (object o, EventArgs ea) => { textBox.Text = (Math.Round(trackBar.Value * (1.0/scale), 2)).ToString(); };
            textBox.Leave += (object o, EventArgs ea) => { trackBar.Value = Convert.ToInt32(double.Parse(textBox.Text) * scale); };
            textBox.Location = new Point((x+400), (y+30));
            textBox.Size = sliderTextBoxSize;
            trackBar.AutoSize = false;

            ToolTip toolTip = new ToolTip();
            toolTip.ToolTipIcon = ToolTipIcon.Info;
            toolTip.ToolTipTitle = name;
            toolTip.SetToolTip(label, uitleg);
            toolTip.SetToolTip(trackBar, uitleg);

            this.Controls.Add(trackBar);
            this.Controls.Add(textBox);
            this.Controls.Add(label);
            

            return (trackBar, textBox, toolTip);
        }
        public void Prep()
        {
            TE = Settings.TotalEntities;
            SE = Settings.StartEntities;
            MiT = Settings.MinTemp;
            MaT = Settings.MaxTemp;
            SL = Settings.SeaLevel;
        }
        public void Warning()
        {
            if (
            TE != Settings.TotalEntities ||
            SE != Settings.StartEntities ||
            MiT != Settings.MinTemp ||
            MaT != Settings.MaxTemp ||
            SL != Settings.SeaLevel)
            {
                MessageBox.Show("For the changes to take effect, restart is required. \n" +
                    "This is done by clicking the squared terminate button.", "Requires restart");
            }
        }
    }

    class HelpMenu : UserControl
    {

        public HelpMenu(int w, int h, EventHandler exitMenu)
        {
            this.BackColor = Color.FromArgb(123, 156, 148);
            this.Size = new Size(w, h);

            Button exit = new ButtonImaged(Properties.Resources.X_icon);
            exit.Location = new Point(2, 2);
            exit.Size = new Size(50, 50);
            exit.FlatAppearance.BorderColor = Color.FromArgb(123, 156, 148);
            exit.Click += exitMenu;

            this.Controls.Add(exit);
        }
    }

    class StatisticsMenu : UserControl
    {

        public StatisticsMenu(int w, int h, EventHandler exitMenu)
        {
            this.BackColor = Color.FromArgb(123, 156, 148);
            this.Size = new Size(w, h);

            Button exit = new ButtonImaged(Properties.Resources.X_icon);
            exit.Location = new Point(2, 2);
            exit.Size = new Size(50, 50);
            exit.FlatAppearance.BorderColor = Color.FromArgb(123, 156, 148);
            exit.Click += exitMenu;

            this.Controls.Add(exit);
        }
    }

    internal class CustomTrackbar : TrackBar
    {
        public CustomTrackbar(int x, int y, int min, int max)
        {
            Location = new Point(x, y);
            Padding = Padding.Empty;
            Cursor = Cursors.Hand;
            Size = new Size(400, 40);
            BackColor = Color.White;
            Minimum = min;
            Maximum = max;
            TickFrequency = (max - min) / 10;
            TickStyle = TickStyle.Both;
            SmallChange = 1;
            LargeChange = (max - min) / 10;
            BackColor = Color.FromArgb(123, 156, 148);
        }
    }
}
