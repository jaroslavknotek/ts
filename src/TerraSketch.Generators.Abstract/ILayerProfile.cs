using System.Numerics;
using TerraSketch.Layer;

namespace TerraSketch.Generators.Abstract
{
    public interface ILayerProfile
    {
        IGeneratorBuilder GetBuilder();
    }   
}
