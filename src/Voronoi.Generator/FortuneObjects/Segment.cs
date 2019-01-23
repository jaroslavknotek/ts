using System.Numerics;
namespace Voronoi.Generator.FortuneObjects
{
    public class Segment
    {
        public Segment(Vector2 start, Vector2 le, Vector2 rg)
        {
            Start = start;

            this.LeftSite = le;
            this.RightSite = rg;
            var x = le - rg;

            End = null;

            this.Direction = new Vector2(x.Y, -x.X);
        }

        private Vector2? _start;

        public Vector2? Start
        {
            get
            {

                return _start;
            }
            private set { _start = value; }
        }

        public Vector2? End { get; set; }

        public Vector2? LeftSite { get; set; }
        public Vector2? RightSite { get; set; }
        public Vector2? Direction { get; set; }

        public static Vector2? GetTwoLinesIntersection(Segment a, Segment b)
        {
            string message = "line is not valid or has invalid point or direction";
            if (a == null || !a.Direction.HasValue || !a.Start.HasValue)
                throw new System.ArgumentException( 
                    message, "a");
            if (b == null || !b.Start.HasValue || !b.Direction.HasValue)
                throw new System.ArgumentException(
                    message, "b");
            var an = new Vector2(a.Direction.Value.Y, -a.Direction.Value.X);
            var bn = new Vector2(b.Direction.Value.Y, -b.Direction.Value.X);
            var ac = -(an.X * a.Start.Value.X + an.Y * a.Start.Value.Y);
            var bc = -(bn.X * b.Start.Value.X + bn.Y * b.Start.Value.Y);

            float delta = an.X * bn.Y - bn.X * an.Y ;
            if (delta == 0)
                return null;

            float x = (bn.Y * ac - an.Y * bc) / delta;
            float y = (an.X * bc - bn.X * ac) / delta;
            return new Vector2(-x,- y);

        }

        public override string ToString()
        {
            var p2 = string.Format("{0}", Start.HasValue? string.Format("X:{0} Y:{1}", Start.Value.X, Start.Value.Y) : "---");
            var p1 = string.Format("{0}", End.HasValue? string.Format("X:{0} Y:{1}", End.Value.X, End.Value.Y) : "---");
            return string.Format("{0} - {1}", p2, p1);
        }
    }
}
