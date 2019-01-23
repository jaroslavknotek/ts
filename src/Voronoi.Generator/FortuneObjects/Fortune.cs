
using System;
using System.Collections.Generic;
using System.Numerics;
using Common.MathUtils;
using Common.DataObjects.Heap;

namespace Voronoi.Generator.FortuneObjects
{
    public class Fortune
    {
        Heap<Event> queue = new Heap<Event>(new FortunePointComparer());
        Parabola root = null;
        IList<Segment> segs = new List<Segment>();
        private float _minSize = 0;
        private float _maxSize = 0;
        private float directixY = float.MaxValue;
        public IList<Segment> GetSegments(IList<Vector2> points, float minSize, float maxSize)
        {
            if (minSize >= maxSize) throw new ArgumentException("Minimal size is greater that maximal size.");
            _minSize = minSize;
            _maxSize = maxSize;
            foreach (var p in points)
                queue.Insert(new SiteEvent(p));
            while (!queue.IsEmpty)
            {
                Event current = queue.Pop();
                directixY = current.Y;
                if (current is SiteEvent)
                    insertParabola(current);
                else if (current is CircleEvent && (current as CircleEvent).IsValid)
                    removeParabola(current as CircleEvent);

            }

            this.finishSegments(maxSize);
            return segs;
        }

        /// <summary>
        /// Reaction to the site event. Finds intersected parabola. Replaces it by sub tree
        ///  X          oX
        ///     =>     / \
        ///           /   o
        ///          x    /\
        ///              y  x 
        /// X - insersected one, oX intersected one transofrmed to inner node, 
        /// x - left and right part intersected parabola
        /// o - empty inner node
        /// 
        /// Every inner node has its segment.
        /// </summary>
        /// <param name="point"></param>
        private void insertParabola(Vector2 point)
        {
            if (root == null)
            {
                root = new Parabola(point);
                return;
            }
            if (point.Y == root.Site.Y)
            {
                solveDegenerateCase(point);
                return;
            }

            var ipar = getIntersectedParabola(point);
            if (ipar.CircleEvent != null)
                ipar.CircleEvent.Invalidate();

            var intersection = getYOfIntersectedParabola(point, ipar, point.Y);
            var rightSeg = new Segment(intersection, ipar.Site, point);
            var leftSeg = new Segment(intersection, point, ipar.Site);
            segs.Add(leftSeg);
            segs.Add(rightSeg);

            var left = new Parabola(ipar.Site);
            var newPar = new Parabola(point);
            var right = new Parabola(ipar.Site);


            // Create subtree

            ipar.SetLeftParabola(left, leftSeg);

            var emptyRightChild = new Parabola();

            emptyRightChild.SetRightParabola(right, rightSeg);
            emptyRightChild.SetLeftParabola(newPar);

            ipar.SetRightParabola(emptyRightChild);

            checkCircle(left);
            checkCircle(right);
        }

        private void solveDegenerateCase(Vector2 point)
        {
            var newL = new Parabola(point);
            Parabola newP = root;
            var npl = root.GetLeftNode();
            while (npl != null)
            {
                newP = npl;
                npl = newP.GetLeftNode();
            }
            //newP is a root right now.
            var rr = newP;
            newP = new Parabola(newP.Site);

            var p = new Vector2(newL.Site.X +
                JryMath.Abs(newL.Site.X - newP.Site.X) / 2, _maxSize);
            var seg = new Segment(p, newP.Site, newL.Site);
            segs.Add(seg);

            rr.SetRightParabola(newP, seg);
            rr.SetLeftParabola(newL);
        }

        /// <summary>
        /// Reaction to the cirle event. Removes the parabola associated to this event.
        /// It also rebuilds the tree.
        /// </summary>
        /// <param name="ce"></param>
        private void removeParabola(CircleEvent ce)
        {
            var p = ce.Parabola;
            var pl = p.GetLeftParent();
            var pr = p.GetRightParent();
            var l = p.GetLeafParabolaOnTheLeft();
            var r = p.GetLeafParabolaOnTheRight();

            var sl = pl.Segment;
            var sr = pr.Segment;
            if (l.CircleEvent != null)
                l.CircleEvent.Invalidate();
            if (r.CircleEvent != null)
                r.CircleEvent.Invalidate();

            if (sl != null) sl.End = ce.Center;
            if (sr != null) sr.End = ce.Center;

            var seg = new Segment(ce.Center.Value, r.Site, l.Site);
            segs.Add(seg);

            var pp = p.Parent;
            
            if (pp.GetRightNode() == p)//removed parabole is on the right so the left one is correct
            {
                if (pp.Parent.GetLeftNode() == pp)
                    pp.Parent.SetLeftParabola(pp.GetLeftNode());
                if (pp.Parent.GetRightNode() == pp)
                    pp.Parent.SetRightParabola(pp.GetLeftNode());
                pr.Segment = seg;
            }
            else
            {
                if (pp.Parent.GetLeftNode() == pp)
                    pp.Parent.SetLeftParabola(pp.GetRightNode());
                if (pp.Parent.GetRightNode() == pp)
                    pp.Parent.SetRightParabola(pp.GetRightNode());

                pl.Segment = seg;
            }

            checkCircle(l);
            checkCircle(r);
        }

        /// <summary>
        /// Using fortune binary tree it returns parabolae that is uder the point of intersection.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="isInTheMiddle"></param>
        /// <returns></returns>
        public Parabola getIntersectedParabola(Vector2 point)
        {
            Parabola par = root;
            Vector2 x;

            while (!par.IsLeaf)
            {
                var l = par.GetLeftChildLeaf();
                var p = par.GetRightChildLeaf();
                x = getParabalaeIntersection(l, p, directixY);
                if (x.X >= point.X)
                    par = par.GetLeftNode();
                else
                    par = par.GetRightNode();
            }
            return par;
        }



        /// <summary>
        /// Checking whether two rays intersect. Their intersection is center of a cirlcle.(circle event)
        /// Using projection it checks if the event is on the correct place.
        /// </summary>
        /// <param name="par"></param>
        private void checkCircle(Parabola par)
        {
            var leftSeg = par.GetLeftSegment();
            var rightSeg = par.GetRightSegment();
            if (leftSeg == null || rightSeg == null) return;

            var center = Segment.GetTwoLinesIntersection(leftSeg, rightSeg);
            if (!center.HasValue) return;
            var c = center.Value;
            Segment s = null;
            if (center == leftSeg.Start)
                s = rightSeg;
            else
                s = leftSeg;

            var u =s.Start.Value - c;
            if (u.Length() < NumericsUtils.Vector2Epsilon) return;
            var v = (u).Normalize() - s.Direction.Value.Normalize();
            if (v.Length() < NumericsUtils.Vector2Epsilon /*&& center != sl.Start && center != sr.Start*/)
                return;

            var radius = (c - par.Site).Length();

            if (c.Y - radius - directixY > NumericsUtils.Vector2Epsilon) return;
            if (par.CircleEvent != null)
                par.CircleEvent.Invalidate();// when this happens the segment should be removed because it is invalid. maybe start = end => zero lenght

            CircleEvent cev = new CircleEvent(c.X, c.Y, radius);
            cev.Parabola = par;
            par.CircleEvent = cev;
            queue.Insert(cev);
        }

        /// <summary>
        /// Count point of intersection
        /// </summary>
        /// <param name="point"></param>
        /// <param name="par"></param>
        /// <param name="directixY"></param>
        /// <returns></returns>
        private Vector2 getYOfIntersectedParabola(Vector2 point, Parabola par, double directixY)
        {
            if (par == null )
                throw new Exception("NoIntersection");

            double denominator1 = 2.0f * (par.Site.Y - directixY);
            double a1 = 1.0f / denominator1;
            double b1 = -2.0f * par.Site.X / denominator1;
            double c1 = ((par.Site.X * par.Site.X) + (par.Site.Y * par.Site.Y) - (directixY * directixY))
                / denominator1;

            double x = point.X;
            // i have chosen to use left parabola to get Y coor;
            double y = a1 * x * x + b1 * x + c1;

            return new Vector2((float)x, (float)y);
        }

        public Vector2 getParabalaeIntersection(Parabola left, Parabola right, double directixY)
        {
            if (left == null || right == null || left.Site == right.Site)
                throw new Exception("NoIntersectoinFound");


            double denominator1 = 2.0f * (left.Site.Y - directixY);
//#warning quickfix
            if (denominator1 == 0) denominator1 = 0.000001;
            double a1 = 1.0f / denominator1;
            double b1 = -2.0f * left.Site.X / denominator1;
            double c1 = ((left.Site.X * left.Site.X) + (left.Site.Y * left.Site.Y) - (directixY * directixY))
                / denominator1;
            //y + dp / 4 + px * px / dp
            double denominator2 = 2.0f * (right.Site.Y - directixY);
//#warning quickfix
            if (denominator2 == 0) denominator2 = 0.000001;
            double a2 = 1.0f / denominator2;
            double b2 = -2.0f * right.Site.X / denominator2;
            double c2 = ((right.Site.X * right.Site.X) + (right.Site.Y * right.Site.Y) - (directixY * directixY))
                / denominator2;


            double a = a1 - a2;
            double b = b1 - b2;
            double c = c1 - c2;

            double x1 = 0, x2 = 0;
            if (a == 0)// 0 = bx +c
            {
                x1 = -c / b;
                x2 = x1;
            }
            else //0 = ax^2 + bx + c
            {
                double disc = b * b - 4 * a * c;
                if (disc < 0)// when everything is ok. this case become imposible
                {

                    throw new InvalidOperationException("Negative discriminant root not found in real domain");
      
                }
                x1 = (-b + JryMath.Sqrt(disc)) / (2 * a);
                x2 = (-b - JryMath.Sqrt(disc)) / (2 * a);
            }

            double x;
            if (left.Site.Y < right.Site.Y)
                x = JryMath.Max(x1, x2);
            else x = JryMath.Min(x1, x2);

            double y;
            // i have chosen to use left parabola to get Y coor;
            y = a1 * x * x + b1 * x + c1;

            return new Vector2((float)x, (float)y);
        }


        /// <summary>
        /// Finishes unfinished segments
        /// </summary>
        /// <param name="maxsize"></param>
        private void finishSegments(float maxsize)
        {
            foreach (var s in segs)
            {
                if (!s.End.HasValue)
                {
                    s.End = scaleSegment(s.Start.Value, s.Direction.Value);
                }
            }
        }

        /// <summary>
        /// Scale segment in order to touch boundaries
        /// </summary>
        /// <param name="p"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private Vector2 scaleSegment(Vector2 p, Vector2 dir)
        {
            var s = dir.Y / dir.X;
            double coef = 0;
            if (s <= 1 && s >= 0)//means that the boundaries are on left and right
            {
                if (dir.X < 0) //left
                    coef = (p.X - _minSize) / dir.X;
                else
                    coef = (p.X - _maxSize) / dir.X;
            }
            else// means that the boundaries are on the top and bottom
            {
                if (dir.Y < 0) //left
                    coef = (p.Y - _minSize) / dir.Y;
                else
                    coef = (p.Y - _maxSize) / dir.Y;
            }
            dir *= (float)Math.Abs(coef);


            return p + dir;
        }

        /// <summary>
        /// this comparer is used when digging values from heap.
        /// </summary>
        private class FortunePointComparer : IComparer<Event>
        {
            public int Compare(Event x, Event y)
            {
                if (x == null || y == null)
                    throw new System.ArgumentException("Can not compare nulls");


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

                    if (x.IsCircleEvent)
                        return 1;
                    if (y.IsCircleEvent)
                        return -1;

                    return 0;
                }

            }
        }

    }


}
