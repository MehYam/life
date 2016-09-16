﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lifeEngine
{
    public static class Util
    {
        // Because C# can't support math on generics...
        //KAI: could use extensions and C# version of specialization?
        public static Point<int> Add(Point<int> a, Point<int> b)
        {
            return new Point<int>(a.x + b.x, a.y + b.y);
        }
        public static Point<int> Subtract(Point<int> a, Point<int> b)
        {
            return new Point<int>(a.x - b.x, a.y - b.y);
        }
        public static Point<int> Multiply(Point<int> a, Point<int> b)
        {
            return new Point<int>(a.x * b.x, a.y * b.y);
        }
        public static Point<int> Divide(Point<int> a, Point<int> b)
        {
            return new Point<int>(a.x / b.x, a.y / b.y);
        }
        public static Point<int> Add(Point<int> a, int b)
        {
            return new Point<int>(a.x + b, a.y + b);
        }
        public static Point<int> Subtract(Point<int> a, int b)
        {
            return new Point<int>(a.x - b, a.y - b);
        }
        public static Point<int> Multiply(Point<int> a, int b)
        {
            return new Point<int>(a.x * b, a.y * b);
        }
        public static Point<int> Divide(Point<int> a, int b)
        {
            return new Point<int>(a.x / b, a.y / b);
        }
        public static Point<float> Add(Point<float> a, Point<float> b)
        {
            return new Point<float>(a.x + b.x, a.y + b.y);
        }
        public static Point<float> Subtract(Point<float> a, Point<float> b)
        {
            return new Point<float>(a.x - b.x, a.y - b.y);
        }
        public static Point<float> Multiply(Point<float> a, Point<float> b)
        {
            return new Point<float>(a.x * b.x, a.y * b.y);
        }
        public static Point<float> Divide(Point<float> a, Point<float> b)
        {
            return new Point<float>(a.x / b.x, a.y / b.y);
        }
        public static Point<float> Add(Point<float> a, float b)
        {
            return new Point<float>(a.x + b, a.y + b);
        }
        public static Point<float> Subtract(Point<float> a, float b)
        {
            return new Point<float>(a.x - b, a.y - b);
        }
        public static Point<float> Multiply(Point<float> a, float b)
        {
            return new Point<float>(a.x * b, a.y * b);
        }
        public static Point<float> Divide(Point<float> a, float b)
        {
            return new Point<float>(a.x / b, a.y / b);
        }
        public static float Magnitude(Point<float> a)
        {
            return (float)Math.Sqrt(Math.Pow(a.x, 2) + Math.Pow(a.y, 2));
        }
        public static bool NearlyEqual(float a, float b, float tolerance = 0.0001f)
        {
            return Math.Abs(a - b) <= tolerance;
        }
        public static bool NearlyEqual(Point<float> a, Point<float> b)
        {
            return NearlyEqual(a.x, b.x) && NearlyEqual(a.y, b.y);
        }
        public static bool Within(Point<int> testPoint, Point<int> min, Point<int> max)
        {
            return min.x <= testPoint.x && testPoint.x <= max.x && min.y <= testPoint.y && testPoint.y <= max.y;
        }
        public static readonly Point<int> zero = new Point<int>(0, 0);
    }
}
