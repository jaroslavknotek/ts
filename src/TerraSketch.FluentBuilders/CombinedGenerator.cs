using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using TerraSketch.Generators;
using TerraSketch.Generators.Abstract;
using TerraSketch.Layer;
using TerraSketch.Logging;

namespace TerraSketch.FluentBuilders
{
    public class CombinedGenerator : ABaseGenerator
    {
        private readonly IList<ISubGenerator> _generators;
        private readonly LayerUtility layUtils = new LayerUtility();
        private readonly IVisualLogger _logger;
        
        public CombinedGenerator(IList<ISubGenerator> generators, ILayerLocalParameters layerLocalParameters
            , IVisualLogger vl)
        {
            _layerLocalParameters = layerLocalParameters;
            _logger = vl;
            _generators = generators;
        }
        public override async Task<ILayerMasked> GenerateLayer()
        {
            var layers = await generateLayers();
            var x = layers.Min(r => r.Layer.Resolution.X);
            var y = layers.Min(r => r.Layer.Resolution.Y);
            var size = new Vector2(x, y);
            var merged = new Layer2DObject(x, y);
            
            // move to layerUtils class
            foreach (var layerWithInfluence in layers)
            {
                var inf = layerWithInfluence.Influence;
                var layer = layerWithInfluence.Layer;
                // TODO use paralallel and atomic addition
                // disable cache
                for (int i = 0; i < size.Y; i++)
                {
                    for (int j = 0; j < size.X; j++)
                    {
                        var influencedValue = inf * layer[j, i];
                        var mergedValue = merged[j, i];
                        merged[j, i] =
                            mergedValue.HasValue ?
                                mergedValue + influencedValue :
                                influencedValue;
                    }
                }
            }
            merged.Mask = obtainBluredMask(_layerLocalParameters.Polygon.Points.ToArray()
                , _layerLocalParameters.BlurSize
                , _layerLocalParameters.ExtendSize);

            applyOffset(_layerLocalParameters.ExtendSize
                , _layerLocalParameters.Polygon.Points.ToArray()
                , merged);
            _logger.Log(merged, "combined");
            return merged;
        }

        private async Task<LayerWithInfluence[]> generateLayers()
        {
            var layers = new LayerWithInfluence[_generators.Count];

            for (int i = 0; i < _generators.Count; i++)
            {
                var layer = await _generators[i].GenerateLayer();
                layUtils.Normalize(layer);
                var inf = _generators[i].Influence;
                layers[i] = new LayerWithInfluence(layer, inf);
            }
            return layers;
        }



        private struct LayerWithInfluence
        {
            public LayerWithInfluence(ILayerMasked layer, float influence)
            {
                Layer = layer;
                Influence = influence;
            }

            public float Influence { get; }
            public ILayerMasked Layer { get; }
        }
    }
}