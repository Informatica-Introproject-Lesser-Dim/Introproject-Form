namespace IntroProject.Core.Math
{
    public class Point2D
    {
        public double X;
        public double Y;
    }

    public static class Trigonometry
    {
        public static double Hypo(double a, double b) => System.Math.Sqrt(a * a + b * b);
        public static double Hypo(Point2D a) => Hypo(a.X, a.Y);
        public static double Distance((double, double) a, (double, double) b) => Hypo(a.Item1 - b.Item1, a.Item2 - b.Item2);
        public static double Distance(Point2D a, Point2D b) => Distance((a.X, a.Y), (b.X, b.Y));
    }
}
