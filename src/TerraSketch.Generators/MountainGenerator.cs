using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Common.DataObjects.Geometry;
using Common.MathUtils;
using TerraSketch.Generators.Abstract;
using TerraSketch.Layer;
using TerraSketch.Logging;

namespace TerraSketch.Generators
{
    public class MountainGenerator : ABaseGenerator
    {
        private readonly IVisualLogger _logger;
        private readonly IVoronoiAreaGenerator _voronoiAreGenerator;
        private readonly IVoronoiAreaDrawer _voronoiAreaDrawer;
        private readonly AreaLoggerWithFill _areaLoggerWithFill;

        public MountainGenerator(IVisualLogger logger, IVoronoiAreaGenerator voronoi, INoise np, ILayerLocalParameters lp) : base(np, lp)
        {
            _logger = logger;
            _voronoiAreGenerator = voronoi;
            _voronoiAreaDrawer = new VoronoiAreaDrawer();

            // single purpouse logger. No need to be injected.
            _areaLoggerWithFill = new AreaLoggerWithFill(logger);
        }


        public override Task<ILayerMasked> GenerateLayer()
        {
            var pts = _layerLocalParameters.Polygon.Points.ToList();
            var bounds = new GeometryUtils().GetBoundsForPoints(pts);
            var mask = obtainBluredMask( pts,
                _layerLocalParameters.BlurSize,
                _layerLocalParameters.ExtendSize);


            var size = mask.Resolution;
            int count = (int)JryMath.Max( size.X,size.Y ) / 100 +1;
            var areas = _voronoiAreGenerator.GenerateAreas(size, bounds.GetSize(), count);

            _areaLoggerWithFill.LogAreas(size, areas, "loggedAreasFilled");

            var layer = new Layer2DObject(new Vector2(size.X + _layerLocalParameters.ExtendSize, size.Y + _layerLocalParameters.ExtendSize));
            var layerSize = layer.Resolution;
            var deltaX = (layerSize.X - size.X) / 2;
            var deltaY = (layerSize.Y - size.Y) / 2;

            var translateVector = new Vector2(deltaX, deltaY);

            _voronoiAreaDrawer.PrintToLayer(layer, areas, translateVector);
            layer.Mask = mask;
            _logger.Log(layer, "mountain");
            applyOffset(_layerLocalParameters.ExtendSize, _layerLocalParameters.Polygon.Points.ToList(), layer);
            _layUtils.Normalize(layer);
            layer.Mask = mask;
            return Task.FromResult((ILayerMasked)layer);
        }
    }


}
