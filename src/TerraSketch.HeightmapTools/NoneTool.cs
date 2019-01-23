using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TerraSketch.DataObjects;
using TerraSketch.Layer;

namespace TerraSketch.Heightmap.Tools
{
   public class NoneTool : IConvolutionPlugin
    {
        public string Caption
        {
            get
            {
                return "None"; 
            }
        }

        public float Apply(ILayer target, int x, int y)
        {
            
            return target[x,y].HasValue ? target[x,y].Value: 0;
        }

        public void InitializeKernelMatrix(int size = -1)
        {
            return;
        }
    }
}
