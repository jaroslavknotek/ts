using TerraSketch.Generators.Abstract;
using TerraSketch.Layer.BlendModes;

namespace TerraSketch.Generators
{
    public class LayerGlobalParameters : ILayerGlobalParameters
    {
        public IGenerator Generator { get; set; }

        public IBlendMode BlendMode { get; set; }
        public float Offset { get; set; }

    }
}
