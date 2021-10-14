using TerraSketch.DataObjects.FieldObjects.FieldParams;
using TerraSketch.Generators.Abstract;

namespace TerraSketch.Generators.LayerProfile
{
    public class RockyMountainProfile : AFieldProfile, ILayerProfile
    {
        public override string Caption => "Rocky Mountain";


        private readonly IGeneratorBuilder _builder;

        public RockyMountainProfile(IGeneratorBuilder builder)
        {
            _builder = builder;
        }
        public IGeneratorBuilder GetBuilder()
        {

            return _builder;
        }
    }
}
