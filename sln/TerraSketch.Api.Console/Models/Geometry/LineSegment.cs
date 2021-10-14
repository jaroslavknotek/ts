using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;
using Common.MathUtils;

namespace Common.DataObjects.Geometry
{
    public struct LineSegment
    {
        public LineSegment(Vector2 p1, Vector2 p2) : this()
        {
            Point1 = p1;
            Point2 = p2;

        }


        public Vector2 Point1 { get; private set; }
        public Vector2 Point2 { get; private set; }
        public Vector2 Direction
        {
            get
            {
                var x = Point1 - Point2;
                return new Vector2(x.X, x.Y).Normalize();
            }
        }
        public Vector2 PerpendicularNotNormalized
        {
            get
            {
                var x = Point1 - Point2;
                return new Vector2(x.Y, -x.X).Normalize();
            }
        }

        public float Distance
        {
            get
            {
                var x = Point1 - Point2;
                return new Vector2(x.X, x.Y).Length();
            }
        }


        public override string ToString()
        {

            return string.Format("{0} - {1}", Point1.ToString(), Point2.ToString());
        }
    }
}
