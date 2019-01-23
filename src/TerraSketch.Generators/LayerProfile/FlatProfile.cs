
using TerraSketch.DataObjects.FieldObjects.FieldParams;
using TerraSketch.Generators.Abstract;

namespace TerraSketch.Generators.LayerProfile
{
    public class FlatProfile : AFieldProfile, ILayerProfile
    {
        public override string Caption => "Flat";

        private readonly IGeneratorBuilder _builder;

        public FlatProfile(IGeneratorBuilder builder)
        {
            _builder =builder;
        }
        public IGeneratorBuilder GetBuilder()
        {

            return _builder;
        }
    }
}
