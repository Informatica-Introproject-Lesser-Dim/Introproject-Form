using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IntroProject
{
    static public class DrawLoading
    {

        public static void draw(PaintEventArgs e, Bitmap bitmap, int x, int y)
        {
            Bitmap bm = new Bitmap(50,50);
            Graphics newGraphics = Graphics.FromImage(bm);
            
            e.Graphics.DrawImage(bm, new Point(100,100));
        }
    }
}
