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

            Resize += (object o, EventArgs ea) =>
            {
                edge = Math.Max(Size.Width, Size.Height) / 36;
                exit.Size = new Size(edge, edge);
            };

            Button statname1 = ButtonList("PopulationSize");
            Button statname2 = ButtonList("Velocity");
            Button statname3 = ButtonList("Size");
            Button statname4 = ButtonList("Statistics");
            Button statname5 = ButtonList("Statistics");
            Button statname6 = ButtonList("Statistics");

            statname1.Click += (object o, EventArgs ea) => { InitChart(1); };
            statname2.Click += (object o, EventArgs ea) => { InitChart(2); };
            statname3.Click += (object o, EventArgs ea) => { InitChart(3); };
            statname4.Click += (object o, EventArgs ea) => { InitChart(4); };
            statname5.Click += (object o, EventArgs ea) => { InitChart(5); };
            statname6.Click += (object o, EventArgs ea) => { InitChart(6); };
        }

        public void InitChart(int GraphChoice)
        {
            WorkableStats = StatisticsValues.statisticsvalues;
            this.Controls.Remove(this.plot1);

            this.plot1 = new PlotView
            {
                Location = new Point(Width / 10, 0),
                Margin = new Padding(0),
                Name = "plot1",
                Size = new Size((int)(Width * 0.8), (int)(Height * 0.9)),
                TabIndex = 0,
            };

            var pm = new PlotModel
            {
                Title = "Graphs of Life",
                Subtitle = "Epic",
                Background = OxyColors.SlateGray,
                PlotType = PlotType.Cartesian
            };

            pm.Series.Clear();
            pm.Axes.Clear();
            
            if(GraphChoice == 1)
            { 
                var Choice1C = new LineSeries
                {
                    Title = "PopulationSize Carnivores",
                    Color = OxyColor.FromArgb(255, 254, 197, 163)
                };
                var Choice1H = new LineSeries
                {
                    Title = "PopulationSize Herbivores",
                    Color = OxyColor.FromArgb(255, 255, 253, 158)

                };
                foreach (Statistics stats in WorkableStats)
                {
                    Choice1C.Points.Add(new DataPoint(stats.time / 1000, stats.PopulationSizeCarnivores));
                    Choice1H.Points.Add(new DataPoint(stats.time / 1000, stats.PopulationSizeHerbivores));
                }
                pm.Series.Add(Choice1C);
                pm.Series.Add(Choice1H);
                pm.Axes.Add(new LinearAxis
                { 
                    Title = "PopulationSize",
                    Position = AxisPosition.Left,
                    AbsoluteMinimum = 0,
                    AbsoluteMaximum = GetMax(WorkableStats, 1) + 1,
                    MinimumMajorStep = 1
                });
                pm.Axes.Add(new LinearAxis
                { 
                    Title = "Time in seconds",
                    Position = AxisPosition.Bottom,
                    AbsoluteMinimum = 0,
                    AbsoluteMaximum = GetTime(WorkableStats)
                });
            }

            if(GraphChoice == 2)
            {
                var Choice2C = new LineSeries
                {
                    Title = "Average Velocity Carnivores",
                    Color = OxyColor.FromArgb(255, 254, 197, 163)
                };
                var Choice2H = new LineSeries
                {
                    Title = "Average Velocity Herbivores",
                    Color = OxyColor.FromArgb(255, 255, 253, 158)
                };
                foreach(Statistics stats in WorkableStats)
                {
                    Choice2C.Points.Add(new DataPoint(stats.time / 1000, stats.TotalVelocityCarnivores / stats.PopulationSizeCarnivores));
                    Choice2H.Points.Add(new DataPoint(stats.time / 1000, stats.TotalVelocityHerbivores / stats.PopulationSizeHerbivores));
                }
                pm.Series.Add(Choice2C);
                pm.Series.Add(Choice2H);
                pm.Axes.Add(new LinearAxis
                {
                    Title = "Average Velocity",
                    Position = AxisPosition.Left,
                    AbsoluteMinimum = 0,
                    AbsoluteMaximum = GetMax(WorkableStats, 2) + 1
                });
                pm.Axes.Add(new LinearAxis
                {
                    Title = "Time in seconds",
                    Position = AxisPosition.Bottom,
                    AbsoluteMinimum = 0,
                    AbsoluteMaximum = GetTime(WorkableStats)
                });
            }

            if (GraphChoice == 3)
            {
                var Choice3C = new LineSeries
                {
                    Title = "Average Size Carnivores",
                    Color = OxyColor.FromArgb(255, 254, 197, 163)

                };
                var Choice3H = new LineSeries
                {
                    Title = "Average Size Herbivores",
                    Color = OxyColor.FromArgb(255, 255, 253, 158)
                };
                foreach (Statistics stats in WorkableStats)
                {
                    Choice3C.Points.Add(new DataPoint(stats.time / 1000, stats.TotalSizeCarnivores / stats.PopulationSizeCarnivores));
                    Choice3H.Points.Add(new DataPoint(stats.time / 1000, stats.TotalSizeHerbivores / stats.PopulationSizeHerbivores));
                }
                pm.Series.Add(Choice3C);
                pm.Series.Add(Choice3H);
                pm.Axes.Add(new LinearAxis
                {
                    Title = "Average Size",
                    Position = AxisPosition.Left,
                    AbsoluteMinimum = 0,
                    AbsoluteMaximum = GetMax(WorkableStats, 3) + 1
                });
                pm.Axes.Add(new LinearAxis
                {
                    Title = "Time in seconds",
                    Position = AxisPosition.Bottom,
                    AbsoluteMinimum = 0,
                    AbsoluteMaximum = GetTime(WorkableStats)
                });
            }
            plot1.Model = pm;
            this.Controls.Add(this.plot1);
        }

        private double GetMax(IList<Statistics> statistics, int type)
        {
            double Max = 0;
            if (type == 1)
                foreach (Statistics stats in statistics)
                    Max = Math.Max(Max, Math.Max(stats.PopulationSizeCarnivores, stats.PopulationSizeHerbivores));

            if (type == 2)
                foreach (Statistics stats in statistics)
                    Max = Math.Max(Max, Math.Max(stats.TotalVelocityCarnivores / stats.PopulationSizeCarnivores, stats.TotalVelocityHerbivores / stats.PopulationSizeHerbivores));

            if (type == 3)
                foreach (Statistics stats in statistics)
                    Max = Math.Max(Max, Math.Max(stats.TotalSizeCarnivores / stats.PopulationSizeCarnivores, stats.TotalSizeHerbivores / stats.PopulationSizeHerbivores));

            return Max;
        }

        private double GetTime(IList<Statistics> stats)
        {
            double time = 1;
            if (stats.Count != 0)
                time = stats[^1].time / 1000;
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
