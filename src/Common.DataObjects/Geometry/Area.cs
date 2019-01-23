using System.Collections.Generic;
using System.Numerics;

namespace Common.DataObjects.Geometry
{
    public class Area : IArea
    {
        public Vector2 Center { get; set; }
        public IList<LineSegment> Segments { get; set; }
        public IList<Vector2> Points { get; set; }

#if DEBUG
        public override string ToString()
        {
            return $"{Center}";
        }
#endif
    }
}
