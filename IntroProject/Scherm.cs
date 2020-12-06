using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Drawing;

namespace IntroProject
{
    public partial class Scherm : Form
    {
        public Scherm()
        {
            InitializeComponent();

            Button play = new Button();
            Button stop = new Button();
            Button pause = new Button();
            Button settings = new Button();
            Button statistics = new Button();
            Button exit = new Button();
            Button help = new Button();
            play.Image = IntroProject.Properties.Resources.Play_icon;
            stop.Image = IntroProject.Properties.Resources.Stop_icon;
            pause.Image = IntroProject.Properties.Resources.Pause_icon;
            settings.Image = IntroProject.Properties.Resources.Settings_icon;
            statistics.Image = IntroProject.Properties.Resources.Graph_icon;
            exit.Image = IntroProject.Properties.Resources.X_icon;
            help.Image = IntroProject.Properties.Resources.Help_icon;
            
            pause.Location = new Point (40, 40);
            play.Location = new Point(80, 40);
            stop.Location = new Point(120, 40);

            pause.AutoSize = true;

            this.Controls.Add(pause);
            this.Controls.Add(play);
            this.Controls.Add(stop);
            // (mogelijk) na informatie van de user is verkregen maak een processor classe, om het ecosysteem op te zetten.
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
