using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Common.DataObjects.Geometry;

namespace Common.DataObjects.Utils
{
    public class AreaFactory
    {
        private SegmentSort segmentSort = new SegmentSort();
        public IArea instantiate(Vector2 center, IList<LineSegment> lineSegments)
        {
            var are = (IArea)new Area()
            {
                Center = center,
                Segments = lineSegments
            };
            var sortedSegments = segmentSort.Sort(are.Segments);
            are.Segments = sortedSegments.ToList();
            are.Points = sortedSegments.Select(r => r.Point1).ToList();
            return are;
        }
    }
}
