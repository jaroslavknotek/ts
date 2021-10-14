using TerraSketch.DataObjects.FieldObjects.FieldParams;
using TerraSketch.Generators.Abstract;

namespace TerraSketch.Generators.LayerProfile
{

    public class CanyonProfile : AFieldProfile, ILayerProfile
    {
        public override string Caption => "Canyon";

        private readonly IGeneratorBuilder _builder;

        public CanyonProfile(IGeneratorBuilder builder)
        {
            _builder = builder;
        }
        public IGeneratorBuilder GetBuilder()
        {

            return _builder;
        }

    }

}
