using Common.DataObjects.Geometry;
using TerraSketch.DataObjects.ParameterObjects;
using TerraSketch.Layer.BlendModes;

namespace TerraSketch.Generators.Abstract
{

    /// <summary>
    /// Parameters important to generator.
    /// Those parameters are in need when generating single layer
    /// </summary>
    public interface ILayerLocalParameters
    {
        bool HasMask { get; }
        IPolygon Polygon { get; }
        
        int BlurSize { get; }
        
        int ExtendSize { get; }
    }

    /// Parameters important to composer.
    /// Those parameters are in need when composing layers toghether
    public interface ILayerGlobalParameters
    {
        IGenerator Generator { get; }
        IBlendMode BlendMode { get; }
        float Offset { get; }
    }
    public interface IBaseLayerDescriptor:IErosionDescriptor
    {
        ILayerGlobalParameters LayerGlobalParameters { get;} 
        IBlendModeAlpha AlphaBlend { get; }
        
        //IErosion AirErosion { get; set; }
        IWorldInformativeParameters WorldParams { get; }

    }
    public interface IErosionDescriptor
    {
        IErosionType HydraulicErosion { get; }
        IErosionParameters HydraulicErosionParams { get; }
    }
}