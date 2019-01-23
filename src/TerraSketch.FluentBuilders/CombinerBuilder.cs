using System;
using System.Collections.Generic;
using System.Linq;
using TerraSketch.Generators;
using TerraSketch.Generators.Abstract;
using TerraSketch.Logging;

namespace TerraSketch.FluentBuilders
{
    public class CombinerBuilder : IGeneratorBuilder
    {
        private ILayerBuilder _layerBuilder;
        private readonly IList<IGeneratorBuilder> builders = new List<IGeneratorBuilder>();
        
        private INoiseBuilder _noiseBuilder;

        public int BuilderCount => builders.Count;

        public CombinerBuilder Add(IGeneratorBuilder generatorBuilder)
        {
            builders.Add(generatorBuilder);
            return this;
        }

      

        public CombinerBuilder Generator<T>(Action<T> mountainGeneratorBuildAction)
            where T : IGeneratorBuilder, new()

        {
            var generator = new T();
            mountainGeneratorBuildAction(generator);
            builders.Add(generator);
            return this;
        }

        public ISubGenerator Build(int seed)
        {
            if (_layerBuilder == null)
                throw new InvalidBuildStateException("layer parameters was not set");
            if (_noiseBuilder == null)
                throw new InvalidBuildStateException("noise parameters was not set");

            var visualLogger = new VisualLogger();
            var gens = builders.Select(r =>
            {
                r.Layer(_layerBuilder);
                r.Noise(_noiseBuilder);

                return r.Build(seed);
            }
            ).ToList();
            return new CombinedGenerator(gens,_layerBuilder.Build() ,visualLogger);
        }

        public IGeneratorBuilder Influence(float influence)
        {
            // todo use when generator implementation is expanded
            return this;
        }

        public IGeneratorBuilder Noise(Action<INoiseBuilder> noiseBuildingAction)
        {
            _noiseBuilder =new NoiseBuilder();
            noiseBuildingAction(_noiseBuilder);
            return this;
        }

        public IGeneratorBuilder Layer(Action<ILayerBuilder> layerBuildingAction)
        {
            _layerBuilder = new LayerBuilder();
            layerBuildingAction(_layerBuilder);
            return this;
        }

        public IGeneratorBuilder Noise(INoiseBuilder noiseBuilding)
        {
            this._noiseBuilder = noiseBuilding;
            return this;
        }

        public IGeneratorBuilder Layer(ILayerBuilder layerBuilding)
        {
            _layerBuilder = layerBuilding;
            return this;
        }
    }
}