using System.Drawing;
using System.Threading.Tasks;
using TerraSketch.Heightmap.Composer;
using TerraSketch.Layer;
using TerraSketch.Logging;

namespace TerraSketch.SideDoorModule
{
    class BallRiversSideDoor : ISideDoor
    {
        
        public async Task SaveLayers()
        {
            Blur b = new Blur();
            var dr = new BallDrainageSimulator();
            
            var vl = new VisualLogger();

            var file = "SavedFiles/rvrnw.bmp";
            var i = Image.FromFile(file);
            var bitmap = new Bitmap(i);

            var layer = new LayerConverter().LoadLayer(bitmap);



            ILayer blured = new Layer2DObject(layer.Resolution);
            //b.Process(layer, blured, 3);
            blured = layer;
            vl.Log(blured, "base");
            var drainagemap = dr.GetDrainageMap(blured,10);

            //logger shit
            drainagemap[0, 0] = 0;
            vl.Log(drainagemap, "drainage");
            
            
        }
    }
}