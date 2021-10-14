using System.Collections.Generic;
using Common.DataObjects.Geometry;
using Common.MathUtils;

namespace TerraSketch.Generators
{
    public interface IVoronoiAreaGenerator
    {
        IList<IArea> GenerateAreas(IntVector2 size, IntVector2 sizeToGenerate, int countOfCells);
    }
}