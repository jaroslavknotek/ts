using System;
using System.Numerics;
using Common.MathUtils;
using Common.MathUtils.Probability;
using TerraSketch.Generators.Abstract;
using TerraSketch.Layer;

namespace TerraSketch.Generators.Noise
{
    public class DsNoiseParameters
    {
        public float HillProbability { get; set; }
    }
    public class DiamondSquare //: INoise
    {
        private readonly IRandom2 _rand;
        public DiamondSquare(IRandom2 rand)
        {
            _rand = rand;
        }
        public ILayerMasked Do(INoiseParameters param, Vector2 size)
        {

            var dt = DateTime.Now.Millisecond;
            System.Diagnostics.Debug.Print(dt + "Rand" + _rand.NextD(1, 2));

            var pow = (int)JryMath.Max(JryMath.Ceil(JryMath.Log(size.X, 2)), JryMath.Ceil(JryMath.Log(size.Y, 2)));
            int sizePow2 = (int)JryMath.Pow(2, pow) + 1;
            Layer2DObject layer = new Layer2DObject(sizePow2, sizePow2);

            setupBaseDS(sizePow2, layer, _rand);
            var par = param.BaseAmplitude * param.Amplitude;

            ds(layer, sizePow2, par, param, _rand);


            var croped = new Layer2DObject(size);
            for (int y = 0; y < size.Y; y++)
            {
                for (int x = 0; x < size.X; x++)
                {
                    croped[x, y] = layer[x, y];
                }
            }
            return croped;
        }


        private void setupBaseDS(int sizePow2, Layer2DObject layer, IRandom2 rand)
        {
            int blockSize = sizePow2;
            int xstart = 0;
            int ystart = 0;
            int xend = xstart + blockSize - 1;
            int yend = ystart + blockSize - 1;

            var tl = rnd(rand, xstart, ystart);
            var tr = rnd(rand, xend, ystart);
            var bl = rnd(rand, xstart, yend);
            var br = rnd(rand, xend, yend);

            layer[xend, ystart] = tr;
            layer[xstart, yend] = bl;
            layer[xstart, ystart] = tl;
            layer[xend, yend] = br;
        }

        private void ds(Layer2DObject layer, int sizePow2, float ba, INoiseParameters np, IRandom2 rand)
        {
            int size = sizePow2 - 1;
            int log = (int)JryMath.Log(size, 2);

            // HACK
            var x = np as DsNoiseParameters;

            for (int l = 0; l < log; l++)
            {
                if (l == 0 && x != null)
                {
                    hackeddiamond(layer, ba, x.HillProbability);
                }
                else

                    diamond(layer, size, ba, l, rand);


                square(layer, size, ba, l, rand);
                ba *= np.Amplitude;
                if (l >= np.ToDepth)
                    ba *= np.Amplitude;
            }
        }

        private void hackeddiamond(Layer2DObject layer, float ap, float he)
        {

            var s = layer.Resolution.X - 1;
            var tl = layer[0, 0].Value;
            var bl = layer[0, s].Value;
            var tr = layer[s, 0].Value;
            var br = layer[s, s].Value;
            var c = (tl + tr + br + bl) / 4 + (he - .5f) * ap;
            //var c = he-.5f * ap ;
            layer[s / 2, s / 2] = c;


        }

        private void diamond(ILayer layer, int size, float ap, int l, IRandom2 rand)
        {
            int squareSize = (int)(size * JryMath.Pow(.5f, l));
            int squareHalfSize = (int)(squareSize / 2);
            for (int iy = 0; iy < JryMath.Pow(2, l); iy++)
            {
                for (int ix = 0; ix < JryMath.Pow(2, l); ix++)
                {

                    var xCorL = ix * squareSize;
                    var xCorR = (ix + 1) * squareSize;
                    var yCorT = iy * squareSize;
                    var yCorB = (iy + 1) * squareSize;

                    var tl = layer[xCorL, yCorT].Value;
                    var bl = layer[xCorL, yCorB].Value;
                    var tr = layer[xCorR, yCorT].Value;
                    var br = layer[xCorR, yCorB].Value;
                    int xph = xCorL + squareHalfSize;
                    int yph = yCorT + squareHalfSize;
                    var c = (tl + tr + br + bl) / 4 + rnd(rand, xph, yph) * ap;
                    layer[xph, yph] = c;
                }
            }
        }
        private void square(Layer2DObject layer, int size, float ap, int depth, IRandom2 rand)
        {
            int squareSize = (int)(size * JryMath.Pow(.5f, depth));
            int squareHalfSize = (int)(squareSize / 2);
            for (int iy = 0; iy < JryMath.Pow(2, depth); iy++)
            {
                for (int ix = 0; ix < JryMath.Pow(2, depth); ix++)
                {

                    var xCorL = ix * squareSize;
                    var xCorR = (ix + 1) * squareSize;
                    var yCorT = iy * squareSize;
                    var yCorB = (iy + 1) * squareSize;

                    var tl = layer[xCorL, yCorT].Value;
                    var bl = layer[xCorL, yCorB].Value;
                    var tr = layer[xCorR, yCorT].Value;
                    var br = layer[xCorR, yCorB].Value;
                    int xph = xCorL + squareHalfSize;
                    int yph = yCorT + squareHalfSize;
                    var c = (tl + tr + br + bl) / 4 + rnd(rand, xph, yph) * ap;
                    var b = (br + bl + c + getClipped(layer, xph, yph + squareSize)) / 4 + rnd(rand, xph, yph + squareSize) * ap;
                    var t = (tr + tl + c + getClipped(layer, xph, yph - squareSize)) / 4 + rnd(rand, xph, yph - squareSize) * ap;
                    var r = (tr + br + c + getClipped(layer, xph + squareSize, yph)) / 4 + rnd(rand, xph + squareSize, yph) * ap;
                    var l = (tl + bl + c + getClipped(layer, xph - squareSize, yph)) / 4 + rnd(rand, xph - squareSize, yph) * ap;

                    layer[xph, yCorT] = t;
                    layer[xph, yCorB] = b;
                    layer[xCorL, yph] = l;
                    layer[xCorR, yph] = r;
                }
            }
        }

        private static float rnd(IRandom2 r, int x, int y)
        {
            var rr = r.NextD(x, y);
            return rr - .5f;
        }

        private float getClipped(Layer2DObject layer, int xph, int v)
        {

            int x = xph;
            int y = v;
            if (xph < layer.Resolution.X) x = JryMath.Abs(xph);
            if (v < layer.Resolution.Y) y = JryMath.Abs(v);

            if (xph >= layer.Resolution.X) x = 2 * layer.Resolution.X - xph - 2;
            if (v >= layer.Resolution.Y) y = 2 * layer.Resolution.Y - v - 2;



            return layer[x, y].Value;
        }


    }
}
