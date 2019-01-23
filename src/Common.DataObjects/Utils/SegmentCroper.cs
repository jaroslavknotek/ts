using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Common.DataObjects.Geometry;
using Common.MathUtils;

namespace Common.DataObjects.Utils
{
    public interface ISegmentCroper
    {
        IList<LineSegment> CropSegmentsByArea(IList<LineSegment> subAreaSegments, IArea parentArea
);
    }

    public class SegmentCroper : ISegmentCroper
    {
        private readonly GeometryUtils _geometryUtilty;
        public SegmentCroper(GeometryUtils geometryUtils)
        {
            _geometryUtilty = geometryUtils;
        }

        public IList<LineSegment> CropSegmentsByArea(IList<LineSegment> subAreaSegments, IArea parentArea
)
        {
            // TODO refactor once it is not a prototype

            var cropedSegments = new List<LineSegment>();
            foreach (var subareaSegment in subAreaSegments)
            {
                LineSegment? potentionalyCropedSegment = subareaSegment;
                foreach (var parentAreaSegment in parentArea.Segments)
                {
                    var intersectionPoint = _geometryUtilty.GetSegmentsIntersection(parentAreaSegment, subareaSegment);
                    if (intersectionPoint.HasValue)
                    {
                        if(_geometryUtilty.IsInPolygon(parentArea.Points.ToArray(), subareaSegment.Point1))
                            potentionalyCropedSegment=new LineSegment(intersectionPoint.Value, subareaSegment.Point1);
                        else
                            potentionalyCropedSegment = new LineSegment(intersectionPoint.Value, subareaSegment.Point2);
                    }
                    else 
                    {
                        // is on the same halfplane
                        bool isOnTheSameHalfPlane =
                            _geometryUtilty.IsInPolygon(parentArea.Points.ToArray(), subareaSegment.Point1)
                            && _geometryUtilty.IsInPolygon(parentArea.Points.ToArray(), subareaSegment.Point1);
                        if (!isOnTheSameHalfPlane)
                        {
                            potentionalyCropedSegment = null;
                            break;
                            
                        }
                    }

                }

                if(potentionalyCropedSegment.HasValue)
                cropedSegments.Add(potentionalyCropedSegment.Value);
            }

            return cropedSegments;
        }

        public LineSegment CropSegmentBySize(IntVector2 size, LineSegment r)
        {
            var p1 = cropBySize(r.Point1, size);
            var p2 = cropBySize(r.Point2, size);
            return new LineSegment(p1, p2);
        }

        public Vector2 cropBySize(Vector2 v, IntVector2 size)
        {
            var x = JryMath.Max(JryMath.Min(v.X, size.X), 0);
            var y = JryMath.Max(JryMath.Min(v.Y, size.Y), 0);
            return new Vector2(x, y);
        }

    }

}
