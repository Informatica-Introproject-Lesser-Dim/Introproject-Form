using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace IntroProject.Presentation.Controls
{
    class HomeMenu : UserControl
    {
        public int screenSizeX, screenSizeY, hexagonSize;
        public Point startButtonLocation, exitButtonLocation, settingsButtonLocation, runPresetButtonLocation, languageButtonLocation, fullScreenButtonLocation, helpButtonLocation;
        EventHandler _start, _settingStart, _preSet, _fullScreen, _help;
        public HomeMenu(int w, int h, EventHandler start, EventHandler settingStart, EventHandler preSet, EventHandler fullScreen, EventHandler help)
        {
            this.Size = new Size(w, h);
            BackgroundImage = Properties.Resources.Background_blurred;
            BackgroundImageLayout = ImageLayout.Stretch;
            screenSizeX = w;
            screenSizeY = h;
            hexagonSize = (int)(h / 8);
            _start = start;
            _settingStart = settingStart;
            _preSet = preSet;
            _fullScreen = fullScreen;
            _help = help;

            int[] menuButtonLocations = createMenuButtonLocations();
            startButtonLocation = new Point(menuButtonLocations[0], menuButtonLocations[1]);
            exitButtonLocation = new Point(menuButtonLocations[2], menuButtonLocations[3]);
            settingsButtonLocation = new Point(menuButtonLocations[4], menuButtonLocations[5]);
            runPresetButtonLocation = new Point(menuButtonLocations[6], menuButtonLocations[7]);
            languageButtonLocation = new Point(menuButtonLocations[8], menuButtonLocations[9]);
            fullScreenButtonLocation = new Point(menuButtonLocations[10], menuButtonLocations[11]);
            helpButtonLocation = new Point(menuButtonLocations[12], menuButtonLocations[13]);

            createAllLabels();
            createAllButtons();

        }

        public void createAllButtons()
        {
            Button startHexagonBt = LoadButton(startButtonLocation + new Size(1,1), hexagonSize + 2);
            startHexagonBt.BackColor = Color.Brown;
            Button exitHexagonBt = LoadButton(exitButtonLocation, hexagonSize);
            Button settingsHexagonBt = LoadButton(settingsButtonLocation, hexagonSize);
            Button runPresetHexagonBt = LoadButton(runPresetButtonLocation, hexagonSize);
            Button languageHexagonBt = LoadButton(languageButtonLocation, hexagonSize);
            Button fullScreenHexagonBt = LoadButton(fullScreenButtonLocation, hexagonSize);
            Button helpHexagonBt = LoadButton(helpButtonLocation, hexagonSize);

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
            Point[] usedHexagonPoints = HexagonPoints(hexagonLocation.X, hexagonLocation.Y, currentHexagonSize);
            hexagonButton.SetBounds(usedHexagonPoints[0].X - 5, usedHexagonPoints[0].Y - 5, usedHexagonPoints[2].X + 5, usedHexagonPoints[3].Y + 5);
            GraphicsPath hexagonPath = new GraphicsPath(FillMode.Winding);
            hexagonPath.AddPolygon(usedHexagonPoints);
            Region hexagonRegion = new Region(hexagonPath);
            hexagonButton.Region = hexagonRegion;
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
        private int[] createMenuButtonLocations()
        {
            int[] res = {
                          (int)(screenSizeX / 4 + 3 * hexagonSize * 2.6 / 24), screenSizeY / 4 + hexagonSize / 2,
                          (int)(screenSizeX / 4 + 3 * hexagonSize * 2.6 / 24), screenSizeY / 4 + hexagonSize + hexagonSize / 2,
                          (int)(screenSizeX / 4 + 3 * hexagonSize * 2.6 / 12 + 3 * hexagonSize * 2.6 / 24) , (int)(screenSizeY / 4) ,
                          (int)(screenSizeX / 4 + 3 * hexagonSize * 2.6 / 12 + 3 * hexagonSize * 2.6 / 24) , (int)(screenSizeY / 4 + hexagonSize ) ,
                          (int)(screenSizeX / 4 + 3 * hexagonSize * 2.6 / 24) , screenSizeY/4 - hexagonSize / 2,
                          (int)(screenSizeX / 4 - 3 * hexagonSize * 2.6 / 12 + 3 * hexagonSize * 2.6 / 24)   , (int)(screenSizeY / 4) ,
                          (int)(screenSizeX / 4 - 3 * hexagonSize * 2.6 / 12 + 3 * hexagonSize * 2.6 / 24)   , (int)(screenSizeY / 4 + hexagonSize) };
            return res;
        }
        private void createAllLabels()
        {
            int[] labelLoc = MenuLabelLocations(150, 40);
            CreateLabel(labelLoc[0], labelLoc[1], () => "start", Color.Brown);
            CreateLabel(labelLoc[2], labelLoc[3], () => "exit", Color.IndianRed);
            CreateLabel(labelLoc[4], labelLoc[5], () => "settings", Color.MediumPurple);
            CreateLabel(labelLoc[6], labelLoc[7], () => "runPreset", Color.ForestGreen);
            CreateLabel(labelLoc[8], labelLoc[9], () => "language", Color.Honeydew);
            CreateLabel(labelLoc[10], labelLoc[11], () => "fullScreen", Color.Yellow);
            CreateLabel(labelLoc[12], labelLoc[13], () => "help", Color.LightSkyBlue);

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
                (int)(screenSizeX / 2 - w / 3), (int)(screenSizeY / 2 -w/3),
                (int)(screenSizeX / 2 - w / 3), (int)(screenSizeY / 2 + hexagonSize*2-h/3),
                (int)(screenSizeX / 2 + 3 * hexagonSize * 2.6 / 12 * 2 - w / 3), (int)(screenSizeY / 2 - hexagonSize -h/3),
                (int)(screenSizeX / 2 + 3 * hexagonSize * 2.6 / 12 * 2 - w / 3), (int)(screenSizeY / 2 + hexagonSize -h/3),
                (int)(screenSizeX / 2 - w / 3), (int)(screenSizeY / 2 - hexagonSize*2-h/3),
                (int)(screenSizeX / 2 - 3 * hexagonSize * 2.6 / 12 * 2 - w / 3), (int)(screenSizeY / 2 - hexagonSize -h/3),
                (int)(screenSizeX / 2 - 3 * hexagonSize * 2.6 / 12 * 2 - w / 3), (int)(screenSizeY / 2 + hexagonSize -h/3)
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
