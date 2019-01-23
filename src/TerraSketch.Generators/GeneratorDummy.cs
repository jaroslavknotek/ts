using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraSketch.DataObjects;
using TerraSketch.Generators;
using TerraSketch.Layer;

namespace TerraSketch.Generators
{
    public class GeneratorDummy: IGenerator
    {
        public Layer2DObject[] Generate(IWorld world)
        {
            int x = (int)world.Parameters.BitmapResolution.X;
            int y = (int)world.Parameters.BitmapResolution.Y;

            Layer2DObject layer = new Layer2DObject(world.Parameters.BitmapResolution);
            return new Layer2DObject[] { layer };
        }
    }
}
