using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;
using Common.DataObjects.Geometry;
using Common.MathUtils;
using Common.MathUtils.Probability;
using TerraSketch.Generators;
using TerraSketch.Layer;
using TerraSketch.Logging;
using Voronoi.Generator;

namespace TerraSketch.SideDoorModule
{
    class VoronoiSideDoor : ISideDoor
    {
        const int seed = 12345;
        static IntVector2 res = new IntVector2(512, 512);
        public async Task SaveLayers()
        {
            var countOfCells = 4;

            
            var amplitude = 50;
            var rnd2 = new RandomSeeded(seed);
            SegmentDivider divider = new SegmentDivider(rnd2,  amplitude, 1);
            doVor(  countOfCells, divider);
            //var profile = printMountainProfile(countOfCells, divider);
            //vl.Log(profile, "profile");

                
            



        }

        private static void doVor( int countOfCells, SegmentDivider divider)
        {
            
            printVoronoi(countOfCells, divider);
            
            
        }

        private static void printVoronoi(int countOfCells, ISegmendDivider sd)
        {
            
            var l = new Layer2DObject(res);

            // TODO change rand approach
            
            var rnd = new Rand(seed);
            
            
            var c = new VoronoiConverter(sd);
            var g = new VoronoiGenerator(c);
            var gg = new VoronoiAreaGenerator(g, rnd);

            var vd = new VoronoiAreaDrawer();

            var areas= gg.GenerateAreas(res, res*.5f, countOfCells);
            

            
            vd.PrintToLayer(l, areas, new Vector2());
            var vl = new VisualLogger();
            vl.Log(l, "profile");
        }

        private static ILayer printMountainProfile(int countOfCells, ISegmendDivider sd)
        {

            var l = new Layer2DObject(res);

            // TODO change rand approach

            var rnd = new Rand(seed);


            var c = new VoronoiConverter(sd);
            var g = new VoronoiGenerator(c);
            var gg = new VoronoiAreaGenerator(g, rnd);

            var vd = new VoronoiAreaDrawer();


            var areas= gg.GenerateAreas(res, res,countOfCells);
            vd.PrintToLayer(l, areas,  Vector2.Zero);
            return l;
        }
    }
}