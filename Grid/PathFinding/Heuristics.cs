using System;
using System.Windows;

namespace Grid.PathFinding
{
    public class Heuristics
    {
        public static double ManhattanDistance(Point a, Point b)
        {
            return (Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y));
        }

        public static double EuclideanDistance(Point a, Point b)
        {
            double xd = a.X - b.X;
            double yd = a.Y - b.Y;
            return Math.Sqrt(xd * xd + yd * yd);
        }
    }
}