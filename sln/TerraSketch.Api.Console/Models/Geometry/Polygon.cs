using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Common.DataObjects.Geometry
{
    public class Polygon : IPolygon
    {
        private IList<Vector2> _points;

        public IEnumerable<Vector2> Points
        {
            get { return _points; }

            set { _points = value.ToList(); }
        }
        public int PointsCount => _points.Count;
    }
}
