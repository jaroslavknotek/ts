using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Common.DataObjects.Geometry;
using Common.MathUtils;
using TerraSketch.Generators.Abstract;
using TerraSketch.Layer;

namespace TerraSketch.Generators
{
    // TODO analyze influnce
    // it should not be visible from generator.
   public abstract class ABaseGenerator : ISubGenerator
    {
        public float Influence { get; set; }
        protected readonly GeometryUtils _geoUtils = new GeometryUtils();
        protected readonly LayerUtility _layUtils = new LayerUtility();
        protected readonly INoise _noise;

        protected ILayerLocalParameters _layerLocalParameters;

        protected ABaseGenerator( INoise noise, ILayerLocalParameters layerLocalParameters)
        {
            _noise = noise;
            _layerLocalParameters = layerLocalParameters;
        }

        protected ABaseGenerator()
        {
        }

        public abstract Task<ILayerMasked> GenerateLayer();

        protected void applyOffset(int extendSize, IList<Vector2> points, ILayerMasked layer)
        {
            var bounds = _geoUtils.GetBoundsForPoints(points);
            layer.Offset = new IntVector2(bounds.Left - extendSize, bounds.Top - extendSize);
        }

        protected IMask obtainBluredMask(IList<Vector2> points, int blurSize, int extendSize)
        {
            IMask mask = _layUtils.GetMaskForPoints(points);
            var layer = _layUtils.ExtendLayer(mask,extendSize);
            layer = _layUtils.Blur(layer, blurSize);
            layer = _layUtils.Blur(layer, blurSize);
            layer = _layUtils.Blur(layer, 15);
            _layUtils.Normalize(layer);
            return layer;
        }



    }
}
