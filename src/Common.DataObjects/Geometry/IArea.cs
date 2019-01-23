using System.Collections.Generic;
using System.Numerics;

namespace Common.DataObjects.Geometry
{
    public interface IArea
    {
        /// <summary>
        /// Must be sorted the way that if i and i+1 forms a segment. 
        /// Then all segments forms original polygon.
        /// </summary>
        IList<Vector2> Points { get; set; }
        Vector2 Center { get; set; }
        IList<LineSegment> Segments { get; set; }

    }
}