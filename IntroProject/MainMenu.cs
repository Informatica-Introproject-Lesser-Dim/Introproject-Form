using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;


namespace IntroProject
{
    public partial class MainMenu : Form
    {
        public int screenSizeX = 800;
        public int screenSizeY = 800;
        public int hexagonSize = 100;
        public int hexagonSizeDiference = 2;

        Button startHexagonBt = new Button();
        Button exitHexagonBt = new Button();
        Button abcHexagonBt = new Button();
        Button bcdHexagonBt = new Button();
        Button efgHexagonBt = new Button();
        Button iopHexagonBt = new Button();
        Button jklHexagonBt = new Button();

        public Point startButtonLocation, exitButtonLocation, abcButtonLocation, bcdButtonLocation, efgButtonLocation, iopButtonLocation, jklButtonLocation;

        public MainMenu()
        {
            InitializeComponent();

            this.Size = new Size(screenSizeX, screenSizeY);
            int[] menuButtonLocations = {   screenSizeX / 4                                     , screenSizeY / 4 + hexagonSize / 2,
                                            screenSizeX / 4                                     , screenSizeY / 4 + hexagonSize + hexagonSize / 2,
                                            (int)(screenSizeX / 4 + 3 * hexagonSize * 2.6 / 12) , (int)(screenSizeY / 4-hexagonSize / 2 + hexagonSize / 2) ,
                                            (int)(screenSizeX / 4 + 3 * hexagonSize * 2.6 / 12) , (int)(screenSizeY / 4+hexagonSize / 2 + hexagonSize / 2) ,
                                            screenSizeX / 4                                     , screenSizeY/4 - hexagonSize + hexagonSize / 2,
                                            (int)(screenSizeX / 4 - 3 * hexagonSize*2.6 / 12)   , (int)(screenSizeY / 4 - hexagonSize / 2+hexagonSize / 2) ,
                                            (int)(screenSizeX / 4 - 3 * hexagonSize*2.6 / 12)   , (int)(screenSizeY / 4 + hexagonSize / 2+hexagonSize / 2) ,};

            startButtonLocation = new Point(menuButtonLocations[0], menuButtonLocations[1]);
            exitButtonLocation = new Point(menuButtonLocations[2], menuButtonLocations[3]);
            abcButtonLocation = new Point(menuButtonLocations[4], menuButtonLocations[5]);
            bcdButtonLocation = new Point(menuButtonLocations[6], menuButtonLocations[7]);
            efgButtonLocation = new Point(menuButtonLocations[8],menuButtonLocations[9]);
            iopButtonLocation = new Point(menuButtonLocations[10], menuButtonLocations[11]);
            jklButtonLocation = new Point(menuButtonLocations[12], menuButtonLocations[13]);

            createAllButtons();
        }
    public void createAllButtons()
        {
            startHexagonBt.Text = "start";
            exitHexagonBt.Text = "exit";
            abcHexagonBt.Text = "abc";
            bcdHexagonBt.Text = "bcd";
            efgHexagonBt.Text = "efg";
            iopHexagonBt.Text = "iop";
            jklHexagonBt.Text = "jkl";

            LoadButton(startButtonLocation, hexagonSize, startHexagonBt);
            LoadButton(exitButtonLocation, hexagonSize, exitHexagonBt);
            LoadButton(abcButtonLocation, hexagonSize, abcHexagonBt);
            LoadButton(bcdButtonLocation, hexagonSize, bcdHexagonBt);
            LoadButton(efgButtonLocation, hexagonSize, efgHexagonBt);
            LoadButton(iopButtonLocation, hexagonSize, iopHexagonBt);
            LoadButton(jklButtonLocation, hexagonSize, jklHexagonBt);

            startHexagonBt.Click += startBTPressed;
            exitHexagonBt.Click += exitBTPressed;
            abcHexagonBt.Click += abcBTPressed;
            bcdHexagonBt.Click += bcdBTPressed;
            efgHexagonBt.Click += efgBTPressed;
            iopHexagonBt.Click += iopBTPressed;
            jklHexagonBt.Click += jklBTPressed;

            Controls.Add(startHexagonBt);
            Controls.Add(exitHexagonBt);
            Controls.Add(abcHexagonBt);
            Controls.Add(bcdHexagonBt);
            Controls.Add(efgHexagonBt);
            Controls.Add(iopHexagonBt);
            Controls.Add(jklHexagonBt);
        }
        public Point[] HexagonPoints(int centreX, int centreY, int size)
        {
            Point[] hexagonPoints = new Point[6];
            Rectangle boundBy = new Rectangle((int)(centreX - size * 2.6 / 3), (int)(centreY - size), (int)((size * 2.6 / 3) * 2), (int)(size*2));
            int quartWidth = boundBy.Width / 4;
            int halfHeight = boundBy.Height / 2;
            hexagonPoints[0] = new Point(boundBy.Left + quartWidth, boundBy.Top);
            hexagonPoints[1] = new Point(boundBy.Right - quartWidth, boundBy.Top);
            hexagonPoints[2] = new Point(boundBy.Right, boundBy.Top+halfHeight);
            hexagonPoints[3] = new Point(boundBy.Right - quartWidth, boundBy.Bottom);
            hexagonPoints[4] = new Point(boundBy.Left + quartWidth, boundBy.Bottom);
            hexagonPoints[5] = new Point(boundBy.Left, boundBy.Top + halfHeight);
            return hexagonPoints;
        }
        void LoadButton(Point hexagonLocation, int currentHexagonSize,Button hexagonButton)
        {
            Point[] usedHexagonPoints = HexagonPoints(hexagonLocation.X, hexagonLocation.Y, currentHexagonSize);
            hexagonButton.SetBounds(usedHexagonPoints[0].X-5, usedHexagonPoints[0].Y-5, usedHexagonPoints[2].X+5, usedHexagonPoints[3].Y+5);
            GraphicsPath hexagonPath = new GraphicsPath(FillMode.Winding);
            hexagonPath.AddPolygon(usedHexagonPoints);
            Region hexagonRegion = new Region(hexagonPath);
            hexagonButton.Region = hexagonRegion;
        }
        void UpdateButton(Point hexagonLocation, int currentHexagonSize, Button hexagonButton)
        {
            GraphicsPath hexagonPath = new GraphicsPath(FillMode.Winding);
            hexagonPath.AddPolygon(HexagonPoints(hexagonLocation.X, hexagonLocation.Y, currentHexagonSize));
            hexagonButton.Region = new Region(hexagonPath);
        }
        public void pointDefinition(object obj, PaintEventArgs pea)
        {
            pea.Graphics.DrawRectangle(new Pen(Brushes.Black), 300, 300, 310, 310);
            
        }
        private void startBTPressed(object sender, EventArgs e)
        {
            //Paint += pointDefinition;
            //this.Invalidate();
            //Debug.WriteLine("start button pressed");
            //this.Close();
        }
        private void exitBTPressed(object sender, EventArgs e)
        {
            //Debug.WriteLine("Exit button pressed");
        }
        private void abcBTPressed(object sender, EventArgs e)
        {
            //Debug.WriteLine("ABC button pressed");
        }
        private void bcdBTPressed(object sender, EventArgs e)
        {
            //Debug.WriteLine("BCD button pressed");
        }
        private void efgBTPressed(object sender, EventArgs e)
        {
            //Debug.WriteLine("EFG button pressed");
        }
        private void iopBTPressed(object sender, EventArgs e)
        {
            //Debug.WriteLine("IOP button pressed");
        }
        private void jklBTPressed(object sender, EventArgs e)
        { 
            //Debug.WriteLine("jkl button pressed");
        }
    }
}
