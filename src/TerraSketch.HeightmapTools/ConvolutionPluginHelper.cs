using Common.MathUtils;
using System.Numerics;
using TerraSketch.Layer;

namespace TerraSketch.Heightmap.Tools 
{
   public  class ConvolutionPluginHelper
    {
        public float ApplyMatrix(ILayer source, Vector2 loc,  MatrixNxN kernel,int div)
        {
            
            var halfSize = (int)(kernel.N / 2);
            float acc = 0;
            
            for (int j = 0; j < kernel.N; j++)
            {
                for (int i = 0; i < kernel.N; i++)
                {
                    int x = -halfSize + (int)loc.X +(int)i;
                    int y = -halfSize + (int)loc.Y +(int)j;
                    if (isOk(x, source.Resolution.X) && isOk(y, source.Resolution.Y))
                    {
                        var sourceVal = source[x, y];
                        if (sourceVal.HasValue)
                            acc += sourceVal.Value * kernel[(uint)i, (uint)j];
                    }
                }

            }

            var previous = source[(int)loc.X, (int)loc.Y];
            var future = acc / div;
            if(previous.HasValue)
                return  previous.Value + JryMath.Min(future-previous.Value,.1f);
            return future;
        }
        private bool isOk(int x, int size)
        {
            return x > 0 && x < size;
        }
    }
}
