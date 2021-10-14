using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.MathUtils
{
    public static class JryMath
    {
        public static float Pi { get { return (float)Math.PI; } }

        public static float Log(float a, float thebase)
        {
            return (float)Math.Log(a, thebase);
        }

        public static float Max(float v1, float v2, float v3, float v4, float v5, float v6)
        {
            return Max(v1, Max(v2, Max(v3, Max(v4, Max(v5, v6)))));
        }
        public static float Max(float v1, float v2)
        {
            return Math.Max(v1, v2);
        }
        public static int Max(int v1, int v2)
        {
            return Math.Max(v1, v2);
        }

        //public static int Min(int v1, int v2)
        //{
        //    return Math.Min(v1, v2);
        //}

        public static double Max(double v1, double v2)
        {
            return Math.Max(v1, v2);
        }

        public static int Ceil(float v)
        {
            return (int)Math.Ceiling(v);
        }



        public static float Min(float v1, float v2, float v3, float v4, float v5, float v6)
        {
            return Min(v1, Min(v2, Min(v3, Min(v4, Min(v5, v6)))));
        }

        public static int Min(int v1, int v2)
        {
            return Math.Min(v1, v2);
        }
        public static float Min(float v1, float v2)
        {
            return Math.Min(v1, v2);
        }
        public static double Min(double v1, Double v2)
        {
            return Math.Min(v1, v2);
        }
        public static int Abs(int v)
        {
            return Math.Abs(v);
        }
        public static double Abs(double v)
        {
            return Math.Abs(v);
        }

        public static float Abs(float x)
        {
            return Math.Abs(x);
        }

        public static float Pow(int v, int depth)
        {
            return (float)Math.Pow(v, depth);
        }

        public static float Pow(float v, float depth)
        {
            return (float)Math.Pow(v, depth);
        }


        public static float Sin(float x)
        {
            return (float)Math.Sin(x);
        }
        public static float Cos(float p)
        {
            return (float)Math.Cos(p);
        }

        public static float Sqrt(float v)
        {
            return (float)Math.Sqrt(v);
        }
        public static double Sqrt(double v)
        {
            return Math.Sqrt(v);
        }

        public static float Acos(float v)
        {
            return (float)Math.Acos(v);
        }

        public static float Clip(float a, float min, float max)
        {

            if (a > max)
                return max;
            if (a < min)
                return min;
            return a;

        }

        public static int Floor(float v)
        {
            return (int)Math.Floor(v);
        }

        public static float Sign(float number)
        {
            return Math.Sign(number);
        }

        public static float Atan2(float x, float y)
        {
            return (float)Math.Atan2(x, y);
        }

    }
}
