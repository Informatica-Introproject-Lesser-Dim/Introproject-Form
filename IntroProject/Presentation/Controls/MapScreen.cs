using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace IntroProject.Presentation.Controls
{
    public class MapScreen : UserControl
    {
        DateTime oldTime = DateTime.Now;
        TimeSpan dt;

        public Button plus = new ButtonImaged(Properties.Resources.Plus_icon);
        private Button play = new ButtonImaged(Properties.Resources.Play_icon);
        private Entity selected;
        private Map map;
        private Font font = new Font("Arial", 12);
        public bool paused = true, buttonPaused = true;
        bool camAutoMove = false;
        int[] pos = new int[2] { 0, 0 };
        int xCam = 0, yCam = 0, n = 0;
        public const int size = 35;

        public MapScreen(Size size) : this(size.Width, size.Height) { }

        public MapScreen(int w, int h)
        {
            Path.initializePaths(size); //THIS NEEDS TO BE CALLED BEFORE ANY CREATURES ARE MOVED OR PATHS CREATED ETC

            Button stop = new ButtonImaged(Properties.Resources.Stop_icon);
            Button pause = new ButtonImaged(Properties.Resources.Pause_icon);

            plus.Size = new Size(50, 50);
            plus.Location = new Point(1800 - 66, 5);
            this.Controls.Add(plus);

            Size method = new Size(60, 60);

            pause.Size = method;
            play.Size = method;
            stop.Size = method;

            pause.Location = new Point(40, 5);
            play.Location = new Point(40, 5);
            stop.Location = new Point(105, 5);

            play.Click += Play_Click;
            pause.Click += (object o, EventArgs ea) => { play.Show(); paused = true; buttonPaused = true; };
            stop.Click += (object o, EventArgs ea) => { play.Show(); paused = true; buttonPaused = true; play.Click += NewMap; play.Click -= Play_Click;  };

            Controls.Add(play);
            Controls.Add(pause);
            Controls.Add(stop);

            Size = new Size(w, h);
            Paint += drawScreen;

            MakeMap();

            MouseClick += MapClick;
        }

        public List<Statistics> GetStats() => map.statistics;

        private void Play_Click(object sender, EventArgs ea)
        {
            play.Hide();
            paused = false;
            buttonPaused = false;
            oldTime = DateTime.Now;
        }

        private void NewMap(object sender, EventArgs ea)
        {
            MakeMap();
            play.Click -= NewMap;
            play.Click += Play_Click;
            play.PerformClick();
        }

        public void MakeMap()
        {
            map = new Map(100, 70, size, 0);

            for (int i = 0; i < Settings.StartEntities; i++)
            {
                map.placeRandom(new Herbivore());
                map.placeRandom(new DeathPile(0,0,100));
                if (i % 3 == 0)
                    map.placeRandom(new Carnivore());
            }
        }

        public void MapClick(object o, MouseEventArgs mea)
        {
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
                    yCam = (int)Math.Min(0.5 * Size.Height, yCam + size * 0.5 * Hexagon.sqrt3);
                    break;
                case Keys.Down:
                    yCam = (int)Math.Max(0.5 * Size.Height - map.tiles[map.width - 1, map.height - 1].y, yCam - size * 0.5 * Hexagon.sqrt3);
                    break;
                case Keys.Left:
                    xCam = (int)Math.Min(0.5 * Size.Width, xCam + 1.5 * size);
                    break;
                case Keys.Right:
                    xCam = (int)Math.Max(0.5 * Size.Width - map.tiles[map.width - 1, map.height - 1].x, xCam - size * 1.5);
                    break;
            }
            camAutoMove = false;

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void drawScreen(object o, PaintEventArgs pea)
        {
            if (!paused)
            {
                dt = DateTime.Now - oldTime;
                oldTime += dt;
                double mil = Math.Min(dt.TotalMilliseconds, 100);
                map.TimeStep(mil);
            }

            pea.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(108, 116, 150)), 0, 0, Width, Height); // Is dit wel nodig, dat het elke keer wordt gedaan?
            if (camAutoMove)
            {
                xCam = Width / 2 - selected.GlobalLoc.x;
                yCam = Height / 2 - selected.GlobalLoc.y;
            }
            map.draw(pea.Graphics, xCam, yCam, Width, Height);
            n++;

            if (selected != null)
            {
                pea.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0, 0)), Width - 500, 100, 200, 100);
                pea.Graphics.DrawString(selected.GetType().ToString().Substring(13, selected.GetType().ToString().Length - 13) + "\n" +
                                        "Energy: " + Math.Round(selected.energyVal, 2) + "\nGender: " + selected.gender,
                                        font, new SolidBrush(Color.Black),
                                        Width - 490, 110);
            }

            int[] genders = map.countMalesAndFemales();

            pea.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0, 0)), 50, 200, 200, 100);
            pea.Graphics.DrawString("Male Count: " + genders[0].ToString() +
                                    "\nMales Born: " + map.malesAdded +
                                    "\nFemale Count: " + genders[1].ToString() +
                                    "\nFemales Born:" + map.femalesAdded
                                        , font, new SolidBrush(Color.Black),
                                        60, 210);
            Invalidate();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleparam = base.CreateParams;
                handleparam.ExStyle |= 0x02000000;
                return handleparam;
            }
        }

        public void UpdateVars(bool newMap)
        {
            Calculator.Update();
            if (newMap)
                MakeMap();
            else
                map.Update();
        }
    }
}
