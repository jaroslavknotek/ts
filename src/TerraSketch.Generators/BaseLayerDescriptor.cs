using TerraSketch.DataObjects.ParameterObjects;
using TerraSketch.Generators.Abstract;
using TerraSketch.Layer;
using TerraSketch.Layer.BlendModes;

namespace TerraSketch.Generators
{
    public class BaseLayerDescriptor : IBaseLayerDescriptor
    {
        public IErosionDescriptor ErosionDescriptor { get; set; }
        public ILayerProfile Profile { get; set; }
        public IBlendModeAlpha AlphaBlend { get;  set; }
      
        public IErosionType HydraulicErosion {
            get { return ErosionDescriptor.HydraulicErosion; } }



        public  IErosionParameters HydraulicErosionParams {
            get { return ErosionDescriptor.HydraulicErosionParams; }
        }
        public IWorldInformativeParameters WorldParams { get;  set; }

        public ILayerGlobalParameters LayerGlobalParameters
        {
            get;set;
        }

        public BaseLayerDescriptor( ILayerProfile profile)
        {
            // defaults
            AlphaBlend = new AlphaBlend();
            
            ErosionDescriptor = new ErosionDescriptor();
            
            Profile = profile;
        }
    }
}