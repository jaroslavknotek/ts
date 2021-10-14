using Common.MathUtils.Probability;
using TerraSketch.Generators.Abstract;

namespace TerraSketch.Generators.Noise
{
    public class NoiseParameters : INoiseParameters
    {
        public float Lacunarity { get; set; }
        public int FromDepth { get; set; }
        public int ToDepth { get; set; }
        public float BaseAmplitude { get; set; }
        public float Amplitude { get; set; }

        public IRandom0 Random { get; set; }

        public NoiseParameters()
        {
            FromDepth = 0;
            ToDepth = 4;
            BaseAmplitude = 1;
            Amplitude = .5f;
            Lacunarity = 2;
        }
    }
}
