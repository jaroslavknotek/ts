using Common.DataObjects.Geometry;
using TerraSketch.Generators;
using TerraSketch.Generators.Abstract;

namespace TerraSketch.FluentBuilders
{
    public class LayerBuilder : ILayerBuilder
    {
        private readonly LayerLocalParameters _descriptor = new LayerLocalParameters();

        public ILayerBuilder Blur(int blur)
        {
            _descriptor.BlurSize = blur;
            return this;

        }
        public ILayerBuilder Polygon(IPolygon poly)
        {
            _descriptor.Polygon = poly;
            return this;

        }

        public ILayerBuilder ExtendSize(int extendSize)
        {
            _descriptor.ExtendSize = extendSize;
            return this;

        }

        public ILayerLocalParameters Build()
        {
            validate();
            return _descriptor;
        }

        private void validate()
        {
            if (_descriptor?.Polygon?.Points == null)
            {
                throw new InvalidBuildStateException("No polygon");
            }
        }

        public ILayerBuilder WithoutMask()
        {
            _descriptor.HasMask = false;
            return this;
        }

        public ILayerBuilder WithMask()
        {
            _descriptor.HasMask = true;
            return this;
        }
    }
}