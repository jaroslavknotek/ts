using System.Numerics;
using TerraSketch.Layer;

namespace TerraSketch.Heightmap.Tools
{
    public class ConstantSizeBlur : IConvolutionPlugin
    {
        private MatrixNxN kernel;
        
        public string Caption 
        {
            get
            {
                return "Box Blur Constant";
            }
        }
        private ConvolutionPluginHelper helper = new ConvolutionPluginHelper();


        internal ConstantSizeBlur()
        {

        }
        public void InitializeKernelMatrix(int size = -1)
        {
            kernel = new MatrixNxN(9);
            for (uint y = 0; y < kernel.N; y++)
            {
                for (uint x = 0; x < kernel.N; x++)
                {
                    kernel[x, y] = 1;
                }
            }

        }

        public float Apply(ILayer source, int x, int y)
        {
            return helper.ApplyMatrix(source, new Vector2(x, y), kernel, 9*9);
        }


    }
}
