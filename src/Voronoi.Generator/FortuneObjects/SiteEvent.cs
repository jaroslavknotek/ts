using System.Numerics;
namespace Voronoi.Generator.FortuneObjects
{
    public class SiteEvent : Event
    {
        private Vector2 _vec;

        public SiteEvent( Vector2 v)
        {
            _vec = v;
        }
        public override float X
        {
            get
            {
                return _vec.X;
            }
            
        }

        public override float Y
        {
            get
            {
                return _vec.Y;
            }
        }

        public static implicit operator Vector2(SiteEvent e)
        {
            return new Vector2(e.X, e.Y);
        }



    }
}
