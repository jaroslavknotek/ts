namespace TerraSketch.Heightmap.Tools
{
    public static class Plugins
    {
        private static NoneTool _none;

        public static NoneTool None => _none ?? (_none = new NoneTool());

        private static ConstantSizeBlur _blur;

        public static ConstantSizeBlur Blur => _blur ?? (_blur = new ConstantSizeBlur());

        private static Sharpen _sharpen;

        public static Sharpen Sharpen => _sharpen ?? (_sharpen = new Sharpen());

        private static TESTBRUSH _testBrush;

        public static TESTBRUSH TestBrush => _testBrush ?? (_testBrush = new TESTBRUSH());

        private static Elevate _elevate;

        public static Elevate Elevate => _elevate ?? (_elevate = new Elevate());

        private static Lower _lower;

        public static Lower Lower => _lower ?? (_lower = new Lower());
    }

    public class TESTBRUSH : IConvolutionPlugin
    {
        private MatrixNxN kernel;
        public string Caption => "TESTBRUSH";
        
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
            kernel[1, 2] = -1;
            kernel[0, 1] = -1;
            kernel[2, 1] = -1;
            kernel[1, 1] = 5;
            divisor = 1;
        }

        public float Apply(Layer.ILayer source, int x, int y)
        {
            return .5f;
            //return helper.ApplyMatrix(source, new System.Numerics.Vector2(x, y), kernel, divisor);
        }


    }
}
