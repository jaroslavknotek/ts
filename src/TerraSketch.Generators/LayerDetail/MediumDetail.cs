using System.Numerics;
using TerraSketch.DataObjects.FieldObjects.FieldParams;
using TerraSketch.Generators.Abstract;

namespace TerraSketch.Generators.LayerDetail
{
    public class MediumDetail : AFieldDetail, IDetailParameterEnhancer
    {
        public override string Caption => "Medium";


        public void Enhance(INoiseParametersDetails np)
        {
            np.FromDepth = 4;
            np.ToDepth = 11;
            np.Amplitude = .5f;
            np.BaseAmplitude = 1;
            np.Lacunarity = 2.0f;
        }
    }
}