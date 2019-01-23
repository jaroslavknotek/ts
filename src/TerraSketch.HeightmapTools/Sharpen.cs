using System.Numerics;
using TerraSketch.Layer;

namespace TerraSketch.Heightmap.Tools
{
    public class Sharpen : IConvolutionPlugin
    {
        private MatrixNxN kernel;
        public string Caption
        {
            get
            {
                return "Sharpen - 2";
            } 
        }

        internal Sharpen()
        {

        }

        private ConvolutionPluginHelper helper = new ConvolutionPluginHelper();
        private int divisor;

        public void InitializeKernelMatrix(int size = -1)
        {

            kernel = new MatrixNxN((uint)3);
            //for (uint y = 0; y < kernel.N; y++)
            //{
            //    for (uint x = 0; x < kernel.N; x++)
            //    {
            //        kernel[x, y] = -1;
            //    }
            //}
            kernel[1, 0] = -1;
            kernel[1,2] = -1;
            kernel[0, 1] = -1;
            kernel[2, 1] = -1;
            kernel[1, 1] = 5;
            divisor = 1;
        }

        public float Apply(ILayer source,int x,int y)
        {
            return helper.ApplyMatrix(source, new Vector2(x,y), kernel,divisor);
        }


    }
}
