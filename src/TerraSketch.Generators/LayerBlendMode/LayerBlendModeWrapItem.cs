using TerraSketch.DataObjects.FieldObjects.FieldParams;
using TerraSketch.Layer.BlendModes;

namespace TerraSketch.Generators.LayerBlendMode
{
   public class LayerBlendModeWrapItem :AFieldBlendMode
    {
        
        public LayerBlendModeWrapItem(string name, IBlendMode bm)
        {
            Caption = name;
            BlendMode = bm;
            
        }

        public override string Caption { get; }
    }
}
