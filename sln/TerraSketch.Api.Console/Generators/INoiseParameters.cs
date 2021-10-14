using Common.MathUtils.Probability;

namespace TerraSketch.Generators.Abstract
{

    public interface INoiseParametersDetails
    {
        int FromDepth { get; set; }
        int ToDepth { get; set; }
        float Lacunarity { get; set; }
        float Amplitude { get; set; }
        float BaseAmplitude { get; set; }
    }

    public interface INoiseParameters : INoiseParametersDetails
    {
        IRandom0 Random { get; set; }
    }
}