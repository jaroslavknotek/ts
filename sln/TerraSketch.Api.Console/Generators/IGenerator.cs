using System.Threading.Tasks;
using TerraSketch.Layer;

namespace TerraSketch.Generators.Abstract
{
    //public interface IGenerator
    //{

    //    //Task<ILayerMasked[]> GenerateLayers(IList<ILayerDescriptor> descs);
    //    Task<ILayerMasked> GenerateLayer(ILayerGlobalParameters desc);
    //}

    public interface IGenerator
    {
        Task<ILayerMasked> GenerateLayer();
    }

    public interface ISubGenerator : IGenerator
    {
        float Influence { get; set; }
    }

}
