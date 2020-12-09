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
            debugscr = new DebugScreen(this.Size);
            this.tabs.TabPages.Add(debugscr);
        }
    }

    class DebugScreen : TabPage
    {
        Map kaart;
        int[] pos = new int[2] { 0, 0 };
        Font font = new Font("Arial", 12);
        int n = 0;
        


        public DebugScreen(Size size) : this(size.Width, size.Height) { }
        public DebugScreen(int w, int h) 
        {
            this.Size = new Size(w, h);
            this.Paint += drawScreen;
            kaart = new Map(50, 30, 20, 0);
            Path.initializePaths(20);
            for (int i = 0; i < 1; i++) {
                kaart.placeRandom(new Planten(0, 0, 5));
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
            kaart.draw(pea.Graphics, 50, 50, this.Width, this.Height);
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
}
