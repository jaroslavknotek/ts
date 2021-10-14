using TerraSketch.Layer;

namespace TerraSketch.Generators.Abstract
{

    public interface IErosionParameters
    {
        public float SeaLevel { get; set; }
        int Strenght { get; set; }
    }
    public interface IErosionType
    {
        void Erode(ILayer layer, IErosionParameters par);
    }
}
