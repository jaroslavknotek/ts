using System.Collections.Generic;
using System.Numerics;
using Common.DataObjects.Geometry;
using TerraSketch.Layer;

namespace TerraSketch.Generators
{
    public interface IVoronoiAreaDrawer
    {
        void PrintToLayer(ILayerMasked layer, IList<IArea> areas, Vector2 translateVector);
    }
}