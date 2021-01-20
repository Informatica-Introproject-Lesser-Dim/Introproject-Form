using System;
using System.Drawing;
using System.Windows.Forms;

namespace IntroProject.Presentation.Controls
{
    class DropMenu : UserControl
    {
        public DropMenu(int w, int h, EventHandler settingsMenuOnClick, EventHandler helpMenuOnClick, EventHandler statisticsMenuOnClick, Button plus)
        {
            // FlatStyle to flat, and the colors of FlatAppearance MouseDownBackColor & MouseOverBackColor to transparent
            ButtonImaged settings = new ButtonImaged(Properties.Resources.Settings_icon);
            ButtonImaged statistics = new ButtonImaged(Properties.Resources.Graph_icon);
            ButtonImaged help = new ButtonImaged(Properties.Resources.Help_icon);
            ButtonImaged min = new ButtonImaged(Properties.Resources.Minus_icon);

            int edge = Math.Min((int)(Size.Width * 1.2), Size.Height / 7);
            int locY = (int)(Size.Height * .4);
            int marge = (int)(edge * .2);
            Size big = new Size(edge, edge);

            statistics.Location = new Point(Size.Width/10, locY);
            settings.Location = new Point(Size.Width / 10, locY + big.Width + marge);
            help.Location = new Point(Size.Width / 10, locY + 2*big.Width + 2 *marge);
            min.Location = new Point(5, 5);
            min.BringToFront();

            settings.Click += settingsMenuOnClick;
            statistics.Click += statisticsMenuOnClick;
            help.Click += helpMenuOnClick;
            min.Click += (object o, EventArgs ea) => { Hide(); plus.Show(); };

            Controls.Add(min);
            Controls.Add(statistics);
            Controls.Add(settings);
            Controls.Add(help);

            BackColor = Color.DimGray;
            Size = new Size(w, h);

            Resize += (object o, EventArgs ea) =>
            {
                edge = Math.Min((int)(Size.Width * 1.2), Size.Height / 7);

                big = new Size(edge, edge);
                statistics.Size = big;
                settings.Size = big;
                help.Size = big;
                min.Size = big / 3;

                locY = (int)(Size.Height * .4);
                marge = (int)(edge * .2);

                statistics.Location = new Point(Size.Width / 10, locY);
                settings.Location = new Point(Size.Width / 10, locY + big.Width + marge);
                help.Location = new Point(Size.Width / 10, locY + 2 * big.Width + 2 * marge);
            };
        }
    }
}
