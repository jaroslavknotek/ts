using System.Numerics;
using TerraSketch.Layer;

namespace TerraSketch.Generators.Abstract
{

    public interface INoise
    {
        ILayerMasked Do(Vector2 size);
    }
}
