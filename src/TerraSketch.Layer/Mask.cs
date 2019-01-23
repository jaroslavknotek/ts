using Common.MathUtils;
using System.Numerics;

namespace TerraSketch.Layer
{
    public class Mask : IMask
    {
        public IntVector2 Resolution { get; private set; }
        private float?[] cachedLine;
        private int cachedLineIndex = -1;
        private float?[][] layer = null;

        public float? this[int x, int y]
        {
            get
            {
                if (cachedLineIndex == y)
                    return cachedLine[x];

                cachedLine = layer[y];
                return cachedLine[x];
            }
            set
            {

                if (cachedLineIndex == y)
                {
                    cachedLine[x] = value;
                    return;
                }

                cachedLine = layer[y];
                cachedLine[x] = value;
            }
        }

        public float? this[Vector2 v]
        {
            get { return this[(int)v.X, (int)v.Y]; }
            set { this[(int)v.X, (int)v.Y] = value; }
        }
        public Mask(Vector2 size)
        {
            this.Resolution = (IntVector2)size;
            var x = Resolution.X;
            var y = Resolution.Y;
            init(x, y);

        }
        private void init(int width, int height)
        {
            layer = new float?[height][];
            for (int i = 0; i < height; i++)
            {
                layer[i] = new float?[width];
            }
        }
    }
}
