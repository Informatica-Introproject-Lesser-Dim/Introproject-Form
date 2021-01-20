using System;
using System.Drawing;
using System.Windows.Forms;

using IntroProject.Presentation.Controls;

namespace IntroProject.Presentation
{
    public partial class HexagonOfLife : Form
    {
        MapScreen mapscr;
        DropMenu dropMenu;
        SettingsMenu settingsMenu;
        HelpMenu helpMenu;
        StatisticsMenu statisticsMenu;

        public HexagonOfLife()
        {
            InitializeComponent();

            Size = new Size(1800, 1200);
            mapscr = new MapScreen(Size);
            settingsMenu = new SettingsMenu(Size.Width, Size.Height, (object o, EventArgs ea) =>
            { 
                settingsMenu.RevertSettings(); 
                mapscr.UpdateVars(settingsMenu.newMap); 
                settingsMenu.newMap = false; 
                settingsMenu.Hide(); 
                if (!mapscr.buttonPaused)
                    mapscr.paused = false; 
            });
            helpMenu = new HelpMenu(Size.Width, Size.Height, (object o, EventArgs ea) => { helpMenu.Hide(); if (!mapscr.buttonPaused) mapscr.paused = false; });
            statisticsMenu = new StatisticsMenu(Size.Width, Size.Height, (object o, EventArgs ea) => { statisticsMenu.Hide(); if (!mapscr.buttonPaused) mapscr.paused = false; });
            dropMenu = new DropMenu(Size.Width/10, Size.Height, 
                                   (object o, EventArgs ea) => { settingsMenu.Show(); settingsMenu.BringToFront(); mapscr.paused = true; }, 
                                   (object o, EventArgs ea) => { helpMenu.Show(); helpMenu.BringToFront(); mapscr.paused = true; }, 
                                   (object o, EventArgs ea) => { statisticsMenu.Show(); statisticsMenu.BringToFront(); mapscr.paused = true; },          
                                    mapscr.plus);
            dropMenu.Dock = DockStyle.Right;

            Controls.Add(dropMenu);
            Controls.Add(mapscr);
            Controls.Add(settingsMenu);
            Controls.Add(helpMenu);
            Controls.Add(statisticsMenu);
            dropMenu.Hide();
            settingsMenu.Hide();
            statisticsMenu.Hide();
            helpMenu.Hide();

            Resize += (object o, EventArgs ea) => 
            {
                int maxim = Math.Max(Size.Width, Size.Height) / 36;
                mapscr.Size = new Size(Size.Width, Size.Height);
                dropMenu.Size = new Size(Size.Width / 10, Size.Height);
                settingsMenu.Size = new Size(Size.Width, Size.Height);
                helpMenu.Size = new Size(Size.Width, Size.Height);
                statisticsMenu.Size = new Size(Size.Width, Size.Height);
                mapscr.plus.Location = new Point(Size.Width - maxim - 16, 5);
                mapscr.plus.Size = new Size(maxim, maxim);
            };
            mapscr.plus.Click += (object o, EventArgs ea) => { dropMenu.Show(); mapscr.plus.Hide(); };
        }
    }

    internal class CustomTrackbar : TrackBar
    {
        public CustomTrackbar(int x, int y, int min, int max)
        {
            Location = new Point(x, y);
            Padding = Padding.Empty;
            Cursor = Cursors.Hand;
            BackColor = Color.White;
            Minimum = min;
            Maximum = max;
            TickFrequency = (max - min) / 10;
            TickStyle = TickStyle.Both;
            SmallChange = 1;
            LargeChange = (max - min) / 10;
            BackColor = Color.FromArgb(123, 156, 148);
        }
    }
}
