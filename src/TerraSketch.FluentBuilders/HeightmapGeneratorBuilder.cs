using Common.MathUtils.Probability;
using TerraSketch.Generators;
using TerraSketch.Generators.Abstract;

namespace TerraSketch.FluentBuilders
{
    public class HeightmapGeneratorBuilder : AGeneratorBuilder
    {
        public override ISubGenerator Build(int seed)
        {
            checkValidBuilderSetup();
            var np = _noiseBuilder.Random(new Rand(seed)).Build();
            var lp = _layerBuilder.Build();


            return new HeightmapGenerator(np, lp)
            {
                Influence = _influence
            };
        }
    }
}