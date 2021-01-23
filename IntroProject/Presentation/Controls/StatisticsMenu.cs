using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.WindowsForms;

namespace IntroProject.Presentation.Controls
{ 
    public class StatisticsMenu : UserControl
    {
        int r = 0;
        private PlotView plot1;
        public IList<Statistics> WorkableStats = new List<Statistics>();
        private bool GraphChoice1, GraphChoice2, GraphChoice3, GraphChoice4, GraphChoice5, GraphChoice6;

        public StatisticsMenu(int w, int h, EventHandler exitMenu)
        {
            BackColor = Color.FromArgb(123, 156, 148);
            Size = new Size(w, h);
            int edge = Math.Max(Size.Width, Size.Height) / 36;

            Button exit = new ButtonImaged(Properties.Resources.X_icon);
            exit.Location = new Point(2, 2);
            exit.Size = new Size(edge, edge);
            exit.FlatAppearance.BorderColor = Color.FromArgb(123, 156, 148);
            exit.Click += exitMenu;
            Controls.Add(exit);

            InitChart();

            Resize += (object o, EventArgs ea) =>
            {
                edge = Math.Max(Size.Width, Size.Height) / 36;
                exit.Size = new Size(edge, edge);
            };

            Button statname1 = ButtonList("CarnivorePopulation");
            Button statname2 = ButtonList("CarnivoreVelocity");
            Button statname3 = ButtonList("CarnivoreSize");
            Button statname4 = ButtonList("HerbivorePopulation");
            Button statname5 = ButtonList("HerbivoreVelocity");
            Button statname6 = ButtonList("HerbivoreSize");

            statname1.Click += (object o, EventArgs ea) => { GraphChoice1 = !GraphChoice1; InitChart(); };
            statname2.Click += (object o, EventArgs ea) => { GraphChoice2 = !GraphChoice2; InitChart(); };
            statname3.Click += (object o, EventArgs ea) => { GraphChoice3 = !GraphChoice3; InitChart(); };
            statname4.Click += (object o, EventArgs ea) => { GraphChoice4 = !GraphChoice4; InitChart(); };
            statname5.Click += (object o, EventArgs ea) => { GraphChoice5 = !GraphChoice5; InitChart(); };
            statname6.Click += (object o, EventArgs ea) => { GraphChoice6 = !GraphChoice6; InitChart(); };
        }

        public void InitChart()
        {
            WorkableStats = StatisticsValues.statisticsvalues;

            this.plot1 = new PlotView
            {
                Location = new Point(Width / 10, 0),
                Margin = new Padding(0),
                Name = "plot1",
                Size = new Size((int)(Width * 0.9), Height),
                TabIndex = 0,
            };

            var pm = new PlotModel
            {
                Title = "Graphs of Life",
                Subtitle = "Epic",
                Background = OxyColors.SlateGray,
                PlotType = PlotType.Cartesian
            };

            pm.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 25,
                MajorStep = 5,
                MinorStep = 1,
                TickStyle = OxyPlot.Axes.TickStyle.Outside
            });

            pm.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Top,
                Minimum = 0,
                Maximum = GetTime(WorkableStats),
                MajorStep = GetTime(WorkableStats) / 10,
                MinorStep = GetTime(WorkableStats) / 100,
                TickStyle = OxyPlot.Axes.TickStyle.Outside
            });

            if(GraphChoice1)
            {
                var Choice1 = new LineSeries
                {
                    Title = "PopulationSize Carnivores",
                    Color = OxyColors.SkyBlue,
                    MarkerSize = 6,
                    MarkerStroke = OxyColors.White,
                    MarkerFill = OxyColors.SkyBlue,
                    MarkerStrokeThickness = 1.5
                };
                foreach (Statistics stats in WorkableStats)
                {
                    Choice1.Points.Add(new DataPoint(stats.time, stats.PopulationSizeCarnivores));
                    Debug.WriteLine(Convert.ToString(stats.PopulationSizeCarnivores) + " " + Convert.ToString(stats.time));
                }
                pm.Series.Add(Choice1);
            }

            plot1.Model = pm;
            this.Controls.Add(this.plot1);
        }

        private double GetTime(IList<Statistics> stats)
        {
            double time = 1;
            if (stats.Count > 0)
                time = stats[^1].time;
            return time;
        }

        private Button ButtonList(String name) //makes a button and next call makes a button under the previus.
        {
            r++;
            int i = r, y = Math.Min(Size.Width / 30, Size.Height / 20) + 10;
            Button button = new Button();
            button.Text = name;
            button.Location = new Point(0, i*y);
            button.Size = new Size(Width / 12, Height / 20);
            button.BackColor = Color.White;

            Resize += (object o, EventArgs ea) =>
            {
                y = Math.Min(Size.Width / 30, Size.Height / 20) + 10;

                button.Location = new Point(0, i * y);
                button.Size = new Size(Width / 12, Height / 20);
            };

            Controls.Add(button);
            return button;
        }
    }
}
