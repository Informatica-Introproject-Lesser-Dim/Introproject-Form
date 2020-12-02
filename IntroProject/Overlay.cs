using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntroProject
{
    public class Overlay
    {
        Settings settings;
        OverlayButton[] buttons;

        public Overlay(Settings settings)
        {
            this.settings = settings;
            buttons = new OverlayButton[6];
            for (int i = 0; i < 3; i++)
            {
                buttons[i] = new OverlayButton(5 + 60 * i, 5, 50, 50);
            }
            for (int i = 0; i < 3; i++)
            {
                buttons[i + 3] = new OverlayButton(settings.camWidth - 55 - 60 * i, settings.camHeight - 55, 50, 50);
            }
        }

        public void Draw(Graphics g)
        {
            for (int i = 0; i < 6; i++)
                buttons[i].Draw(g);

        }
        //Visualiseert data van wezen uit het ecosysteem. Wanneer muis houdt boven een wezen visualiseer de data van het wezen.
    }
}
