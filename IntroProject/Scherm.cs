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
    public partial class Scherm : Form
    {
        public Scherm()
        {
            InitializeComponent();

            //HIER ADDE IK DE BUTTONS
            Button play = new Button();
            Button stop = new Button();
            Button pause = new Button();
            Button settings = new Button();
            Button statistics = new Button();
            // play.Image = this.BackgroundImage path
            // settings.Location = (Point) this.Width - 200, this.Height - 500;

            // (mogelijk) na informatie van de user is verkregen maak een processor classe, om het ecosysteem op te zetten.
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
