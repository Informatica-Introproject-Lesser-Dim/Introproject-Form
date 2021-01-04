using System;
using System.Collections.Generic;
using System.Text;

namespace IntroProject
{
    static class Calculator
    {
        private const float jumpBias = 0.001f;
        private const float moveBias = 0.1f; //mess a bit with these values when we got everything else set up correctly
        private const float standardBias = 0.00001f;

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
    }
}
