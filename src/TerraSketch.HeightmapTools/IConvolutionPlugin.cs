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
    public interface IConvolutionPlugin
    {
        string Caption { get;        }


        void InitializeKernelMatrix(int size=-1);
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer">Source of data</param>
        /// <param name="target">Layer that the result will be written to</param>
        /// <param name="loc"> Desired location for matrix operation to be applied</param>
        /// <param name="size"> Optional parameter: Size</param>
        float Apply(ILayer source, int x, int y );
    }
}
