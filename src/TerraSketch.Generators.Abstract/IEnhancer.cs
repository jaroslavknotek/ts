using TerraSketch.Layer;

namespace TerraSketch.Generators.Abstract
{

    public interface IErosionParameters
    {
        int Strenght { get; set; }
    }
    public interface IErosionType
    {
        void Erode(ILayer layer, IErosionParameters par);
    }
}
