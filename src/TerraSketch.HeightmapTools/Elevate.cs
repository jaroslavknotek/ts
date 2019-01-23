using TerraSketch.Layer;

namespace TerraSketch.Heightmap.Tools
{
    public class Elevate : IConvolutionPlugin
    {
        private const float value = .01f;
        public string Caption => "Elevate";


        public void InitializeKernelMatrix(int size = -1)
        {
        }

        public float Apply(ILayer source, int x, int y)
        {
            return source[x, y].HasValue ? source[x, y].Value + value :value;
        }


    }
}
