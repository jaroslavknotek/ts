using TerraSketch.DataObjects.FieldObjects.FieldParams;
using TerraSketch.Generators.Abstract;

namespace TerraSketch.Generators.LayerDetail
{
    public class LowDetail : AFieldDetail, IDetailParameterEnhancer
    {
        public override string Caption => "Low";

        public void Enhance(INoiseParametersDetails np)
        {
            np.FromDepth = 4;
            np.ToDepth = 10;
            np.Amplitude = .40f;
            np.BaseAmplitude = 1.0f;
            np.Lacunarity = 2.0f;
        }
    }
}