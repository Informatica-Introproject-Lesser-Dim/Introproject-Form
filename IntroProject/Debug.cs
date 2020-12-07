using System.Drawing;
using System.Windows.Forms;

namespace IntroProject
{
    public partial class Debug : Form
    {
        DebugScreen debugscr;
        public Debug()
        {
            InitializeComponent();
            this.Size = new Size(1800, 1200);
            debugscr = new DebugScreen(1800, 1200);
        }

        private void Debug_MouseClick(object sender, MouseEventArgs e) =>
            debugscr.Klik(sender, e);

        private void Debug_Paint(object sender, PaintEventArgs e) =>
            debugscr.drawScreen(sender, e);
    }

    class DebugScreen : UserControl
    {
        Map kaart;
        int[] pos = new int[2] { 0, 0 };
        Font font = new Font("Arial", 12);

        public DebugScreen(int w, int h) 
        {
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
}
