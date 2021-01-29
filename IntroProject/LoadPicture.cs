using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace IntroProject
{
    
    class LoadPicture : UserControl
    {

        Bitmap[] loadImages = { new Bitmap(Properties.Resources.Loading0),
                                    new Bitmap(Properties.Resources.Loading1),
                                    new Bitmap(Properties.Resources.Loading2),
                                    new Bitmap(Properties.Resources.Loading3),
                                    new Bitmap(Properties.Resources.Loading4),
                                    new Bitmap(Properties.Resources.Loading5),
                                    new Bitmap(Properties.Resources.Loading6),
                                    new Bitmap(Properties.Resources.Loading7)};
        public int count;
        public LoadPicture(int counter)
        {
            count = counter;
            Paint += drawLoading;

        }
        public void drawLoading(Object o, PaintEventArgs pea)
        {
            pea.Graphics.DrawImage(loadImages[count], 100, 100, 100, 100);
        }

    }
    
}
