using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Common.DataObjects.Geometry;

namespace Common.DataObjects.Utils
{
    public class SegmentSort :ISegmentSort
    {

        public IEnumerable<LineSegment> Sort(IEnumerable<LineSegment> segments)
        {
            var segs = segments.ToArray();
            int segsLength = segs.Length;

            if (segsLength<=1)
                return segs;
            
            for (int i = 0; i < segsLength - 1; i++)
            {
                var selected = segs[i];
                for (int j = i + 1; j < segsLength ; j++)
                {
                    var compared = segs[j];
                    var seg = GetValue(selected, compared);
                    if (!seg.HasValue) continue;

                    segs[j] = segs[i + 1];
                    segs[i + 1] = seg.Value;
                    break;

                }
            }

            //handle last  one
            var sel = segs[segsLength - 2];
            var comp = segs[segsLength - 1];
            var lineSegment = GetValue(sel, comp);
            if (lineSegment != null) segs[segsLength - 1] = lineSegment.Value;

            return segs;
        }

        private LineSegment? GetValue(LineSegment selected, LineSegment compared)
        {
            //return prepared segment

            //var p = getConsecutivePoint(selected.Point1, compared);
            //if (p.HasValue)
            //    if (p == 1)
            //        return compared;
            //    else
            //        return swapPoints(compared);

            var q = getConsecutivePoint(selected.Point2, compared);
            if (q.HasValue)
                if (q == 1)
                    return compared;
                else
                    return swapPoints(compared);

            return null;
        }

        private static LineSegment swapPoints(LineSegment compared)
        {
            return new LineSegment(compared.Point2, compared.Point1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="compared"></param>
        /// <returns>Integer identifier of a point that is consecutive</returns>
        private static int? getConsecutivePoint(Vector2 temp, LineSegment compared)
        {
            if (compared.Point1 == temp)
                return 1;
            if (compared.Point2 == temp)
                return 2;

            return null;
        }
    }

    public interface ISegmentSort
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments">Not sorted</param>
        /// <returns>Segments sorted in polygonal manner. Segments are sequence that forms polygon</returns>
        IEnumerable<LineSegment> Sort(IEnumerable<LineSegment> segments);
    }
}
