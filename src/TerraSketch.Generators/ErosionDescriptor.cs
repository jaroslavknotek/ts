using TerraSketch.Generators.Abstract;

namespace TerraSketch.Generators
{
    public class ErosionDescriptor : IErosionDescriptor
    {
        public IErosionType HydraulicErosion
        {
            get;
        }

        public IErosionParameters HydraulicErosionParams
        {
            get;
        }

        public ErosionDescriptor()
        {
            HydraulicErosionParams = new HydroErosionParams();
            HydraulicErosion = new BasicHydraulicErosion();
        }

    
    }
}