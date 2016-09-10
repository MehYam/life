using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace life
{
    static class Util
    {
        // Because C# can't support math on generics...
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
    }
}
