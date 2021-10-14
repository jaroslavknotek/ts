using TerraSketch.DataObjects.Abstract;
using TerraSketch.DataObjects.FieldObjects.FieldParams;

namespace TerraSketch.DataObjects.ParameterObjects
{

    public interface IFieldParameters
    {
        IFieldDetail Detail
        {
            get;
            set;
        }


        IFieldProfile FieldProfile
        {
            get;
            set;
        }

        IFieldBlendMode BlendModeWrap { get; set; }

        float Offset { get; set; }
        int? Seed { get; set; }
    }
}