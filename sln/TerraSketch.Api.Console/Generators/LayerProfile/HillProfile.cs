using TerraSketch.DataObjects.FieldObjects.FieldParams;
using TerraSketch.Generators.Abstract;

namespace TerraSketch.Generators.LayerProfile
{
    public class HillProfile : AFieldProfile, ILayerProfile
    {
        public override string Caption => "Hill";



        private readonly IGeneratorBuilder _builder;

        public HillProfile(IGeneratorBuilder builder)
        {
            _builder = builder;
        }
        public IGeneratorBuilder GetBuilder()
        {

            return _builder;
        }



    }
}
