using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Common.MathUtils;

namespace Voronoi.Generator.FortuneObjects
{
    public class CircleEvent : Event
    {
        public float Radius { get; private set; }

        private Vector2? _center;

        public Vector2? Center
        {
            get { return _center; }
            set { _center = value; }
        }

        public override float X { get { return Center.HasValue ? Center.Value.X : 0; } }
        public override float Y { get { return Center.HasValue ? Center.Value.Y - Radius : 0; } }
        public Parabola Parabola { get; set; }

        public bool IsValid { get; private set; }


        public CircleEvent(float x, float y, float rad)
        {
            Center = new Vector2(x, y); Radius = rad;
            IsValid = true;
        }
        public static bool operator ==(CircleEvent a, CircleEvent b)
        {
            if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null))
                return true;
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
                return false;


            return a.X == b.X && b.Y == a.Y;

        }
        public static bool operator !=(CircleEvent a, CircleEvent b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;

            var o = obj as CircleEvent;
            if (ReferenceEquals(o, null))
                throw new InvalidCastException(string.Format("Can't cast {0} to {1}", obj.GetType(), this.GetType()));

            return base.Equals(obj);
        }
        public override string ToString()
        {
            return string.Format("[X:{0} ;Y:{1}]", X, Y);
        }

        public void Invalidate()
        {
            this.IsValid = false;
        }
    }
}
