using System;
using System.Collections.Generic;
using System.Text;

namespace IntroProject
{
    static class Calculator
    {
        private const float jumpBias = 0.01f;
        private const float moveBias = 0.001f; //mess a bit with these values when we got everything else set up correctly

        public static float JumpCost(Gene gene) {
            return gene.Size * gene.JumpHeight * gene.JumpHeight * jumpBias;
        }

        public static float EnergyPerMeter(Gene gene) {
            return gene.Velocity * StandardEnergyCost(gene);
        }

        public static float EnergyPerTic(Gene gene) {
            return EnergyPerMeter(gene) * gene.Velocity;
        }

        public static float StandardEnergyCost(Gene gene) {
            return moveBias * gene.Size;
        }
    }
}
