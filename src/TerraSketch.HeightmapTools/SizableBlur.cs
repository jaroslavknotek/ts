using System;
using System.Numerics;
using TerraSketch.Layer;

namespace TerraSketch.Heightmap.Tools
{
    public class SizableBlur : IConvolutionPlugin
    {
        private MatrixNxN kernel ;
        private int divisor;
        public string Caption => "Box Blur Sizable";
        private readonly ConvolutionPluginHelper _helper = new ConvolutionPluginHelper();

        public void InitializeKernelMatrix(int size = -1 )
        {
            if(size > 0)
            {
                divisor = 0;
                kernel = new MatrixNxN((uint)size);
                for (uint y = 0; y < kernel.N; y++)
                {
                    for (uint x = 0; x < kernel.N; x++)
                    {
                        kernel[x, y] = 1;
                        divisor++;
                    }
                }
            }
            else
            {
                throw new ArgumentException();
            }
            
        }

        public float Apply(ILayer source, int x, int y)
        {
            return _helper.ApplyMatrix(source,  new Vector2(x,y), kernel,divisor);
        }

        
    }
}
