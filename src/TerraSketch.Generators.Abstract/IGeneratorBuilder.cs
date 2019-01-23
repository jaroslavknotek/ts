using System;
using Common.DataObjects.Geometry;
using Common.MathUtils.Probability;
using TerraSketch.Layer.BlendModes;

namespace TerraSketch.Generators.Abstract
{
    public interface IGeneratorBuilder
    {
        IGeneratorBuilder Influence(float influence);
        IGeneratorBuilder Noise(Action<INoiseBuilder> noiseBuildingAction);
        IGeneratorBuilder Noise(INoiseBuilder noiseBuilding);
        IGeneratorBuilder Layer(Action<ILayerBuilder> layerBuildingAction);
        IGeneratorBuilder Layer(ILayerBuilder layerBuilding);
        ISubGenerator Build(int seed);
    }

    public interface ILayerBuilder
    {
        
        ILayerBuilder Blur(int blur);
        ILayerLocalParameters Build();
        ILayerBuilder ExtendSize(int extendSize);
        ILayerBuilder Polygon(IPolygon poly);

        ILayerBuilder WithoutMask();
        ILayerBuilder WithMask();
    }

    public interface INoiseBuilder
    {
        INoiseBuilder Amplitude(float amplitude);
        INoiseBuilder ApplyDetailLevel(IDetailParameterEnhancer enhancer);
        INoise Build();

        INoiseBuilder FromDepth(int from);
        INoiseBuilder Lacunarity(float lacunarity);
        INoiseBuilder Perlin();
        INoiseBuilder Random(IRandom0 rand);
        INoiseBuilder StartAmplitude(float startAmplitude);
        INoiseBuilder ToDepth(int to);
    }
}