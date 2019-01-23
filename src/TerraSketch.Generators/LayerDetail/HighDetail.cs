using TerraSketch.DataObjects.FieldObjects.FieldParams;
using TerraSketch.Generators.Abstract;

namespace TerraSketch.Generators.LayerDetail
{
    public class HighDetail : AFieldDetail, IDetailParameterEnhancer
    {
        public override string Caption => "High";

        public void Enhance(INoiseParametersDetails np)
        {
            np.FromDepth = 4;
            np.ToDepth = 12;
            np.Amplitude = .5f;
            np.BaseAmplitude = 1;
            np.Lacunarity = 2.0f;
        }
    }
}