using System.Collections.Generic;
using Common.DataObjects.Geometry;
using Common.MathUtils;
using TerraSketch.Layer;
using TerraSketch.Logging;

namespace TerraSketch.Generators
{
    public class AreaLoggerWithFill : IAreaLogger
    {
        private readonly LayerUtility _layerUtility;
        private readonly IVisualLogger _logger;

        public AreaLoggerWithFill(IVisualLogger logger)
        {
            _layerUtility = new LayerUtility();
            _logger = logger;
        }

        public void LogAreas(IntVector2 size, IList<IArea> areas, string name)
        {
            var debugColoredAreas = drawSiteToLayer(size, areas);
            _logger.Log(debugColoredAreas, "debugColoredAreas");
        }

        private ILayerMasked drawSiteToLayer(IntVector2 size, IEnumerable<IArea> areas)
        {
            var layer = new Layer2DObject(size.X, size.Y);
            _layerUtility.IterateValues(layer, (x, y, va) => layer[x, y] = 0);
            var layerDrawer = new LayerDrawer(layer);
            foreach (var area in areas)
            {
                layerDrawer.FillArea(area);
            }
            return layer;
        }
    }

    public class AreaLogger : IAreaLogger
    {
        private readonly LayerUtility _layerUtility;
        private readonly IVisualLogger _logger;

        public AreaLogger(IVisualLogger logger)
        {
            _layerUtility = new LayerUtility();
            _logger = logger;
        }

        public void LogAreas(IntVector2 size, IList<IArea> areas, string name)
        {
            var debugColoredAreas = drawSiteToLayer(size, areas);
            _logger.Log(debugColoredAreas, name);
        }

        private ILayerMasked drawSiteToLayer(IntVector2 size, IEnumerable<IArea> areas)
        {
            var layer = new Layer2DObject(size.X, size.Y);
            _layerUtility.IterateValues(layer, (x, y, va) => layer[x, y] = 0);
            var layerDrawer = new LayerDrawer(layer);
            foreach (var area in areas)
            {
                layerDrawer.DrawArea(area);
            }
            return layer;
        }
    }
}