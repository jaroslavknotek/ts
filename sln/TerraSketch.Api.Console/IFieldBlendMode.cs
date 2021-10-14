using TerraSketch.Layer.BlendModes;

namespace TerraSketch.DataObjects.Abstract
{
    public interface IFieldBlendMode
    {
        IBlendMode BlendMode { get; }
        string Caption { get; }
    }
}
