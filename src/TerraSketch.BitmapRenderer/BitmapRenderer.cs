using System;
using System.Drawing;
using System.Drawing.Imaging;
using Common.MathUtils;
using TerraSketch.Layer;

namespace TerraSketch.BitmapRendering
{
    public class BitmapRenderer : IBitmapRenderer
    {
        public ILayerMasked RenderBitmapToHeightMap(Bitmap bitmap)
        {
            int hei = bitmap.Height;
            int wid = bitmap.Width;
            var layer = new Layer2DObject(wid,hei);

            BitmapData dataOut = bitmap.LockBits(new Rectangle(0, 0, wid, hei), ImageLockMode.WriteOnly, bitmap.PixelFormat);

            unsafe
            {
                int dO = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;  // pixel size in bytes

                for (int yo = 0; yo < hei; yo++)
                {
                    byte* optr = (byte*)dataOut.Scan0 + yo * dataOut.Stride;

                    for (int xo = 0; xo < wid; xo++)
                    {
                        //COLOR here

                        var color = toGrayScale(optr[0], optr[1], optr[2]);

                        layer[xo, yo] = color;

                        optr += dO;
                    }
                }
            }
            bitmap.UnlockBits(dataOut);

            return layer;
        }

        private float toGrayScale(byte r, byte g, byte b)
        {
            return ((float) (r + g + b) / 3 )/ 255;
        }

        public Bitmap RenderHeightMapToBitmap(ILayer layer, float min, float max)
        {
            
            if (layer == null)
                throw new ArgumentNullException(nameof(layer));
                if(min >= max)
                return new  Bitmap(layer.Resolution.X,layer.Resolution.Y, PixelFormat.Format24bppRgb);
            var b = renderLayerToBitmap(layer, min, max);

            return b;
        }
        public Bitmap RenderHeightMapToBitmap(ILayer layer)
        {
            var stats = new LayerUtility().GatherStats(layer);

            var min = stats.Min == stats.Max ? stats.Max - 1 : stats.Min;

            return RenderHeightMapToBitmap(layer, min, stats.Max);
        }

        private Bitmap renderLayerToBitmap(ILayer input, float min, float max)
        {
            // Simplified code from 081fullcolor project that's author is Josef Pelikan
            var wid = input.Resolution.X;
            var hei = input.Resolution.Y;
            var oneOverminmaxCoef = 1 / (max - min);
            Bitmap output = new Bitmap(wid, hei, PixelFormat.Format24bppRgb);

            BitmapData dataOut = output.LockBits(new Rectangle(0, 0, wid, hei), ImageLockMode.WriteOnly, output.PixelFormat);

            unsafe
            {
                int dO = Image.GetPixelFormatSize(output.PixelFormat) / 8;  // pixel size in bytes

                for (int yo = 0; yo < hei; yo++)
                {
                    byte* optr = (byte*)dataOut.Scan0 + yo * dataOut.Stride;

                    for (int xo = 0; xo < wid; xo++)
                    {
                        //COLOR here

                        var color = obtainColor(input[xo, yo], min, oneOverminmaxCoef);
                        //

                        optr[0] = color;
                        optr[1] = color;
                        optr[2] = color;

                        optr += dO;
                    }
                }
            }
            output.UnlockBits(dataOut);
            return output;
        }

        //Color is a struct so everything should be quicker
        private byte obtainColor(float? fVal, float min, float oneOverminmaxCoef)
        {
            if (fVal.HasValue)
            {
                return (byte)((fVal.Value - min) * oneOverminmaxCoef * 255);
            }

            return 0;
        }

        private byte obtainColor(float? fVal)
        {
            if (fVal.HasValue)
            {
                return (byte)(  JryMath.Max(0,JryMath.Min( fVal.Value * 255,255)));
            }

            return 0;
        }

        public Bitmap RenderHeightMapToBitmapClip(ILayer input)
        {
            // Simplified code from 081fullcolor project that's author is Josef Pelikan
            var wid = input.Resolution.X;
            var hei = input.Resolution.Y;
            
            Bitmap output = new Bitmap(wid, hei, PixelFormat.Format24bppRgb);

            BitmapData dataOut = output.LockBits(new Rectangle(0, 0, wid, hei), ImageLockMode.WriteOnly, output.PixelFormat);

            unsafe
            {
                int dO = Image.GetPixelFormatSize(output.PixelFormat) / 8;  // pixel size in bytes

                for (int yo = 0; yo < hei; yo++)
                {
                    byte* optr = (byte*)dataOut.Scan0 + yo * dataOut.Stride;

                    for (int xo = 0; xo < wid; xo++)
                    {
                        //COLOR here

                        var color = obtainColor(input[xo, yo]);
                        //

                        optr[0] = color;
                        optr[1] = color;
                        optr[2] = color;

                        optr += dO;
                    }
                }
            }
            output.UnlockBits(dataOut);
            return output;
        }
    }

    public interface IBitmapRenderer
    {
        ILayerMasked RenderBitmapToHeightMap(Bitmap bitmap);
        Bitmap RenderHeightMapToBitmap(ILayer layer, float min, float max);
        Bitmap RenderHeightMapToBitmap(ILayer layer);
        Bitmap RenderHeightMapToBitmapClip(ILayer layer);
    }
}
