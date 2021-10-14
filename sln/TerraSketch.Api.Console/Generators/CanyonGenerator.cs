using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Common.DataObjects.Geometry;
using TerraSketch.Generators.Abstract;
using TerraSketch.Layer;
using TerraSketch.Logging;

namespace TerraSketch.Generators
{
    public class CanyonGenerator : ABaseGenerator
    {
        private readonly IVisualLogger _logger;
        private readonly LayerUtility _layerUtility = new LayerUtility();
        public CanyonGenerator(IVisualLogger logger, INoise np, ILayerLocalParameters lp) : base(np, lp)
        {

            _logger = logger;
        }




        public override Task<ILayerMasked> GenerateLayer()
        {

            var oldpoints = _layerLocalParameters.Polygon.Points.ToList();
            var mask = getBlurredMask(oldpoints);
            Vector2 resolution = mask.Resolution;
            var noisescaleDown = .33f;
            const float thatMakesItGoDown = -1;

            _logger.Log(mask, "canyonMask");

            var blured = getBlurredProfile(oldpoints, resolution);
            var layer = _noise.Do(resolution);

            _layerUtility.IterateValues(blured, (coor, val) =>
            {
                var cv = val.HasValue ? val.Value : 0;

                layer[coor] = layer[coor] * noisescaleDown + cv + thatMakesItGoDown;
            });
            _logger.Log(layer, "canyon");
            layer.Mask = mask;

            applyOffset(_layerLocalParameters.ExtendSize, oldpoints, layer);
            return Task.FromResult(layer);
        }

        private IMask getBlurredMask(List<Vector2> oldpoints)
        {
            IMask mask = obtainBluredMask(oldpoints, _layerLocalParameters.BlurSize, _layerLocalParameters.ExtendSize);
            mask = _layerUtility.Blur(mask, 5);
            mask = _layerUtility.Blur(mask, 5);
            return mask;
        }

        private ILayerMasked getBlurredProfile(List<Vector2> oldpoints, Vector2 resolution)
        {
            var canyonProfile = getCanyonProfile(oldpoints, resolution);


            _layerUtility.Normalize(canyonProfile);
            _logger.Log(canyonProfile, "canyonProfile");
            var blured = _layerUtility.Blur(canyonProfile, 5);
            blured = _layerUtility.Blur(blured, 5);
            blured = _layerUtility.Blur(blured, 5);

            _logger.Log(blured, "bluredProfile");
            return blured;
        }


        // TODO create new generator that crates gradient primitives!
        private static Layer2DObject getCanyonProfile(List<Vector2> oldpoints, Vector2 resolution)
        {
            // polygon has to be centered
            var oldcenter = oldpoints.Aggregate((a, b) => a + b) / oldpoints.Count;
            var maskCenter = resolution * .5f;
            var ptTranslateVector = oldcenter - maskCenter;
            //var ptTranslateVector = Vector2.Zero;
            var points = oldpoints.Select(r => r - ptTranslateVector).ToList();
            var center = points.Aggregate((a, b) => a + b) / points.Count;
            var v = new VerySpecificQuadraticVoronoiAreaDrawer();
            var canyonProfile = new Layer2DObject(resolution);

            var segs = points.Select((t, i) => new LineSegment(t, points[(i + 1) % points.Count])).ToList();
            v.PrintToLayer(canyonProfile, new List<IArea>()
            {
                new Area()
                {
                    Center = center,
                    Points = points,
                    Segments = segs
                }
            }, Vector2.Zero);
            return canyonProfile;
        }
    }


}
