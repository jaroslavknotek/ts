using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Common.MathUtils;

namespace Common.DataObjects.Geometry
{
    public class GeometryUtils
    {

        private const float epsilon = .001f;

        // this code was entirely copied from (fragment part of url here is correct)
        // http://stackoverflow.com/questions/4243042/c-sharp-point-in-polygon#7123291
        // credits to user http://stackoverflow.com/users/42845/keith

        public bool IsInPolygon(Vector2[] poly, IntVector2 p)
        {
            return IsInPolygon(poly, p.X, p.Y);

        }
        public bool IsInPolygon(Vector2[] poly, int x, int y)
        {
            if (poly.Length < 3)
                return false;


            bool inside = false;
            var oldPoint = new Vector2(
                poly[poly.Length - 1].X, poly[poly.Length - 1].Y);


            for (int i = 0; i < poly.Length; i++)
            {
                var newPoint = new Vector2(poly[i].X, poly[i].Y);

                Vector2 p1;
                Vector2 p2;
                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;

                    p2 = newPoint;
                }

                else
                {
                    p1 = newPoint;

                    p2 = oldPoint;
                }


                if ((newPoint.X < x) == (x <= oldPoint.X)
                    && (y - (long)p1.Y) * (p2.X - p1.X)
                    < (p2.Y - (long)p1.Y) * (x - p1.X))
                {
                    inside = !inside;
                }


                oldPoint = newPoint;
            }


            return inside;
        }

        public Vector2[] ResizePoints(Vector2 centre, Vector2[] pts, float resizeCoef)
        {
            var points = new Vector2[pts.Length];

            for (int i = 0; i < pts.Length; i++)
            {
                var temp = pts[i] - centre;
                points[i] = temp * resizeCoef + centre;

            }
            return points;
        }

        public bool HasIntersection(IEnumerable<LineSegment> edges, LineSegment edge)
        {

            foreach (var e in edges)
            {
                if (e.Point2 == edge.Point1 || e.Point1 == edge.Point2)
                {
                    // those two linses share a point so they intersect
                    continue;
                }

                if (SegmentHasIntersection(e, edge))
                    return true;
            }

            return false;
        }

        public bool LineHasIntersection(LineSegment a, LineSegment b)
        {
            return GetIntersectionPoint(a, b).HasValue;
        }
        public bool SegmentHasIntersection(LineSegment a, LineSegment b)
        {
            //var i = GetLinesIntersection(a, b);
            var i2 = GetIntersectionPoint(a, b);
            if (!i2.HasValue) return false;
            var inter = i2.Value;

            // intersection lies on both lines

            // intersection lies on a segment if its part of its bounding box



            return lineBoundingBoxContainsPoint(a, inter) && lineBoundingBoxContainsPoint(b, inter);

        }

        public Vector2? GetSegmentsIntersection(LineSegment a, LineSegment b)
        {
            //var i = GetLinesIntersection(a, b);
            var i2 = GetIntersectionPoint(a, b);
            if (!i2.HasValue)
                return null;
            var inter = i2.Value;

            // intersection lies on both lines

            // intersection lies on a segment if its part of its bounding box



            if (lineBoundingBoxContainsPoint(a, inter) && lineBoundingBoxContainsPoint(b, inter))
                return i2.Value;
            return null;
        }

        private Vector2? GetIntersectionPoint(LineSegment a, LineSegment b)
        {
            return  GetIntersectionPoint(ref a, ref b);
        }
        private Vector2? GetIntersectionPoint(ref LineSegment a, ref LineSegment b)
        {

            // Get A,B,C of first line - points : a.Point1 to a.Point2
            float A1 = a.Point2.Y - a.Point1.Y;
            float B1 = a.Point1.X - a.Point2.X;
            float C1 = A1 * a.Point1.X + B1 * a.Point1.Y;

            // Get A,B,C of second line - points : b.Point1 to b.Point2
            float A2 = b.Point2.Y - b.Point1.Y;
            float B2 = b.Point1.X - b.Point2.X;
            float C2 = A2 * b.Point1.X + B2 * b.Point1.Y;

            // Get delta and check if the lines are parallel
            float delta = A1 * B2 - A2 * B1;
            if (Math.Abs(delta) < epsilon)
                return null;

            // now return the Vector2 intersection point
            return new Vector2(
                (B2 * C1 - B1 * C2) / delta,
                (A1 * C2 - A2 * C1) / delta
            );
        }


        private static bool lineBoundingBoxContainsPoint(LineSegment a, Vector2 inter)
        {
            return lineBoundingBoxContainsPoint(ref a, ref inter);
        }
        private static bool lineBoundingBoxContainsPoint(ref LineSegment a,ref  Vector2 inter)
        {
            float xL = a.Point1.X < a.Point2.X ? a.Point1.X : a.Point2.X;
            float xG = a.Point1.X >= a.Point2.X ? a.Point1.X : a.Point2.X;


            if (inter.X < xL || inter.X > xG) return false;

            float yt = a.Point1.Y < a.Point2.Y ? a.Point1.Y : a.Point2.Y;
            float yb = a.Point1.Y >= a.Point2.Y ? a.Point1.Y : a.Point2.Y;

            if (inter.Y < yt || inter.Y > yb) return false;

            return true;
        }

        public Vector2 GetCenter(IEnumerable<Vector2> points)
        {
            //if (!points.Any()) throw new ArgumentException($"{nameof(points)} containts not points");
            if (!points.Any()) throw new ArgumentException("**** containts not points");
            Vector2 v = Vector2.Zero;
            int count = 0;
            foreach (var pt in points)
            {
                v += pt;
                count++;
            }

            return v / count;

        }

        public Rect GetBoundsForPoints(IEnumerable<Vector2> pts)
        {
            if (!pts.Any()) throw new ArgumentException("List is empty");
            float? top = null, right = null, bottom = null, left = null;

            foreach (var pt in pts)
            {
                if (right == null || pt.X > right) right = pt.X;
                if (left == null || pt.X < left) left = pt.X;

                if (top == null || pt.Y < top) top = pt.Y;
                if (bottom == null || pt.Y > bottom) bottom = pt.Y;


            }

            return new Rect((int)left.Value, (int)right.Value, (int)top.Value, (int)bottom.Value);
        }


        public Vector2 Crop(Vector2 point, Vector2 min, Vector2 max)
        {
            var x = crop(min.X, max.X, point.X);
            var y = crop(min.Y, max.Y, point.Y);
            return new Vector2(x, y);
        }

        private static float crop(float min, float max, float p)
        {
            return JryMath.Min(JryMath.Max(p, min), max);
        }

        public LineSegment Translate(LineSegment lineSegment, Vector2 margin)
        {
            return new LineSegment(lineSegment.Point1 + margin, lineSegment.Point2 + margin);
        }

        //public float DistanceFromPointToLine(Vector2 point, LineSegment line)
        //{
        //    Vector2 l1 = line.Point1;
        //    Vector2 l2 = line.Point2;

        //    var dist = Math.Abs((l2.X - l1.X) * (l1.Y - point.Y) - (l1.X - point.X) * (l2.Y - l1.Y)) /
        //            Math.Sqrt(Math.Pow(l2.X - l1.X, 2) + Math.Pow(l2.Y - l1.Y, 2));
        //    return (float)dist;
        //}

        public float DistanceFromPointToSegment(Vector2 coor, LineSegment seg)
        {
            var perpendicular = new LineSegment(coor, coor + seg.PerpendicularNotNormalized);
            Vector2 intersection;
            if (LineHasIntersectionWithSegment(ref perpendicular,ref seg, out intersection))
                return Vector2.Distance(intersection , coor);

            return JryMath.Min(
                Vector2.Distance(coor, seg.Point1),
                Vector2.Distance(coor, seg.Point2));
        }

        public float DistanceFromPointToSegmentSquare(ref Vector2 coor,ref LineSegment seg)
        {
            var perpendicular = new LineSegment(coor, coor + seg.PerpendicularNotNormalized );
            Vector2 intersection;
            if (LineHasIntersectionWithSegment(ref perpendicular, ref seg, out intersection))
                return (intersection - coor).LengthSquared();

            return JryMath.Min(
                Vector2.DistanceSquared(coor, seg.Point1),
                Vector2.DistanceSquared(coor, seg.Point2));
        }

        //private bool LineHasIntersectionWithSegment(LineSegment line, LineSegment segment)
        //{
        //    var i2 = GetIntersectionPoint(line, segment);
        //    if (!i2.HasValue) return false;
        //    var inter = i2.Value;


        //    return lineBoundingBoxContainsPoint(segment, inter);
        //}

        private bool LineHasIntersectionWithSegment(ref LineSegment line, ref LineSegment segment, out Vector2 inter)
        {
            var i2 = GetIntersectionPoint(ref line, ref segment);
            inter = new Vector2();
            if (!i2.HasValue) return false;
            inter = i2.Value;
            return lineBoundingBoxContainsPoint(ref segment, ref inter);
        }


        public float DistanceToNearestPointOfPoly(Vector2[] points, Vector2 coor)
        {
            if (points.Length <= 0) return 0;
            return points.Select(point => (point - coor).Length()).Min();
        }

        public float? DistanceToTwoNearestPointsOfPoly(Vector2[] points, Vector2 coor)
        {
            var min1 = float.MaxValue;
            var min2 = float.MaxValue;
            foreach (var point in points)
            {
                var dis = (coor - point).Length();

                if (!(min2 > dis)) continue;
                if (min1 > dis)
                {
                    min2 = min1;
                    min1 = dis;
                }
                else
                    min2 = dis;
            }
            return min1 + min2;
        }
    }

    public class VectorComparer : IComparer<Vector2>
    {
        public int Compare(Vector2 x, Vector2 y)
        {
            if (x.Y > y.Y)
            {
                return -1;
            }
            else if (x.Y < y.Y)
            {
                return 1;
            }
            else
            {
                if (x.X > y.X)
                {
                    return -1;
                }
                else if (x.X < y.X)
                {
                    return 1;
                }

                return 0;
            }

        }
    }
}
