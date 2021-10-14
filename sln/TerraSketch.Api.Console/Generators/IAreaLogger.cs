using System.Collections.Generic;
using Common.DataObjects.Geometry;
using Common.MathUtils;

namespace TerraSketch.Generators
{
    public interface IAreaLogger
    {
        void LogAreas(IntVector2 size, IList<IArea> areas, string name);
    }
}