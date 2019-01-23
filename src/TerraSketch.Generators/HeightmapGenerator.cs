using System.Numerics;
using System.Threading.Tasks;
using System.Linq;
using TerraSketch.Generators.Abstract;
using TerraSketch.Layer;
namespace TerraSketch.Generators
{

    public class HeightmapGenerator : ABaseGenerator
    {

        public HeightmapGenerator(INoise np, ILayerLocalParameters lp) : base(np, lp) { }




        public override Task<ILayerMasked> GenerateLayer()
        {
            return Task.Factory.StartNew(() =>
            {
                var points = _layerLocalParameters.Polygon.Points.ToList();

//#warning performance loss. Blur alghorimt contatins error that cause ubalanced blur on a right border which restulst in artifacts.


                IMask mask = obtainBluredMask(points, _layerLocalParameters.BlurSize, _layerLocalParameters.ExtendSize);
                Vector2 resolution = mask.Resolution;
                var layer = _noise.Do(resolution);
                layer.Mask = mask;
                _layUtils.Normalize(layer);
                applyOffset(_layerLocalParameters.ExtendSize, points, layer);
                return layer;
            });
        }


    }
}
