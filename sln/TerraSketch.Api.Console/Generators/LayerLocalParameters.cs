using Common.DataObjects.Geometry;
using TerraSketch.Generators.Abstract;

namespace TerraSketch.Generators
{
    public class LayerLocalParameters : ILayerLocalParameters
    {
        public int BlurSize { get; set; }

        public IPolygon Polygon { get; set; }


        public int ExtendSize { get; set; }
        public bool HasMask { get; set; }

        public LayerLocalParameters()
        {
            BlurSize = 20;

            ExtendSize = BlurSize * 5;
        }
    }
}