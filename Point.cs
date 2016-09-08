using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace life
{
    struct Point<T> : IEquatable<Point<T>>
    {
        public T x;
        public T y;

        public Point(T x, T y)
        {
            this.x = x;
            this.y = y;
        }
        public bool Equals(Point<T> other)
        {
            return x.Equals(other.x) && y.Equals(other.y);
        }
        public override bool Equals(object obj)
        {
            return obj is Point<T> && this == (Point<T>)obj;
        }
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("{0:0.0},{1:0.0}", x, y);
        }

        public static bool operator==(Point<T> a, Point<T> b)
        {
            return a.Equals(b);
        }
        public static bool operator!=(Point<T> a, Point<T> b)
        {
            return !a.Equals(b);
        }
    }
}