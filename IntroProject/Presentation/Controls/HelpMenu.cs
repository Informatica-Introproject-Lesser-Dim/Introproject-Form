using System;
using System.Drawing;
using System.Windows.Forms;

using IntroProject.Core.Utils;

namespace IntroProject.Presentation.Controls
{
    public class HelpMenu : UserControl
    {
        MultipleLanguages translator = MultipleLanguages.Instance;

        private Label textLabel = new Label();

        public HelpMenu(int w, int h, EventHandler exitMenu)
        {
            this.translator = new MultipleLanguages();
            this.BackColor = Color.FromArgb(123, 156, 148);
            this.Size = new Size(w, h);

            Button exit = new ButtonImaged(Properties.Resources.X_icon);
            exit.Location = new Point(2, 2);
            exit.Size = new Size(50, 50);
            exit.FlatAppearance.BorderColor = Color.FromArgb(123, 156, 148);
            exit.Click += exitMenu;

            textLabel.Location = new Point(this.Size.Width / 2 - 200, 30);
            textLabel.Font = new Font("Arial", 22, FontStyle.Regular);
            textLabel.Size = new Size(400, 300);
            textLabel.Text = translator.DisplayText("helpText");

            this.Controls.Add(textLabel);
            this.Controls.Add(exit);
        }
    }
}
