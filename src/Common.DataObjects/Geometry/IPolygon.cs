using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Common.DataObjects.Geometry;

namespace Common.DataObjects.Geometry
{
    public interface IPolygon {

        IEnumerable<Vector2> Points { get; }
        int PointsCount { get; }
    }
}
