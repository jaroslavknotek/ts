using System.Numerics;

namespace Voronoi.Generator.FortuneObjects
{
    public class Parabola
    {
        public Parabola()
        {

        }
        public Parabola(Vector2 point)
        {
            this.Site = point;
        }
        public Vector2 Site { get; set; }
        public CircleEvent CircleEvent { get; set; }

        public Segment Segment { get; set; }


        #region To be reimplemented as a tree
        public bool IsLeaf { get { return LeftParabola == null && RightParabola == null; } }
        public Parabola Parent { get; set; }

        public Segment GetLeftSegment()
        {
            var p = GetLeftParent();
            if (p == null) return null;
            return p.Segment;
        }

        public Segment GetRightSegment()
        {
            var p = GetRightParent();
            if (p == null) return null;
            return p.Segment;
        }


        private Parabola LeftParabola { get; set; }
        private Parabola RightParabola { get; set; }


        public Parabola GetRightParent()
        {
            var parent = Parent;
            var lastVisited = this;
            while (parent.GetRightNode() == lastVisited)
            {
                if (parent.Parent == null) return null;
                lastVisited = parent;
                parent = parent.Parent;
            }
            return parent;
        }
        public Parabola GetLeftParent()
        {
            var parent = Parent;
            var lastVisited = this;
            while (parent.GetLeftNode() == lastVisited)
            {
                if (parent.Parent == null) return null;
                lastVisited = parent;
                parent = parent.Parent;
            }
            return parent;
        }

        public Parabola GetLeftNode()
        {
            return LeftParabola;
        }

        public Parabola GetRightNode()
        {
            return RightParabola;
        }


        public Parabola GetLeftChildLeaf()
        {
            if (IsLeaf) return null;


            Parabola par = GetLeftNode();
            while (!par.IsLeaf) par = par.GetRightNode();
            return par;
        }

        public Parabola GetRightChildLeaf()
        {
            if (IsLeaf) return null;


            Parabola par = GetRightNode();
            while (!par.IsLeaf) par = par.GetLeftNode();
            return par;
        }
        /// <summary>
        /// Returns parabola on the right to this on a beachline. 
        /// Works only when its called on leaf parabola otherwise it does not make a sense
        /// </summary>
        /// <returns></returns>
        public Parabola GetLeafParabolaOnTheLeft()
        {
            Parabola p = null;

            if (this.IsLeaf && Parent != null)
            {
                p = GetLeftParent();
            }

            if (p == null) return null;

            Parabola par = p.GetLeftNode();
            while (!par.IsLeaf) par = par.GetRightNode();
            return par;

        }
        /// <summary>
        /// Returns parabola on the left to this on a beachline. 
        /// Works only when its called on leaf parabola otherwise it does not make a sense
        /// </summary>
        /// <returns></returns>
        public Parabola GetLeafParabolaOnTheRight()
        {
            Parabola p = null;
            if (this.IsLeaf && Parent != null)
            {
                p = GetRightParent();
            }

            if (p == null) return null;
            Parabola par = p.GetRightNode();
            while (!par.IsLeaf) par = par.GetLeftNode();
            return par;

        }

        public void SetLeftParabola(Parabola p, Segment s)
        {
            SetLeftParabola(p);
            Segment = s;
        }

        public void SetLeftParabola(Parabola p)
        {
            p.Parent = this;
            this.LeftParabola = p;
        }

        public void SetRightParabola(Parabola p, Segment s)
        {

            SetRightParabola(p);
            Segment = s;
        }
        public void SetRightParabola(Parabola p)
        {
            p.Parent = this;
            RightParabola = p;
        }
        #endregion

        public override string ToString()
        {
            return Site.ToString();
        }


    }
}
