using Common.MathUtils.Probability;
using TerraSketch.Generators;
using TerraSketch.Generators.Abstract;
using TerraSketch.Logging;
using Voronoi.Generator;

namespace TerraSketch.FluentBuilders
{
    public class MounatinGeneratorBuilder : AGeneratorBuilder
    {
        public override ISubGenerator Build(int seed)
        {
            
            var np = _noiseBuilder.Random(new Rand(seed)).Build();
            var lp = _layerBuilder.Build();
            // TODO change rand approach
            var rnd2 = new RandomSeeded(seed);
            var sd = new SegmentDivider(rnd2,30, 1);
            var c = new VoronoiConverter(sd);
            var g = new VoronoiGenerator(c);
            var gg = new VoronoiAreaGenerator(g, new Rand(seed));
            // todo Use parameters
            return new MountainGenerator(new VisualLogger(), gg, np, lp)
            {
                Influence = _influence
            };
        }
    }

    public class CanyonGeneratorBuilder:AGeneratorBuilder
    {
        public override ISubGenerator Build(int seed)
        {
            var np = _noiseBuilder.Random(new Rand(seed)).Build();
            var lp = _layerBuilder.Build();
            
            return new CanyonGenerator(new VisualLogger(), np, lp)
            {
                Influence = _influence
            };
        }
    }
}