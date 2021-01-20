using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace IntroProject.Presentation.Controls
{
    class HomeMenu : UserControl
    {
        public int screenSizeX, screenSizeY, hexagonSize;
        Button startHexagonBt = new Button(), exitHexagonBt = new Button(), settingsHexagonBt = new Button(), runPresetHexagonBt = new Button(), languageHexagonBt = new Button(), fullScreenHexagonBt = new Button(), helpHexagonBt = new Button();
        public Point startButtonLocation, exitButtonLocation, settingsButtonLocation, runPresetButtonLocation, languageButtonLocation, fullScreenButtonLocation, helpButtonLocation;
        EventHandler _exitMenu;
        public HomeMenu(int w, int h, EventHandler exitMenu)
        {
            _exitMenu = exitMenu;
            screenSizeX = w;
            screenSizeY = h;
            hexagonSize = (int)(h / 8);
            int[] menuButtonLocations = createMenuButtonLocations();
            startButtonLocation = new Point(menuButtonLocations[0], menuButtonLocations[1]);
            exitButtonLocation = new Point(menuButtonLocations[2], menuButtonLocations[3]);
            settingsButtonLocation = new Point(menuButtonLocations[4], menuButtonLocations[5]);
            runPresetButtonLocation = new Point(menuButtonLocations[6], menuButtonLocations[7]);
            languageButtonLocation = new Point(menuButtonLocations[8], menuButtonLocations[9]);
            fullScreenButtonLocation = new Point(menuButtonLocations[10], menuButtonLocations[11]);
            helpButtonLocation = new Point(menuButtonLocations[12], menuButtonLocations[13]);
            createAllButtons();

            createAllLabels();
        }

        public void createAllButtons()
        {
            LoadButton(startButtonLocation, hexagonSize, startHexagonBt);
            LoadButton(exitButtonLocation, hexagonSize, exitHexagonBt);
            LoadButton(settingsButtonLocation, hexagonSize, settingsHexagonBt);
            LoadButton(runPresetButtonLocation, hexagonSize, runPresetHexagonBt);
            LoadButton(languageButtonLocation, hexagonSize, languageHexagonBt);
            LoadButton(fullScreenButtonLocation, hexagonSize, fullScreenHexagonBt);
            LoadButton(helpButtonLocation, hexagonSize, helpHexagonBt);

            startHexagonBt.Click += startBTPressed;
            exitHexagonBt.Click += exitBTPressed;
            settingsHexagonBt.Click += settingsBTPressed;
            runPresetHexagonBt.Click += runPresetBTPressed;
            languageHexagonBt.Click += languageBTPressed;
            fullScreenHexagonBt.Click += fullScreenBTPressed;
            helpHexagonBt.Click += helpBTPressed;

            Controls.Add(startHexagonBt);
            Controls.Add(exitHexagonBt);
            Controls.Add(settingsHexagonBt);
            Controls.Add(runPresetHexagonBt);
            Controls.Add(languageHexagonBt);
            Controls.Add(fullScreenHexagonBt);
            Controls.Add(helpHexagonBt);
        }
        void LoadButton(Point hexagonLocation, int currentHexagonSize, Button hexagonButton)
        {
            Point[] usedHexagonPoints = HexagonPoints(hexagonLocation.X, hexagonLocation.Y, currentHexagonSize);
            hexagonButton.SetBounds(usedHexagonPoints[0].X - 5, usedHexagonPoints[0].Y - 5, usedHexagonPoints[2].X + 5, usedHexagonPoints[3].Y + 5);
            GraphicsPath hexagonPath = new GraphicsPath(FillMode.Winding);
            hexagonPath.AddPolygon(usedHexagonPoints);
            Region hexagonRegion = new Region(hexagonPath);
            hexagonButton.Region = hexagonRegion;
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
        private void startBTPressed(object sender, EventArgs e)
        {
            //save changes
            //exit menu
        }
        private void exitBTPressed(object sender, EventArgs e) => Application.Exit();
        public void settingsBTPressed(object sender, EventArgs e)
        {
            SettingsMenu settingsMenu = new SettingsMenu(screenSizeX,screenSizeY,_exitMenu);

        }

        private void runPresetBTPressed(object sender, EventArgs e)
        {
            //Debug.WriteLine("runPreset button pressed");
        }
        private void languageBTPressed(object sender, EventArgs e)
        {
            //Debug.WriteLine("language button pressed");
        }
        private void fullScreenBTPressed(object sender, EventArgs e)
        {
            //Debug.WriteLine("fullScreen button pressed");
        }
        private void helpBTPressed(object sender, EventArgs e)
        {
            //Debug.WriteLine("help button pressed");
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
            CreateLabel(labelLoc[0], labelLoc[1], () => "start");
            CreateLabel(labelLoc[2], labelLoc[3], () => "exit");
            CreateLabel(labelLoc[4], labelLoc[5], () => "settings");
            CreateLabel(labelLoc[6], labelLoc[7], () => "runPreset");
            CreateLabel(labelLoc[8], labelLoc[9], () => "language");
            CreateLabel(labelLoc[10], labelLoc[11], () => "fullScreen");
            CreateLabel(labelLoc[12], labelLoc[13], () => "help");

        }
        private LazyLabel CreateLabel(int x, int y, Func<string> name)
        {
            LazyLabel label = new LazyLabel();

            label.Location = new Point(x, y);
            label.Width = 150;
            label.Height = 40;
            label.LazyText = name;
            label.Dock = DockStyle.None;
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleCenter;
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
