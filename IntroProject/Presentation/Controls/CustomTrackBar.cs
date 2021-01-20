using System.Drawing;
using System.Windows.Forms;

namespace IntroProject.Presentation.Controls
{
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
