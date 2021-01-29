using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


using IntroProject.Core.Utils;

namespace IntroProject.Presentation.Controls
{
    public class MapScreen : UserControl
    {
        DateTime oldTime = DateTime.Now;
        TimeSpan dt;

        MultipleLanguages multipleLanguages = new MultipleLanguages();

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

            MouseClick += MapClick;
        }

        private void Play_Click(object sender, EventArgs ea)
        {
            play.Hide();
            paused = false;
            buttonPaused = false;
            oldTime = DateTime.Now;
        }

        private void NewMap(object sender, EventArgs ea)
        {
            Cursor.Current = Cursors.WaitCursor;
            MakeMap();
            play.Click -= NewMap;
            play.Click += Play_Click;
            play.PerformClick();
        }

        public void MakeMap()
        {
            map = new Map(50, 50, size, 0);
            StatisticsValues.ClearStats();
            for (int i = 0; i < Settings.StartHerbivore; i++)
            {
                map.placeRandom(new Herbivore());
            }
            for (int i = 0; i < Settings.StartCarnivore; i++)
            {
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

            pea.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(108, 116, 150)), 0, 0, Width, Height);
            if (camAutoMove)
            {
                xCam = Width / 2 - selected.GlobalLoc.x;
                yCam = Height / 2 - selected.GlobalLoc.y;
            }
            if (map == null)
                return;
            map.draw(pea.Graphics, xCam, yCam, Width, Height);
            n++;

            if (selected != null)
            {
                pea.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0, 0)), Width - 500, 100, 200, 100);
                pea.Graphics.DrawString(multipleLanguages.DisplayText(selected.GetType().ToString()[13..]) +
                                        SummaryOverlayRow("Energy") + Math.Round(selected.energyVal, 2) +
                                        SummaryOverlayRow("Gender") + multipleLanguages.DisplayText("GenderN" + selected.gender.ToString()),
                                        font, new SolidBrush(Color.Black),
                                        Width - 490, 110);
            }

            int[] genders = map.countMalesAndFemales();
            int[] type = map.countHerbivoresAndCarnivores();

            Point SummaryOverlayPos = new Point(50, Height - 300);
            pea.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0, 0)), SummaryOverlayPos.X, SummaryOverlayPos.Y, 300, 170);
            pea.Graphics.DrawString(SummaryOverlayRow("HerbivoreAmount") + type[0].ToString() +
                                    SummaryOverlayRow("CarnivoreAmount") + type[1].ToString() +
                                    SummaryOverlayRow("MalesAmount") + genders[0].ToString() +
                                    SummaryOverlayRow("MalesBorn") + map.malesAdded +
                                    SummaryOverlayRow("FemalesAmount") + genders[1].ToString() +
                                    SummaryOverlayRow("FemalesBorn") + map.femalesAdded
                                    , font, new SolidBrush(Color.Black), SummaryOverlayPos.X + 10, SummaryOverlayPos.Y - 10);
            Invalidate();
        }

        private string SummaryOverlayRow(string field) =>
            '\n' + multipleLanguages.DisplayText(field) + ": ";


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
            if (newMap || map == null)
                MakeMap();
            else
                map.Update();
        }

    }
}
