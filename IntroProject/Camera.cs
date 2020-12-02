using System;
using System.Collections.Generic;
using System.Text;

namespace IntroProject
{
    public class Camera
    {
        int width, height;
        Ecosystem ecosystem;
        public Camera(int width, int height, Ecosystem eco) {
            this.width = width;
            this.height = height;
            this.ecosystem = eco;
        
        }
    }
}
