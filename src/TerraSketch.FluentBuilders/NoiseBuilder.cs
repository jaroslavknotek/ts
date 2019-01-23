using Common.MathUtils.Probability;
using TerraSketch.Generators.Abstract;
using TerraSketch.Generators.Noise;

namespace TerraSketch.FluentBuilders
{
    public class NoiseBuilder : INoiseBuilder
    {
        private readonly NoiseParameters _descriptor = new NoiseParameters();
        private INoiseFactory _noiseFactory = new PerlinNoiseFactory();

        public INoiseBuilder ApplyDetailLevel(IDetailParameterEnhancer enhancer)
        {
            enhancer.Enhance(_descriptor);
            return this;
        }
        public INoiseBuilder FromDepth(int from)
        {
            _descriptor.FromDepth = from;
            return this;
        }


        public INoiseBuilder ToDepth(int to)
        {
            _descriptor.ToDepth = to;
            return this;
        }

        public INoiseBuilder Lacunarity(float lacunarity)
        {
            _descriptor.Lacunarity = lacunarity;
            return this;
        }

        public INoiseBuilder Amplitude(float amplitude)
        {
            _descriptor.Amplitude = amplitude;
            return this;
        }

        public INoiseBuilder StartAmplitude(float startAmplitude)
        {
            _descriptor.BaseAmplitude = startAmplitude;
            return this;
        }

        public INoiseBuilder Random(IRandom0 rand)
        {
            _descriptor.Random = rand;
            return this;

        }

        public INoiseBuilder Perlin()
        {
            _noiseFactory = new PerlinNoiseFactory();
            return this;
        }

        public INoise Build()
        {
            return _noiseFactory.Create(_descriptor);
        }
        

        private interface INoiseFactory
        {
            INoise Create(INoiseParameters param);
        }
        private class PerlinNoiseFactory : INoiseFactory
        {
            public INoise Create(INoiseParameters param)
            {
                return new PerlinNoise(param);
            }
        }
    }
}