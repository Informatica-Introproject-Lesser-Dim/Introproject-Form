using System;
using System.Collections.Generic;
using System.Text;

namespace IntroProject
{
    static class Calculator
    {
        private const float jumpBias = 0.001f; /* Settings.JumpEnergy */
        private const float moveBias = 0.1f; /* Settings.WalkEnergy */ //mess a bit with these values when we got everything else set up correctly
        private const float standardBias = 0.00001f; /* Settings.PassiveEnergy */

        public static float JumpCost(Gene gene) {
            return gene.Size * gene.JumpHeight * gene.JumpHeight * jumpBias;
        }

        public static float EnergyPerMeter(Gene gene) {
            return moveBias * gene.Velocity * StandardEnergyCost(gene);
        }

        public static float EnergyPerTic(Gene gene) {
            return EnergyPerMeter(gene) * gene.Velocity;
        }

        public static float StandardEnergyCost(Gene gene) {
            return standardBias * gene.Size;
        }

        public static double distanceSquared(double x1, double y1, double x2, double y2) {
            double dx = x1 - x2;
            double dy = y1 - y2;
            return dx * dx + dy * dy;
        }
    }
}
