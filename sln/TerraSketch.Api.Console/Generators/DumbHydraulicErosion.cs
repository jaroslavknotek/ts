using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using TerraSketch.Generators.Abstract;
using TerraSketch.Layer;

namespace TerraSketch.Generators
{

    public class HydroErosionParams : IErosionParameters
    {
        public float SeaLevel { get; set; }
        public int Strenght { get; set; }
    }

    public class DumbHydraulicErosion : IErosionType
    {
        SeaBlur sb = new SeaBlur();
        public void Erode(ILayer layer, IErosionParameters par)
        {
            sb.Process(layer, par.Strenght, par.SeaLevel);
        }

        private class SeaBlur
        {


            private readonly ParallelOptions _pOptions = new ParallelOptions { MaxDegreeOfParallelism = 16 };

            public ILayerMasked Process(ILayer inImage, int radial, float ignoreLevel)
            {
                //everything that is above ignorelevel is ignored
                var rct = inImage.Resolution;
                var source = new float?[rct.X * rct.Y];

                var width = inImage.Resolution.X;
                var height = inImage.Resolution.Y;

                var _greyScale = new float?[width * height];

                int xSize = inImage.Resolution.X;
                int ySize = inImage.Resolution.Y;
                for (int y = 0; y < ySize; y++)
                {
                    for (int x = 0; x < xSize; x++)
                    {
                        var i = y * xSize + x;
                        var val = inImage[x, y];
                        source[i] = nullToZero(val);
                    }
                }

                Parallel.For(0, source.Length, _pOptions, i =>
                {
                    _greyScale[i] = nullToZero(source[i]);
                });

                var newGrayScale = new float?[width * height];
                var dest = new float?[width * height];

                Parallel.Invoke(
                    () => gaussBlur_4(_greyScale, newGrayScale, radial, width, height));

                Parallel.For(0, dest.Length, _pOptions, i =>
                {
                    var val = newGrayScale[i];
                    if (val > 1)
                        val = 1;
                    if (val < 0)
                        val = 0;


                    dest[i] = val;
                });

                var image = new Layer2DObject(width, height);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var i = y * width + x;
                        var val = dest[i];
                        if (val.HasValue && val.Value < ignoreLevel)
                            image[x, y] = val;
                        else
                            image[x, y] = inImage[x, y];
                    }
                }

                return image;
            }

            private void gaussBlur_4(float?[] source, float?[] dest, int r, int w, int h)
            {
                var bxs = boxesForGauss(r, 3);
                boxBlur_4(source, dest, w, h, (bxs[0] - 1) / 2);
                boxBlur_4(dest, source, w, h, (bxs[1] - 1) / 2);
                boxBlur_4(source, dest, w, h, (bxs[2] - 1) / 2);
            }

            private int[] boxesForGauss(int sigma, int n)
            {
                var wIdeal = Math.Sqrt((12 * sigma * sigma / n) + 1);
                var wl = (int)Math.Floor(wIdeal);
                if (wl % 2 == 0) wl--;
                var wu = wl + 2;

                var mIdeal = (double)(12 * sigma * sigma - n * wl * wl - 4 * n * wl - 3 * n) / (-4 * wl - 4);
                var m = Math.Round(mIdeal);

                var sizes = new List<int>();
                for (var i = 0; i < n; i++) sizes.Add(i < m ? wl : wu);
                return sizes.ToArray();
            }

            private void boxBlur_4(float?[] source, float?[] dest, int w, int h, int r)
            {
                for (var i = 0; i < source.Length; i++)
                    dest[i] = nullToZero(source[i]);


                boxBlurH_4(dest, source, w, h, r);
                boxBlurT_4(source, dest, w, h, r);
            }

            private void boxBlurH_4(float?[] source, float?[] dest, int w, int h, int r)
            {
                var iar = (float)1 / (r + r + 1);
                Parallel.For(0, h, _pOptions, i =>
                {
                    int ti = i * w;
                    int li = ti;
                    int ri = ti + r;
                    var fv = nullToZero(source[ti]);
                    var lv = nullToZero(source[ti + w - 1]);
                    var val = (float?)(r + 1) * fv;
                    for (var j = 0; j < r; j++) val += nullToZero(source[ti + j]);
                    for (var j = 0; j <= r; j++)
                    {
                        val += nullToZero(source[ri++]) - fv;
                        //dest[ti++] = (float)Math.Round(val * iar);
                        dest[ti++] = val * iar;
                    }
                    for (var j = r + 1; j < w - r; j++)
                    {
                        val += nullToZero(source[ri++]) - dest[li++];
                        //dest[ti++] = (float)Math.Round(val * iar);
                        dest[ti++] = val * iar;
                    }
                    for (var j = w - r; j < w; j++)
                    {
                        val += lv - nullToZero(source[li++]);
                        //dest[ti++] = (float)Math.Round(val * iar);
                        dest[ti++] = val * iar;
                    }
                });
            }

            private void boxBlurT_4(float?[] source, float?[] dest, int w, int h, int r)
            {
                var iar = (float)1 / (r + r + 1);
                Parallel.For(0, w, _pOptions, i =>
                {
                    var ti = i;
                    var li = ti;
                    var ri = ti + r * w;
                    var fv = nullToZero(source[ti]);
                    var lv = nullToZero(source[ti + w * (h - 1)]);
                    var val = (r + 1) * fv;
                    for (var j = 0; j < r; j++) val += nullToZero(source[ti + j * w]);
                    for (var j = 0; j <= r; j++)
                    {
                        val += nullToZero(source[ri] - fv);
                        dest[ti] = val * iar;
                        ri += w;
                        ti += w;
                    }
                    for (var j = r + 1; j < h - r; j++)
                    {
                        val += nullToZero(source[ri]) - nullToZero(source[li]);
                        dest[ti] = val * iar;
                        li += w;
                        ri += w;
                        ti += w;
                    }
                    for (var j = h - r; j < h; j++)
                    {
                        val += lv - nullToZero(source[li]);
                        dest[ti] = val * iar;
                        li += w;
                        ti += w;
                    }
                });
            }

            private float nullToZero(float? f)
            {
                return f.HasValue ? f.Value : 0;
            }
        }
    }
}
