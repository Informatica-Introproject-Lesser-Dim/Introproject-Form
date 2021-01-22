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
        HomeMenu homeMenu;

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
            helpMenu = new HelpMenu(Size.Width, Size.Height,
                                   (object o, EventArgs ea) => { helpMenu.Hide(); if (!mapscr.buttonPaused) mapscr.paused = false; },
                                   (object o, EventArgs ea) => { homeMenu.Show(); helpMenu.Hide(); helpMenu.backExit(); });
            statisticsMenu = new StatisticsMenu(Size.Width, Size.Height, (object o, EventArgs ea) => { statisticsMenu.Hide(); if (!mapscr.buttonPaused) mapscr.paused = false; });
            dropMenu = new DropMenu(Size.Width/10, Size.Height, 
                                   (object o, EventArgs ea) => { settingsMenu.Show(); settingsMenu.BringToFront(); mapscr.paused = true; }, 
                                   (object o, EventArgs ea) => { helpMenu.Show(); helpMenu.BringToFront(); mapscr.paused = true; }, 
                                   (object o, EventArgs ea) => { statisticsMenu.Show(); statisticsMenu.BringToFront(); mapscr.paused = true; },          
                                    mapscr.plus);
            dropMenu.Dock = DockStyle.Right;

            homeMenu = new HomeMenu(Size.Width, Size.Height,
                                   (object o, EventArgs ea) => { mapscr.MakeMap(); homeMenu.Hide(); },
                                   (object o, EventArgs ea) => { homeMenu.Hide(); settingsMenu.Show(); settingsMenu.BringToFront(); },
                                   (object o, EventArgs ea) => { settingsMenu.ImportSettings(o, ea); homeMenu.Hide(); },
                                   (object o, EventArgs ea) => { SetFormFullscreen();},
                                   (object o, EventArgs ea) => { homeMenu.Hide(); helpMenu.Show(); helpMenu.BringToFront(); helpMenu.backHome(); }
                                   );

            Controls.Add(homeMenu);
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
                homeMenu.Size = new Size(Size.Width, Size.Height);
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
        public void SetFormFullscreen()
        {
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        }
    }
}
