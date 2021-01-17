namespace IntroProject.Core.Math
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

        public Point2D()
        {
            this.X = 0;
            this.Y = 0;
        }

        /// <summary>
        /// Should be used as it was Point2D(double X, double Y)
        /// It circumvents C# not inheriting base class constructors implicitly
        /// It returns itself to allow chaining
        /// </summary>
        public Point2D SetPosition(double X, double Y)
        {
            this.X = X;
            this.Y = Y;

            return this;
        }

        public static Point2D operator +(Point2D a, Point2D b) =>
            new Point2D().SetPosition(a.X + b.X, a.Y + b.Y);

        public static Point2D operator -(Point2D a, Point2D b) =>
            new Point2D().SetPosition(a.X - b.X, a.Y - b.Y);

        public static Point2D operator *(Point2D a, double scale) =>
            new Point2D().SetPosition(a.X * scale, a.Y * scale);
    }
}
