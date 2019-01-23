using System;
using TerraSketch.Generators.Abstract;

namespace TerraSketch.FluentBuilders
{
    public abstract class AGeneratorBuilder : IGeneratorBuilder
    {
        protected float _influence = 1;
        protected INoiseBuilder _noiseBuilder;
        protected ILayerBuilder _layerBuilder;
        public IGeneratorBuilder Influence(float influence)
        {
            _influence = influence;
            return this;
        }

        public IGeneratorBuilder Noise(Action<INoiseBuilder> noiseBuildingAction)
        {
            _noiseBuilder = new NoiseBuilder();
            noiseBuildingAction(_noiseBuilder);
            return this;
        }

        public IGeneratorBuilder Layer(Action<ILayerBuilder> layerBuildingAction)
        {
            _layerBuilder = new LayerBuilder();
            layerBuildingAction(_layerBuilder);
            return this;
        }

        public abstract ISubGenerator Build(int seed);

        public IGeneratorBuilder Noise(INoiseBuilder noiseBuilding)
        {
            _noiseBuilder = noiseBuilding;
            return this;
        }

        public IGeneratorBuilder Layer(ILayerBuilder layerBuilding)
        {
            _layerBuilder = layerBuilding;
            return this;
        }

        protected void checkValidBuilderSetup()
        {
            if (_layerBuilder == null)
                throw new InvalidBuildStateException("Missing layer initialization");
            if (_noiseBuilder == null)
                throw new InvalidBuildStateException("Missing noise initialization");

        }
    }
}