
using System;
using System.Numerics;

namespace Voronoi.Generator.FortuneObjects
{
    public abstract class Event
    {
        public abstract float X
        {
            get;
        }

        public abstract float Y
        {
            get;
            
        }

        public static implicit operator Vector2(Event e)
        {
            return new Vector2(e.X, e.Y);
        }


//#error fuuuck
        public Boolean IsCircleEvent
        {
            get { return (this is CircleEvent); }
            
        }
        
    }
}