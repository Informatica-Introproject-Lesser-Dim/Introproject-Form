﻿namespace IntroProject.Core.Math
{
    public class Point2D
    {
        public double X;
        public double Y;
        public int x
        {
          get => (int)X;
          set => X = value;
        }
        public int y
        {
          get => (int)Y;
          set => Y = value;
        }

        public static Point2D operator +(Point2D a, Point2D b)
        {
            a.X += b.X;
            a.Y += b.Y;
            return a;
        }
    }

    public static class Trigonometry
    {
        public static double Hypo(double a, double b) => System.Math.Sqrt(HypoPow2(a, b));
        public static double Hypo(Point2D a) => System.Math.Sqrt(HypoPow2(a.X, a.Y));
        public static double HypoPow2(double a, double b) => a * a + b * b;
        public static double HypoPow2(Point2D a) => HypoPow2(a.X, a.Y);
        public static double Distance((double, double) a, (double, double) b) => Hypo(a.Item1 - b.Item1, a.Item2 - b.Item2);
        public static double Distance(Point2D a, Point2D b) => Distance((a.X, a.Y), (b.X, b.Y));
        public static double DistancePow2((double, double) a, (double, double) b) => HypoPow2(a.Item1 - b.Item1, a.Item2 - b.Item2);
        public static double DistancePow2(Point2D a, Point2D b) => DistancePow2((a.X, a.Y), (b.X, b.Y));
    }
}
