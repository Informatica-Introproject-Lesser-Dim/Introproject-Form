using System;
using System.IO;
using System.Security;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;


namespace IntroProject
{
    public partial class HexagonOfLife : Form
    {
        MapScreen mapscr;
        DropMenu dropMenu;
        SettingsMenu settingsMenu;
        HelpMenu helpMenu;
        StatisticsMenu statisticsMenu;

        public HexagonOfLife()
        {
            InitializeComponent();


            this.Size = new Size(1800, 1200);
            mapscr = new MapScreen(this.Size);
            settingsMenu = new SettingsMenu(Size.Width, Size.Height, (object o, EventArgs ea) => { settingsMenu.ImplementChanges(); mapscr.UpdateVars(); settingsMenu.Hide(); if (!mapscr.buttonPaused) mapscr.paused = false; });
            helpMenu = new HelpMenu(Size.Width, Size.Height, (object o, EventArgs ea) => { helpMenu.Hide(); if (!mapscr.buttonPaused) mapscr.paused = false; });
            statisticsMenu = new StatisticsMenu(Size.Width, Size.Height, (object o, EventArgs ea) => { statisticsMenu.Hide(); if (!mapscr.buttonPaused) mapscr.paused = false; });
            dropMenu = new DropMenu(Size.Width/10, Size.Height, 
                                   (object o, EventArgs ea) => { settingsMenu.Show(); settingsMenu.BringToFront(); mapscr.paused = true; }, 
                                   (object o, EventArgs ea) => { helpMenu.Show(); helpMenu.BringToFront(); mapscr.paused = true; }, 
                                   (object o, EventArgs ea) => { statisticsMenu.Show(); statisticsMenu.BringToFront(); mapscr.paused = true; },          
                                    mapscr.plus);
            dropMenu.Dock = DockStyle.Right;
            this.Controls.Add(dropMenu);
            this.Controls.Add(mapscr);
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
                mapscr.Size = new Size(Size.Width, Size.Height);
                dropMenu.Size = new Size(Size.Width / 10, Size.Height);
                settingsMenu.Size = new Size(Size.Width, Size.Height);
                helpMenu.Size = new Size(Size.Width, Size.Height);
                statisticsMenu.Size = new Size(Size.Width, Size.Height);
                mapscr.plus.Location = new Point(Size.Width - maxim - 16, 5);
                mapscr.plus.Size = new Size(maxim, maxim);
            };

            mapscr.plus.Click += (object o, EventArgs ea) => { dropMenu.Show(); mapscr.plus.Hide(); };
        }
    }

    class MapScreen : UserControl
    {
        Map map;

        public bool paused = true;
        public bool buttonPaused = true;
        bool camAutoMove = false;
        int[] pos = new int[2] { 0, 0 };
        Font font = new Font("Arial", 12);
        int n = 0;
        int xCam = 0;
        int yCam = 0;
        public const int size = 35;
        public Button plus = new ButtonImaged(Properties.Resources.Plus_icon);

        private Entity selected;

        DateTime oldTime = DateTime.Now;
        TimeSpan dt;


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

            play.Click += (object o, EventArgs ea) => { play.Hide(); paused = false; buttonPaused = false; oldTime = DateTime.Now; };
            pause.Click += (object o, EventArgs ea) => { play.Show(); paused = true; buttonPaused = true; };

            this.Controls.Add(play); 
            this.Controls.Add(pause);
            this.Controls.Add(stop);

            this.Size = new Size(w, h);
            this.Paint += drawScreen;

            map = new Map(100, 70, size, 0);
            
            for (int i = 0; i < Settings.StartEntities; i++)
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
            switch (keyData)
            {
                case Keys.Up:
                    if (this.yCam + (size * (0.5 * Hexagon.sqrt3)) >= 0.5 * Size.Height)
                        this.yCam = (int)(0.5 * Size.Height);
                    else
                        this.yCam += (int)(size * (0.5 * Hexagon.sqrt3));
                    break;
                
                case Keys.Down:
                    if (this.yCam - (size * 0.5 * Hexagon.sqrt3) <= -(map.tiles[map.width - 1, map.height - 1].y - 0.5 * Size.Height))
                        this.yCam = (int)-(map.tiles[map.width - 1, map.height - 1].y - 0.5 * Size.Height);
                    else
                        this.yCam -= (int)(size * (0.5 * Hexagon.sqrt3));
                    break;
                
                case Keys.Left:
                    if (this.xCam + (size * (3.0 / 2)) >= 0.5 * Size.Width)
                        this.xCam = (int)(0.5 * Size.Width);
                    else
                        this.xCam += (int)(size * (3.0 / 2));
                    break;
                
                case Keys.Right:
                    if (this.xCam - (size * (3.0 / 2)) <= -(map.tiles[map.width - 1, map.height - 1].x - 0.5 * Size.Width))
                        this.xCam = (int)-(map.tiles[map.width - 1, map.height - 1].x - 0.5 * Size.Width);
                    else
                        this.xCam -= (int)(size * (3.0 / 2));
                    break;
            }
            camAutoMove = false;

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void drawScreen(object o, PaintEventArgs pea) 
        {
            if (paused == false) {
                dt = DateTime.Now - oldTime;
                oldTime = oldTime + dt;
                map.TimeStep(dt.TotalMilliseconds);
            }
            pea.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(108, 116, 150)), 0, 0, this.Width, this.Height); // Is dit wel nodig, dat het elke keer wordt gedaan?
            if (camAutoMove)
            {
                xCam = this.Width / 2 - selected.GlobalLoc.X;
                yCam = this.Height / 2 - selected.GlobalLoc.Y;
            }
            map.draw(pea.Graphics, xCam, yCam, this.Width, this.Height);
            n++;
            
            if (selected != null)
            {
                pea.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0, 0)), this.Width - 500, 100, 200, 100);
                pea.Graphics.DrawString(selected.GetType().ToString().Substring(13, selected.GetType().ToString().Length - 13) + "\n" + 
                                        "Energy: " + Math.Round(selected.energyVal, 2), 
                                        font, new SolidBrush(Color.Black),
                                        this.Width - 490, 110);
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
        public void UpdateVars()
        {
            Calculator.Update();
            //In clicked Stop Button method, make new map. With new variable values that require restart.

            //Here Seperate update for current map...
            map.Update();
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
        private int TE, SE, HS, MiT, MaT, GG, GMF, LI;
        private float Sp, MH, MC, WE, JE, PE;
        private bool changed, HM;
        private OpenFileDialog openFileDialog;
        private List<TrackBar> trackBars;
        public SettingsMenu(int w, int h, EventHandler exitMenu) 

        {
            this.BackColor = Color.FromArgb(123, 156, 148);
            this.Size = new Size(w, h);
            int edge = Math.Max((int)(Size.Width / 36), (int)(Size.Height / 36));

            Button exit = new ButtonImaged(Properties.Resources.X_icon);
            exit.Location = new Point(2, 2);
            exit.Size = new Size(edge, edge);
            exit.FlatAppearance.BorderColor = Color.FromArgb(123, 156, 148);
            exit.Click += exitMenu;

            Button importSet = new Button();
            importSet.Location = new Point(Size.Width - (edge * 2) - 16, 5);
            importSet.Size = new Size(edge * 2, edge * 2);
            importSet.FlatAppearance.BorderColor = Color.FromArgb(123, 156, 148);
            importSet.BackColor = Color.White;
            importSet.Text = "Import Settings";
            importSet.Click += ImportSettings;

            this.Controls.Add(exit);
            this.Controls.Add(importSet);

            Resize += (object o, EventArgs ea) =>
            {
                edge = Math.Max((int)(Size.Width / 36), (int)(Size.Height / 36));
                exit.Size = new Size(edge, edge);
                importSet.Size = new Size(edge * 2, edge * 2);
                importSet.Location = new Point(Size.Width - (edge * 2) - 16, 5);
            };
            /* Each triple sentence is an slider group, First the construnction, where after the TrackBar specific event and textbox equivelent handlers.
               Don't forget, slider values are integers. So there is a scale in every thing
               If the scale is a visual addition, don't forget to remove it from the trackbar value.
            */
            (TrackBar TrackBarSpeed, TextBox TextBoxSpeed, ToolTip ToolTipSpeed) = MakeSlider(40, 60, "Speed", 1, 25, 400, 100, "bijbehorende uitleg");
            TrackBarSpeed.ValueChanged += (object o, EventArgs ea) => { Sp = TrackBarSpeed.Value / 100f; };
            TextBoxSpeed.Leave += (object o, EventArgs ea) => { Sp = float.Parse(TextBoxSpeed.Text); };
            trackBars.Add(TrackBarSpeed); //0

            (TrackBar TrackBarTotalEntities, TextBox TextBoxTotalEntities, ToolTip ToolTipTotalEntities) = MakeSlider(40, 140, "Total number of Entities", 50, 40, 100, 1, "bijbehorende uitleg");
            TrackBarTotalEntities.ValueChanged += (object o, EventArgs ea) => { TE = TrackBarTotalEntities.Value; };
            TextBoxTotalEntities.Leave += (object o, EventArgs ea) => { TE = int.Parse(TextBoxTotalEntities.Text); };
            trackBars.Add(TrackBarTotalEntities); //1

            (TrackBar TrackBarStartEntities, TextBox TextBoxStartEntities, ToolTip ToolTipSTartEntities) = MakeSlider(40, 220, "Begin number of Entities", 20, 10, 40, 1, "bijbehorende uitleg");
            TrackBarStartEntities.ValueChanged += (object o, EventArgs ea) => { SE = TrackBarStartEntities.Value; };
            TextBoxStartEntities.Leave += (object o, EventArgs ea) => { SE = int.Parse(TextBoxStartEntities.Text); };
            trackBars.Add(TrackBarStartEntities); //2

            (TrackBar TrackBarHatchSpeed, TextBox TextBoxHatchSpeed, ToolTip ToolTipHatchSpeed) = MakeSlider(40, 300, "Hatch Speed", 20, 10, 100, 1, "bijbehorende uitleg");
            TrackBarHatchSpeed.ValueChanged += (object o, EventArgs ea) => { HS = TrackBarHatchSpeed.Value; };
            TextBoxHatchSpeed.Leave += (object o, EventArgs ea) => { HS = int.Parse(TextBoxHatchSpeed.Text); };
            trackBars.Add(TrackBarHatchSpeed); //3

            (TrackBar TrackBarMiddleHeight, TextBox TextBoxMiddleHeight, ToolTip ToolTipMiddleHeight) = MakeSlider(40, 380, "Sea Level", 0.5f, 0, 99, 100, "bijbehorende uitleg");
            TrackBarMiddleHeight.ValueChanged += (object o, EventArgs ea) => { MH = (TrackBarMiddleHeight.Value/50f) - 0.99f; };
            TextBoxMiddleHeight.Leave += (object o, EventArgs ea) => { MH = float.Parse(TextBoxMiddleHeight.Text) - 0.99f; };
            trackBars.Add(TrackBarMiddleHeight); //4

            (TrackBar TrackBarMatingCost, TextBox TextBoxMatingCost, ToolTip ToolTipMatingCost) = MakeSlider(40, 500, "Mating Cost", .75f, 20, 150, 100, "bijbehorende uitleg");
            TrackBarMatingCost.ValueChanged += (object o, EventArgs ea) => { MC = TrackBarMatingCost.Value / 100f; };
            TextBoxMatingCost.Leave += (object o, EventArgs ea) => { MC = float.Parse(TextBoxMatingCost.Text); };
            trackBars.Add(TrackBarMatingCost); //5

            (TrackBar TrackBarGrassGrowth, TextBox TextBoxGrassGrowth, ToolTip ToolTipGrassGrowth) = MakeSlider(40, 580, "Grass Growth Speed", 200, 100, 500, 1, "bijbehorende uitleg");
            TrackBarGrassGrowth.ValueChanged += (object o, EventArgs ea) => { GG = TrackBarGrassGrowth.Value; };
            TextBoxGrassGrowth.Leave += (object o, EventArgs ea) => { GG = int.Parse(TextBoxGrassGrowth.Text); };
            trackBars.Add(TrackBarGrassGrowth); //6

            (TrackBar TrackBarGrassMaxFeed, TextBox TextBoxGrassMaxFeed, ToolTip ToolTipGrassMaxFeed) = MakeSlider(40, 660, "Grass Feed Max", 1500, 1000, 3000, 1, "bijbehorende uitleg");
            TrackBarGrassMaxFeed.ValueChanged += (object o, EventArgs ea) => { GMF = TrackBarGrassMaxFeed.Value; };
            TextBoxGrassMaxFeed.Leave += (object o, EventArgs ea) => { GMF = int.Parse(TextBoxGrassMaxFeed.Text); };
            trackBars.Add(TrackBarGrassMaxFeed); //7

            (TrackBar TrackBarTemperatureMin, TextBox TextBoxTemperatureMin, ToolTip ToolTipTemperatureMin) = MakeSlider(500, 60, "Temperature Min", 15, 10, 20, 1, "bijbehorende uitleg");
            TrackBarTemperatureMin.ValueChanged += (object o, EventArgs ea) => { MiT = TrackBarTemperatureMin.Value; };
            TextBoxTemperatureMin.Leave += (object o, EventArgs ea) => { MiT = int.Parse(TextBoxTemperatureMin.Text); };
            trackBars.Add(TrackBarTemperatureMin); //8

            (TrackBar TrackBarTemperatureMax, TextBox TextBoxTemperatureMax, ToolTip ToolTipTemperatureMax) = MakeSlider(500, 140, "Temperature Max", 25, 20, 30, 1, "bijbehorende uitleg");
            TrackBarTemperatureMax.ValueChanged += (object o, EventArgs ea) => { MaT = TrackBarTemperatureMax.Value; };
            TextBoxTemperatureMax.Leave += (object o, EventArgs ea) => {  MaT = int.Parse(TextBoxTemperatureMax.Text); };
            trackBars.Add(TrackBarTemperatureMax); //9

            (TrackBar TrackBarWalkEnergy, TextBox TextBoxWalkEnergy, ToolTip ToolTipWalkEnergy) = MakeSlider(500, 220, "Walk Energy Cost", 1, 5, 30, 10, "bijbehorende uitleg");
            TrackBarWalkEnergy.ValueChanged += (object o, EventArgs ea) => { WE = TrackBarWalkEnergy.Value / 100f; };
            TextBoxWalkEnergy.Leave += (object o, EventArgs ea) => { WE = float.Parse(TextBoxWalkEnergy.Text); };
            trackBars.Add(TrackBarWalkEnergy); //10

            (TrackBar TrackBarJumpEnergy, TextBox TextBoxJumpEnergy, ToolTip ToolTipJumpEnergy) = MakeSlider(500, 300, "Jump Energy Cost", 1, 5, 30, 10, "bijbehorende uitleg");
            TrackBarJumpEnergy.ValueChanged += (object o, EventArgs ea) => { JE = TrackBarJumpEnergy.Value / 10000f; };
            TextBoxJumpEnergy.Leave += (object o, EventArgs ea) => { JE = float.Parse(TextBoxJumpEnergy.Text); };
            trackBars.Add(TrackBarJumpEnergy); //11

            (TrackBar TrackBarPassiveEnergy, TextBox TextBoxPassiveEnergy, ToolTip ToolTipPassiveEnergy) = MakeSlider(500, 380, "Passive Energy Cost", 1, 5, 30, 10, "bijbehorende uitleg");
            TrackBarPassiveEnergy.ValueChanged += (object o, EventArgs ea) => { PE = TrackBarPassiveEnergy.Value / 1000000f; };
            TextBoxPassiveEnergy.Leave += (object o, EventArgs ea) => { PE = float.Parse(TextBoxPassiveEnergy.Text); };
            trackBars.Add(TrackBarPassiveEnergy); //12

            this.Controls.Add(exit);
        }
        private (TrackBar, TextBox, ToolTip) MakeSlider(int x, int y, String name, float basevalue, int minvalue, int maxvalue, int scale, String uitleg)
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
            trackBar.Value = (int) (basevalue * scale);
            textBox.Text = basevalue.ToString();
            trackBar.ValueChanged += (object o, EventArgs ea) => { textBox.Text = (Math.Round(trackBar.Value * (1.0/scale), 2)).ToString(); changed = true; };
            textBox.Leave += (object o, EventArgs ea) => { trackBar.Value = Convert.ToInt32(double.Parse(textBox.Text) * scale); changed = true; };
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
        public void ImportSettings(object o, EventArgs ea)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamReader sr = new StreamReader(openFileDialog.FileName);

                    string[] settings = new string[14];
                    string lineInfo;
                    int i = 0;

                    while ((lineInfo = sr.ReadLine()) != null)
                    {
                        settings[i] = lineInfo;
                        i++;
                    }
                    sr.Close();
                    import(settings);
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void import(string[] settings)
        {

        }
        public void ImplementChanges()
        {
            if (changed)
            {
                DialogResult implement = MessageBox.Show("Some Settings have been changed. \n" +
                    "Are you sure you want to implement these changes?", "Implement Changes?", MessageBoxButtons.YesNo);
                if (implement == DialogResult.Yes)
                {
                    ImplementSettings();
                    if (
                    TE != Settings.TotalEntities ||
                    SE != Settings.StartEntities ||
                    MH != Settings.MiddleHeight ||
                    MiT != Settings.MinTemp ||
                    MaT != Settings.MaxTemp)
                    {
                        DialogResult restart = MessageBox.Show("Some of these Settings require a restart. \n" +
                            "Are you sure you want to implement these changes?", "Restart?", MessageBoxButtons.YesNo);
                        if (restart == DialogResult.Yes)
                        {
                            Settings.TotalEntities = TE;
                            Settings.StartEntities = SE;
                            Settings.MiddleHeight = MH;
                            Settings.MinTemp = MiT;
                            Settings.MaxTemp = MaT;
                            //implement restart function with new values
                        }
                        else if (restart == DialogResult.No)
                        {
                            trackBars[1].Value = Settings.TotalEntities;
                            trackBars[2].Value = Settings.StartEntities;
                            trackBars[4].Value = (int) Settings.MiddleHeight * 100;
                            trackBars[8].Value = Settings.MinTemp;
                            trackBars[9].Value = Settings.MaxTemp;
                        }
                    }
                }
                else if (implement == DialogResult.No)
                {
                    RevertSettings();
                }
            }
        }
        private void ImplementSettings()
        {
            Settings.StepSize = Sp;
            Settings.HatchSpeed = HS;
            Settings.MatingCost = MC;
            Settings.GrassGrowth = GG;
            Settings.GrassMaxFeed = GMF;
            Settings.WalkEnergy = WE;
            Settings.JumpEnergy = JE;
            Settings.PassiveEnergy = PE;
            Settings.AddHeatMap = HM;
            Settings.LanguageIndex = LI;
        }
        private void RevertSettings()
        {
            trackBars[0].Value = (int) Settings.StepSize * 100;
            trackBars[1].Value = Settings.TotalEntities;
            trackBars[2].Value = Settings.StartEntities;
            trackBars[3].Value = Settings.HatchSpeed;
            trackBars[4].Value = (int) Settings.MiddleHeight * 100;
            trackBars[5].Value = (int) Settings.MatingCost * 100;
            trackBars[6].Value = Settings.GrassGrowth;
            trackBars[7].Value = Settings.GrassMaxFeed;
            trackBars[8].Value = Settings.MinTemp;
            trackBars[9].Value = Settings.MaxTemp;
            trackBars[10].Value = (int) Settings.WalkEnergy * 10;
            trackBars[11].Value = (int) Settings.JumpEnergy * 10;
            trackBars[12].Value = (int) Settings.PassiveEnergy * 10;
            // Settings.AddHeatMap = HM; MOET HIER NOG INPUT VOOR MAKEN
            // Settings.LanguageIndex = LI; MOET HIER NOG INPUT VOOR MAKEN
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
        int r = 0;
        public StatisticsMenu(int w, int h, EventHandler exitMenu)
        {
            this.BackColor = Color.FromArgb(123, 156, 148);
            this.Size = new Size(w, h);
            int edge = Math.Max((int)(Size.Width / 36), (int)(Size.Height / 36));

            Button exit = new ButtonImaged(Properties.Resources.X_icon);
            exit.Location = new Point(2, 2);
            exit.Size = new Size(edge, edge);
            exit.FlatAppearance.BorderColor = Color.FromArgb(123, 156, 148);
            exit.Click += exitMenu;
            this.Controls.Add(exit);

            Resize += (object o, EventArgs ea) =>
            {
                edge = Math.Max((int)(Size.Width / 36), (int)(Size.Height / 36));
                exit.Size = new Size(edge, edge);
            };
            Button statname1 = ButtonList("statistics");
            Button statname2 = ButtonList("statistics");
            Button statname3 = ButtonList("statistics");
            Button statname4 = ButtonList("statistics");
            Button statname5 = ButtonList("statistics");
            Button statname6 = ButtonList("statistics");

        }

        private Button ButtonList(String name) //makes a button and next call makes a button under the previus.
        {
            r++;
            int i = r, y = Math.Min((int)Size.Width / 30, (int)Size.Height / 20) + 10;
            Button button = new Button();
            button.Text = name;
            button.Location = new Point(0, i*y);
            button.Size = new Size(this.Width / 12, this.Height / 20);
            button.BackColor = Color.White;

            Resize += (object o, EventArgs ea) =>
            {
                y = Math.Min((int)Size.Width / 30, (int)Size.Height / 20) + 10;

                button.Location = new Point(0, i*y);
                button.Size = new Size(this.Width / 12, this.Height / 20);
            };

            this.Controls.Add(button);
            return button;
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
