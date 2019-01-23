using System.Numerics;

namespace TerraSketch.Generators.Abstract
{
    public interface IDetailParameterEnhancer
    {
        void Enhance(INoiseParametersDetails np);
    }
    
}
