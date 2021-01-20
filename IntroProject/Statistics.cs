using System;
using System.Collections.Generic;
using System.Text;

namespace IntroProject
{
    public class Statistics
    {
        private double time, TotalVelocityHerbivores, TotalVelocityCarnivores, TotalSizeHerbivores, TotalSizeCarnivores;
        private int PopulationSizeHerbivores, PopulationSizeCarnivores;

        public Statistics(double time, int PopSizeH, int PopSizeC, double TotVH, double TotVC, double TotSH, double TotSC)
        {
            this.time = time;
            this.PopulationSizeHerbivores = PopSizeH;
            this.PopulationSizeCarnivores = PopSizeC;
            this.TotalVelocityHerbivores = TotVH;
            this.TotalVelocityCarnivores = TotVC;
            this.TotalSizeHerbivores = TotSH;
            this.TotalSizeCarnivores = TotSC;
        }
    }
}
