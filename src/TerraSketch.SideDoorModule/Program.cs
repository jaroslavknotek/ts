using System.Drawing;
using System.Threading.Tasks;
using TerraSketch.DataObjects.SaveLoad;
using TerraSketch.Heightmap.Composer;
using TerraSketch.Logging;

namespace TerraSketch.SideDoorModule
{
    class Program
    {
        static void Main(string[] args)
        {
            //doErosion();
            //doComposing();

            doVoronoi();
            //doRivers();
            //doBallRivers();
        }


        private static void doBallRivers()
        {
            var sd = new BallRiversSideDoor();

            new DSTest().Test(sd);
        }
        

        private static void doVoronoi()
        {
            var sd = new VoronoiSideDoor();

            new DSTest().Test(sd);
        }

        private static void doErosion()
        {
            
            var sd = new HydraulicSideDoor();
           
            new DSTest().Test(sd);
        }

        private static void doComposing()
        {
            var slManager = new SaveLoadManager(new TestResource().GetDataSources());
            var composer = new HeightmapComposer(new VisualLogger());
            var sd = new CommonSideDoor(slManager, composer);

             new DSTest().Test(sd);
        }
    }
}
