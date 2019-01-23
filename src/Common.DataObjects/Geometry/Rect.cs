using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Common.MathUtils;

namespace Common.DataObjects.Geometry
{ 
    public struct Rect
    {
        public int Left { get; private set; }
        public int Right { get; private set; }
        public int Top { get; private set; }
        public int Bottom { get; private set; }

        public Rect(Vector4 v):this()
        {
            Left = (int)v.W;
            Right = (int)v.Y;
            Top = (int)v.X;
            Bottom = (int)v.Z;
        }

        public Rect(int l, int r, int t, int b) : this()
        {
            Left = l;
            Right = r;
            Top = t;
            Bottom = b;
        }

        public Rect Extend(Rect b, int fadeSize)
        {
            return new Rect(b.Left - fadeSize, b.Right + fadeSize, b.Top - fadeSize, b.Bottom + fadeSize);
        }

        public  Vector2 GetSize()
        {
            return new Vector2(Right - Left, Bottom - Top);
        }
        public Vector2 GetTopLeftMargin()
        {
            return new Vector2(Left, Top);
        }

        public Rect RestrictOn(IntVector2 layerResolution)
        {
            return new Rect(
                    NewMethod(Left,layerResolution.X),
                    NewMethod(Right, layerResolution.X),
                    NewMethod(Top, layerResolution.Y),
                    NewMethod(Bottom, layerResolution.Y)
                );
        }

        private int NewMethod(int max, float value)
        {
            return (int)JryMath.Max(JryMath.Min(max, value), 0);
        }
    }
}
