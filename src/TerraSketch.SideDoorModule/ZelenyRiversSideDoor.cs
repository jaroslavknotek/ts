using System.Drawing;
using System.Threading.Tasks;
using TerraSketch.Heightmap.Composer;
using TerraSketch.Layer;
using TerraSketch.Logging;

namespace TerraSketch.SideDoorModule
{
    //class ZelenyRiversSideDoor : ISideDoor
    //{
        
    //    public async Task SaveLayers()
    //    {
    //        Blur b = new Blur();
    //        var dr = new DrainageSimulator();
    //        var h = new Sobel();
    //        var vl = new VisualLogger();

    //        var file = "SavedFiles/sopel.bmp";
    //        var i = Image.FromFile(file);
    //        var bitmap = new Bitmap(i);
    //        var conerted = new LayerConverter().LoadLayer(bitmap);



    //        var blured = new Layer2DObject(conerted.Resolution);
    //        b.Process(conerted, blured, 25);
    //        vl.Log(blured, "base");
    //        var drainagemap = dr.GetDrainageMap(blured);
    //        vl.Log(drainagemap, "drainage");
            
    //        var edges = h.GetEdges(drainagemap);
    //        vl.Log(edges, "edges", LogImageProcessingOptions.Clip);

    //        //var bluredI = new Layer2DObject(conerted.Resolution);
    //        //b.Process(edges, bluredI, 3);
    //        //vl.Log(bluredI, "blurred");

    //        //var edgesI = h.GetEdges(bluredI);
    //        //vl.Log(edgesI, "edgesI", LogImageProcessingOptions.Clip);
    //    }
    //}
}