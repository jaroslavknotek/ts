using System.Drawing;
using System.Threading.Tasks;
using TerraSketch.Generators;
using TerraSketch.Logging;

namespace TerraSketch.SideDoorModule
{
    class HydraulicSideDoor : ISideDoor
    {
        
        public async Task SaveLayers()
        {
            var file = "SavedFiles/test.bmp";
            var i = Image.FromFile(file);
            var bitmap = new Bitmap(i);
            var conerted = new LayerConverter().LoadLayer(bitmap);

            var h =new BasicHydraulicErosion();
            var vl = new VisualLogger();

            
            vl.Log(conerted, "baseNoEro");
            h.Erode(conerted, new HydroErosionParams() {Strenght =  50});
            vl.Log(conerted, "baseWithEro");

        }
    }
}