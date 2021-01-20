using System;
using System.Drawing;
using System.Windows.Forms;

namespace IntroProject.Presentation.Controls
{
    public class StatisticsMenu : UserControl
    {
        int r = 0;
        public StatisticsMenu(int w, int h, EventHandler exitMenu)
        {
            BackColor = Color.FromArgb(123, 156, 148);
            Size = new Size(w, h);
            int edge = Math.Max(Size.Width, Size.Height) / 36;

            Button exit = new ButtonImaged(Properties.Resources.X_icon);
            exit.Location = new Point(2, 2);
            exit.Size = new Size(edge, edge);
            exit.FlatAppearance.BorderColor = Color.FromArgb(123, 156, 148);
            exit.Click += exitMenu;
            Controls.Add(exit);

            Resize += (object o, EventArgs ea) =>
            {
                edge = Math.Max(Size.Width, Size.Height) / 36;
                exit.Size = new Size(edge, edge);
            };

            Button statname1 = ButtonList("CarnivorePopulation");
            Button statname2 = ButtonList("CarnivoreVelocity");
            Button statname3 = ButtonList("CarnivoreSize");
            Button statname4 = ButtonList("HerbivorePopulation");
            Button statname5 = ButtonList("HerbivoreVelocity");
            Button statname6 = ButtonList("HerbivoreSize");
        }

        public void MakeGraph(int i)
        {
            //Getstats and make a graph depending on the type i given by the press of the buttons
        }

        private Button ButtonList(String name) //makes a button and next call makes a button under the previus.
        {
            r++;
            int i = r, y = Math.Min(Size.Width / 30, Size.Height / 20) + 10;
            Button button = new Button();
            button.Text = name;
            button.Location = new Point(0, i*y);
            button.Size = new Size(Width / 12, Height / 20);
            button.BackColor = Color.White;

            Resize += (object o, EventArgs ea) =>
            {
                y = Math.Min(Size.Width / 30, Size.Height / 20) + 10;

                button.Location = new Point(0, i * y);
                button.Size = new Size(Width / 12, Height / 20);
            };

            Controls.Add(button);
            return button;
        }
    }
}
