using Common.DataObjects.Geometry;
using Common.MathUtils;
using System.Numerics;

namespace TerraSketch.Layer
{
    public class Layer2DObject : ILayerMasked
    {
        private int cachedLineIndex = -1;
        private float?[] cachedLine;
        private float?[][] layer;

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
                updateArea(x, y);

                if (cachedLineIndex == y)
                {
                    cachedLine[x] = value;
                    return;
                }

                // check if cachedLine is actualy usefull
                // parallel problems
                cachedLine = layer[y];
                cachedLine[x] = value;
            }
        }

        public float? this[Vector2 v]
        {
            get { return this[(int)v.X, (int)v.Y]; }
            set { this[(int)v.X, (int)v.Y] = value; }
        }

        public Rect ValueArea { get; private set; }
        public IntVector2 Resolution { get;}
        public IntVector2 Offset { get; set; }


        public IMask Mask { get; set; }

        public bool HasMask => Mask != null;


        public Layer2DObject(int width, int height) : this(new Vector2(width, height))
        {
        }

        public Layer2DObject(Vector2 bitmapResolution)
        {
            UpdateValueArea();
            Resolution = bitmapResolution;
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

        public void UpdateValueArea()
        {
            int l = int.MaxValue;
            int r = int.MinValue;
            int t = int.MaxValue;
            int b = int.MinValue;
            for (int y = 0; y < Resolution.Y; y++)
            {
                for (int x = 0; x < Resolution.X; x++)
                {
                    if (this[x, y].HasValue)
                    {
                        if (x > r) r = x;
                        if (x < l) l = x;
                        if (y < t) t = y;
                        if (y > b) b = y;
                    }
                }
            }
            ValueArea = new Rect(l, r, t, b);
        }

        private void updateArea(int x, int y)
        {
            int l = ValueArea.Left;
            int r = ValueArea.Right;
            int t = ValueArea.Top;
            int b = ValueArea.Bottom;

            if (x > r) r = x;
            if (x < l) l = x;
            if (y < t) t = y;
            if (y > b) b = y;

            ValueArea = new Rect(l, r, t, b);
        }

    }
}
