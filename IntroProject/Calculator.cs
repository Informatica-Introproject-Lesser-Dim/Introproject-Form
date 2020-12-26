using System;
using System.Collections.Generic;
using System.Text;

namespace IntroProject
{
    static class Calculator
    {
        private const float jumpBias = 1.0f;
        private const float moveBias = 1.0f; //mess a bit with these values when we got everything else set up correctly

        public static float JumpCost(float jumpHeight) {
            return jumpHeight * jumpHeight * jumpBias;
        }

        public static float EnergyPerMeter(float velocity) {
            return 0.5f * velocity * velocity * moveBias;
        }
    }
}
