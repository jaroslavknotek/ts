using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Common.MathUtils
{
    public struct IntVector2
    {
        private int _x;

        public int X
        {
            get { return _x; }
        }

        private int _y;

        public int Y
        {
            get { return _y; }
        }


        public IntVector2(int x, int y) : this()
        {
            _x = x;
            _y = y;
        }

        public static implicit operator Vector2(IntVector2 v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static implicit operator IntVector2(Vector2 v)
        {
            return new IntVector2((int)v.X, (int)v.Y);
        }

        public static bool operator ==(IntVector2 a, IntVector2 b)
        {

            return a.X == b.X && a.Y == b.Y;
        }
        public static bool operator !=(IntVector2 a, IntVector2 b)
        {
            return !(a == b);
        }

        public static IntVector2 operator +(IntVector2 a, IntVector2 b)
        {

            return new IntVector2(a.X + b.X, a.Y + b.Y);
        }
        public static IntVector2 operator -(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.X - b.X, a.Y - b.Y);
        }
        public static IntVector2 operator *(IntVector2 a, float b)
        {
            return new Vector2(a.X * b, a.Y * b);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

#if DEBUG
        public override string ToString()
        {
            return $"X:{X} Y:{Y}";
        }
#endif
    }
}
