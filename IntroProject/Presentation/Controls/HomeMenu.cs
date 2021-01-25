﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace IntroProject.Presentation.Controls
{
    class HomeMenu : UserControl
    {
        public int hexagonSize;
        EventHandler _start, _settingStart, _preSet, _fullScreen, _help;
        public HomeMenu(int w, int h, EventHandler start, EventHandler settingStart, EventHandler preSet, EventHandler fullScreen, EventHandler help)
        {
            this.Size = new Size(w, h);
            hexagonSize = (int)(Size.Height / 8);
            _start = start;
            _settingStart = settingStart;
            _preSet = preSet;
            _fullScreen = fullScreen;
            _help = help;

            createAllLabels();
            createAllButtons();

            BackgroundImage = Properties.Resources.Background_blurred;
            BackgroundImageLayout = ImageLayout.Tile;
        }

        public void createAllButtons()
        {
            int[] menuButtonLocations = createMenuButtonLocations();
            Point[] buttonLocations = ButtonLocations(menuButtonLocations);
            Button startHexagonBt = LoadButton(buttonLocations[0] + new Size(1,1), hexagonSize + 2);
            startHexagonBt.BackColor = Color.Brown;
            Button exitHexagonBt = LoadButton(buttonLocations[1], hexagonSize);
            Button settingsHexagonBt = LoadButton(buttonLocations[2], hexagonSize);
            Button runPresetHexagonBt = LoadButton(buttonLocations[3], hexagonSize);
            Button languageHexagonBt = LoadButton(buttonLocations[4], hexagonSize);
            Button fullScreenHexagonBt = LoadButton(buttonLocations[5], hexagonSize);
            Button helpHexagonBt = LoadButton(buttonLocations[6], hexagonSize);


            Resize += (object sender, EventArgs h) =>
            {
                menuButtonLocations = createMenuButtonLocations();
                buttonLocations = ButtonLocations(menuButtonLocations);
                ButtonLocationChange(startHexagonBt, buttonLocations[0] + new Size(1, 1), hexagonSize + 2);
                ButtonLocationChange(exitHexagonBt, buttonLocations[1], hexagonSize);
                ButtonLocationChange(settingsHexagonBt, buttonLocations[2], hexagonSize);
                ButtonLocationChange(runPresetHexagonBt, buttonLocations[3], hexagonSize);
                ButtonLocationChange(languageHexagonBt, buttonLocations[4], hexagonSize);
                ButtonLocationChange(fullScreenHexagonBt, buttonLocations[5], hexagonSize);
                ButtonLocationChange(helpHexagonBt, buttonLocations[6], hexagonSize);
            };

            startHexagonBt.Click += _start;
            exitHexagonBt.Click += exitBTPressed;
            settingsHexagonBt.Click += _settingStart;
            runPresetHexagonBt.Click += _preSet;
            languageHexagonBt.Click += languageBTPressed;
            fullScreenHexagonBt.Click += _fullScreen;
            helpHexagonBt.Click += _help;

        }
        Button LoadButton(Point hexagonLocation, int currentHexagonSize)
        {
            Button hexagonButton = new Button();
            ButtonLocationChange(hexagonButton, hexagonLocation, currentHexagonSize);
            hexagonButton.BackColor = Color.FromArgb(30, 30, 30);

            Controls.Add(hexagonButton);

            return hexagonButton;
        }
        public Point[] HexagonPoints(int centreX, int centreY, int size)
        {
            Point[] hexagonPoints = new Point[6];
            Rectangle boundBy = new Rectangle((int)(centreX - size * 2.6 / 3), (int)(centreY - size), (int)((size * 2.6 / 3) * 2), (int)(size * 2));
            int quartWidth = boundBy.Width / 4;
            int halfHeight = boundBy.Height / 2;
            hexagonPoints[0] = new Point(boundBy.Left + quartWidth, boundBy.Top);
            hexagonPoints[1] = new Point(boundBy.Right - quartWidth, boundBy.Top);
            hexagonPoints[2] = new Point(boundBy.Right, boundBy.Top + halfHeight);
            hexagonPoints[3] = new Point(boundBy.Right - quartWidth, boundBy.Bottom);
            hexagonPoints[4] = new Point(boundBy.Left + quartWidth, boundBy.Bottom);
            hexagonPoints[5] = new Point(boundBy.Left, boundBy.Top + halfHeight);
            return hexagonPoints;
        }
        private void exitBTPressed(object sender, EventArgs e) => Application.Exit();

        private void languageBTPressed(object sender, EventArgs e)
        {
            Settings.LanguageIndex = Settings.LanguageIndex == 0 ? 1 : 0;
            Refresh();
        }
        private Point[] ButtonLocations(int[] primitivePoints)
        {
            Point[] buttonPoints = new Point[7];
            buttonPoints[0] = new Point(primitivePoints[0], primitivePoints[1]);
            buttonPoints[1] = new Point(primitivePoints[2], primitivePoints[3]);
            buttonPoints[2] = new Point(primitivePoints[4], primitivePoints[5]);
            buttonPoints[3] = new Point(primitivePoints[6], primitivePoints[7]);
            buttonPoints[4] = new Point(primitivePoints[8], primitivePoints[9]);
            buttonPoints[5] = new Point(primitivePoints[10], primitivePoints[11]);
            buttonPoints[6] = new Point(primitivePoints[12], primitivePoints[13]);

            return buttonPoints;
        }
        private void ButtonLocationChange(Button hexagonButton, Point hexagonLocation, int currentHexagonSize)
        {
            Point[] hexagonPoints = HexagonPoints(hexagonLocation.X, hexagonLocation.Y, currentHexagonSize);
            GraphicsPath hexagonPath = new GraphicsPath(FillMode.Winding);
            hexagonPath.AddPolygon(hexagonPoints);
            Region hexagonRegion = new Region(hexagonPath);
            hexagonButton.SetBounds(hexagonPoints[0].X - 5, hexagonPoints[0].Y - 5, hexagonPoints[2].X + 5, hexagonPoints[3].Y + 5);
            hexagonButton.Region = hexagonRegion;

        }
        private int[] createMenuButtonLocations()
        {
            int[] res = {
                          (int)(Size.Width / 4 + 3 * hexagonSize * 2.6 / 24), Size.Height / 4 + hexagonSize / 2,
                          (int)(Size.Width / 4 + 3 * hexagonSize * 2.6 / 24), Size.Height / 4 + hexagonSize + hexagonSize / 2,
                          (int)(Size.Width / 4 + 3 * hexagonSize * 2.6 / 12 + 3 * hexagonSize * 2.6 / 24) , (int)(Size.Height / 4) ,
                          (int)(Size.Width / 4 + 3 * hexagonSize * 2.6 / 12 + 3 * hexagonSize * 2.6 / 24) , (int)(Size.Height / 4 + hexagonSize ) ,
                          (int)(Size.Width / 4 + 3 * hexagonSize * 2.6 / 24) , Size.Height/4 - hexagonSize / 2,
                          (int)(Size.Width / 4 - 3 * hexagonSize * 2.6 / 12 + 3 * hexagonSize * 2.6 / 24)   , (int)(Size.Height / 4) ,
                          (int)(Size.Width / 4 - 3 * hexagonSize * 2.6 / 12 + 3 * hexagonSize * 2.6 / 24)   , (int)(Size.Height / 4 + hexagonSize) };
            return res;
        }
        private void createAllLabels()
        {
            int[] labelLoc = MenuLabelLocations(150, 40);
            Label a = CreateLabel(labelLoc[0], labelLoc[1], () => "start", Color.Brown);
            Label b = CreateLabel(labelLoc[2], labelLoc[3], () => "exit", Color.IndianRed);
            Label c = CreateLabel(labelLoc[4], labelLoc[5], () => "settings", Color.MediumPurple);
            Label d = CreateLabel(labelLoc[6], labelLoc[7], () => "runPreset", Color.ForestGreen);
            Label e = CreateLabel(labelLoc[8], labelLoc[9], () => "language", Color.Honeydew);
            Label f = CreateLabel(labelLoc[10], labelLoc[11], () => "fullScreen", Color.Yellow);
            Label g = CreateLabel(labelLoc[12], labelLoc[13], () => "help", Color.LightSkyBlue);

            Resize += (object sender, EventArgs h) =>
            {
                hexagonSize = (int)(Size.Height / 8);
                labelLoc = MenuLabelLocations(150, 40);
                a.Location = new Point(labelLoc[0] + 45, labelLoc[1] + 10);
                b.Location = new Point(labelLoc[2] + 45, labelLoc[3] + 10);
                c.Location = new Point(labelLoc[4] + 45, labelLoc[5] + 10);
                d.Location = new Point(labelLoc[6] + 45, labelLoc[7] + 10);
                e.Location = new Point(labelLoc[8] + 45, labelLoc[9] + 10);
                f.Location = new Point(labelLoc[10] + 45, labelLoc[11] + 10);
                g.Location = new Point(labelLoc[12] + 45, labelLoc[13] + 10);
            };

        }

        private LazyLabel CreateLabel(int x, int y, Func<string> name, Color color)
        {
            LazyLabel label = new LazyLabel();

            label.Location = new Point(x + 45, y + 10);
            label.Width = 60; 
            label.Height = 20; 
            label.LazyText = name;
            label.Dock = DockStyle.None;
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.BackColor = color;
            Controls.Add(label);

            return label;
        }
        private int[] MenuLabelLocations(int w, int h)
        {
            int[] menuLabelLocations = {
                (int)(Size.Width / 2 - w / 3), (int)(Size.Height / 2 -w/3),
                (int)(Size.Width / 2 - w / 3), (int)(Size.Height / 2 + hexagonSize*2-h/3),
                (int)(Size.Width / 2 + 3 * hexagonSize * 2.6 / 12 * 2 - w / 3), (int)(Size.Height / 2 - hexagonSize -h/3),
                (int)(Size.Width / 2 + 3 * hexagonSize * 2.6 / 12 * 2 - w / 3), (int)(Size.Height / 2 + hexagonSize -h/3),
                (int)(Size.Width / 2 - w / 3), (int)(Size.Height / 2 - hexagonSize*2-h/3),
                (int)(Size.Width / 2 - 3 * hexagonSize * 2.6 / 12 * 2 - w / 3), (int)(Size.Height / 2 - hexagonSize -h/3),
                (int)(Size.Width / 2 - 3 * hexagonSize * 2.6 / 12 * 2 - w / 3), (int)(Size.Height / 2 + hexagonSize -h/3)
            };
            return menuLabelLocations;
        }
        internal class LazyLabel : Label
        {
            public Func<string> LazyText { get; set; } = () => "";
            public override string Text { get => LazyText(); }
        }
    }
}
