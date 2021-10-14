using Common.DataObjects.Geometry;
using Common.MathUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TerraSketch.Layer.BlendModes;

namespace TerraSketch.Layer
{
    // TODO implement mapping functions
    // http://stackoverflow.com/questions/23268047/procedural-generation-of-a-constrained-landscape
    public class LayerUtility
    {
        private const float thres = .001f;
        private const string error_sizeDoesNotMatch = "Sizes does not match";
        private IBlendModeAlpha ab = new AlphaBlend();
        public void PaintLayerOnLayerMasked(ILayerMasked valueLayer, ILayerMasked maskedLayer, ILayerMasked returnLayer)
        {

            if (valueLayer.Resolution != maskedLayer.Resolution || maskedLayer.Resolution != returnLayer.Resolution)
                throw new ArgumentException(error_sizeDoesNotMatch);
            if (maskedLayer.Mask == null || maskedLayer.Mask.Resolution != maskedLayer.Resolution) throw new ArgumentException();

            for (int y = maskedLayer.ValueArea.Top; y < valueLayer.ValueArea.Bottom; y++)
            {
                for (int x = maskedLayer.ValueArea.Left; x < valueLayer.ValueArea.Right; x++)
                {
                    var source = maskedLayer[x, y];
                    var alpha = maskedLayer.Mask[x, y];

                    if (source.HasValue && alpha.HasValue && alpha.Value > thres)
                    {
                        var target = valueLayer[x, y];
                        if (target.HasValue)
                            returnLayer[x, y] = ab.Blend(target.Value, source.Value, alpha.Value);
                        else
                            returnLayer[x, y] = source.Value;
                    }
                }
            }
        }




        public void FloodFill(ILayer mask)
        {

            // TODO make safe
            var pointInside = findPointInside(mask);
            if (pointInside != null)
            {
                var pmax = pointInside.Value; // definitely inside
                FloodFill(mask, pmax, null, 1);
            }
        }

        public void FloodFill(ILayer mask, Vector2 pmax, float? nullValue, float? fillValue)
        {
            Stack<Vector2> v = new Stack<Vector2>();
            v.Push(pmax);
            while (v.Count > 0)
            {
                var cur = v.Pop();
                if (!InBounds(mask.Resolution, cur.X, cur.Y))
                    continue;
                mask[cur] = fillValue;
                var l = new Vector2(cur.X - 1, cur.Y);
                var r = new Vector2(cur.X + 1, cur.Y);
                var t = new Vector2(cur.X, cur.Y - 1);
                var b = new Vector2(cur.X, cur.Y + 1);

                pushifEmpty(mask, v, l, nullValue);
                pushifEmpty(mask, v, r, nullValue);
                pushifEmpty(mask, v, t, nullValue);
                pushifEmpty(mask, v, b, nullValue);
            }
        }

        public IMask GetMaskForPoints(IList<Vector2> pts)
        {
            var geoUtils = new GeometryUtils();
            var bounds = geoUtils.GetBoundsForPoints(pts);
            var size = new Vector2(bounds.Right - bounds.Left + 1, bounds.Bottom - bounds.Top + 1);
            var mask = new Mask(size);

            var tl = new Vector2(bounds.Left, bounds.Top);
            var firstPt = (pts[0] - tl);
            var tempPt = firstPt;

            LayerDrawer ld = new LayerDrawer(mask);
            for (int i = 1; i < pts.Count; i++)
            {
                var curPt = (pts[i] - tl);

                ld.DrawLine(curPt, tempPt);
                tempPt = curPt;
            }
            ld.DrawLine(firstPt, tempPt);

            FloodFill(mask);
            return mask;
        }





        public IntVector2 GetHighestNeighbour(ILayer layer, int x, int y, bool vonNeumanOnly = false)
        {
            IntVector2 minv = new IntVector2(-1, -1);
            float minh = float.MinValue;

            var xGe0 = x - 1 >= 0;
            var yGe0 = y - 1 >= 0;
            var xLtMax = x + 1 < layer.Resolution.X;
            var yLtMax = y + 1 < layer.Resolution.Y;

            if (xLtMax)
                minh = doHig(layer, new IntVector2(x + 1, y), minh, ref minv);
            if (xGe0)
                minh = doHig(layer, new IntVector2(x - 1, y), minh, ref minv);
            if (yLtMax)
                minh = doHig(layer, new IntVector2(x, y + 1), minh, ref minv);
            if (yGe0)
                minh = doHig(layer, new IntVector2(x, y - 1), minh, ref minv);
            if (vonNeumanOnly) return minv;

            if (xLtMax && yLtMax)
                minh = doHig(layer, new IntVector2(x + 1, y + 1), minh, ref minv);
            if (xGe0 && yGe0)
                minh = doHig(layer, new IntVector2(x - 1, y - 1), minh, ref minv);
            if (xGe0 && yLtMax)
                minh = doHig(layer, new IntVector2(x - 1, y + 1), minh, ref minv);
            if (xLtMax && yGe0)
                /*minh = no need to do last one*/
                doHig(layer, new IntVector2(x + 1, y - 1), minh, ref minv);

            return minv;
        }

        public IntVector2 GetLowestNeighbour(ILayer layer, int x, int y, bool vonNeumanOnly = false)
        {
            IntVector2 minv = new IntVector2(-1, -1);
            float minh = float.MaxValue;

            var xGe0 = x - 1 >= 0;
            var yGe0 = y - 1 >= 0;
            var xLtMax = x + 1 < layer.Resolution.X;
            var yLtMax = y + 1 < layer.Resolution.Y;

            if (xLtMax)
                minh = doLoc(layer, new IntVector2(x + 1, y), minh, ref minv);
            if (xGe0)
                minh = doLoc(layer, new IntVector2(x - 1, y), minh, ref minv);
            if (yLtMax)
                minh = doLoc(layer, new IntVector2(x, y + 1), minh, ref minv);
            if (yGe0)
                minh = doLoc(layer, new IntVector2(x, y - 1), minh, ref minv);
            if (vonNeumanOnly) return minv;

            if (xLtMax && yLtMax)
                minh = doLoc(layer, new IntVector2(x + 1, y + 1), minh, ref minv);
            if (xGe0 && yGe0)
                minh = doLoc(layer, new IntVector2(x - 1, y - 1), minh, ref minv);
            if (xGe0 && yLtMax)
                minh = doLoc(layer, new IntVector2(x - 1, y + 1), minh, ref minv);
            if (xLtMax && yGe0)
                /*minh = no need to do last one*/
                doLoc(layer, new IntVector2(x + 1, y - 1), minh, ref minv);

            return minv;
        }

        private static float doHig(ILayer layer, IntVector2 v, float minh, ref IntVector2 minv)
        {
            // sure value exists
            // ReSharper disable once PossibleInvalidOperationException
            if (!(layer[v].Value > minh)) return minh;

            minh = layer[v].Value;
            minv = v;
            return minh;
        }
        private static float doLoc(ILayer layer, IntVector2 v, float minh, ref IntVector2 minv)
        {
            // sure value exists
            // ReSharper disable once PossibleInvalidOperationException
            if (!(layer[v].Value < minh)) return minh;

            minh = layer[v].Value;
            minv = v;
            return minh;
        }
        public LayerStats GatherStats(ILayer l)
        {
            int xS = l.Resolution.X;
            int yS = l.Resolution.Y;
            float min = float.MaxValue;
            float max = float.MinValue;
            float sumVal = 0;
            int countOfNonNull = 0;
            // float var float EX 
            for (int y = 0; y < yS; y++)
            {
                for (int x = 0; x < xS; x++)
                {
                    var val = l[x, y];
                    if (!val.HasValue) continue;

                    var valval = val.Value;
                    if (valval < min)
                        min = val.Value;
                    if (valval > max)
                        max = valval;

                    sumVal += valval;
                    countOfNonNull++;


                }
            }
            return new LayerStats(min, max, sumVal, countOfNonNull);
        }

        public void CloneFromTo(ILayer layerFrom, ILayer layerTo)
        {
            if (layerTo == null || layerFrom == null) throw new ArgumentNullException();
            if (layerFrom.Resolution != layerTo.Resolution)
                throw new ArgumentException("results have different size");

            IterateValues(layerFrom, (x, y, nValue) =>
            {
                layerTo[x, y] = nValue;
            });

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="action">delegate that gets x,y and value from layer as parameter</param>
        public void IterateValues(ILayer source, Action<int, int, float?> action)
        {
            // TODO implement as enumerator!
            for (int y = 0; y < source.Resolution.Y; y++)
            {
                for (int x = 0; x < source.Resolution.X; x++)
                {

                    action(x, y, source[x, y]);
                }
            }
        }
        public void IterateValues(ILayer source, Action<IntVector2, float?> action)
        {
            for (int y = 0; y < source.Resolution.Y; y++)
            {
                for (int x = 0; x < source.Resolution.X; x++)
                {

                    action(new IntVector2(x, y), source[x, y]);
                }
            }
        }
        public void Normalize(ILayer l)
        {
            var s = GatherStats(l);

            var min = s.Min == s.Max ? s.Max - 1 : s.Min;

            var coef = 1 / (s.Max - min);
            if (float.IsInfinity(coef)) return;

            for (int y = 0; y < l.Resolution.Y; y++)
            {
                for (int x = 0; x < l.Resolution.X; x++)
                {
                    var baseVal = l[x, y];
                    if (baseVal.HasValue)
                    {
                        var val = (baseVal - min) * coef;
                        l[x, y] = val;
                    }
                }
            }
        }
        public void FitInto(ILayer l, float min, float max)
        {
            if (max < min) throw new ArgumentException();
            var s = GatherStats(l);

            var coef = 1 / (s.Max - s.Min);
            if (float.IsInfinity(coef)) return;

            for (int y = 0; y < l.Resolution.Y; y++)
            {
                for (int x = 0; x < l.Resolution.X; x++)
                {
                    var baseVal = l[x, y];
                    if (baseVal.HasValue)
                    {
                        var val = (baseVal - s.Min) * coef;
                        //value is now in<0,1>
                        l[x, y] = val * (max - min) + min;
                    }
                }
            }
        }

        private Vector2? findPointInside(ILayer mask)
        {
            int maskColor = 1;
            int sizeY = mask.Resolution.Y;
            int sizeX = mask.Resolution.X;

            for (int y = 0; y < sizeY; y++)
            {
                int? f = null;
                for (int x = 0; x < sizeX; x++)
                {
                    var c = mask[x, y];
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (c.HasValue && c.Value == maskColor)
                    {
                        if (!f.HasValue)
                            f = x;
                        else if (x - f > 1)
                        {
                            return new Vector2(x - 1, y);
                        }
                        else
                            f = null;
                    }
                }
            }
            return null;
        }

        private void pushifEmpty(ILayer mask, Stack<Vector2> v, Vector2 cur, float? nullValue)
        {
            // if in boundaries
            if (cur.X >= 0 && cur.X < mask.Resolution.X
                && cur.Y >= 0 && cur.Y < mask.Resolution.Y)
            {
                var col = mask[cur];

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (col == nullValue)
                    v.Push(cur);
            }
        }

        public IMask ExtendLayer(IMask source, int size)
        {
            var layer = new Mask(source.Resolution + new IntVector2(2 * size, 2 * size));
            IterateValues(source, (x, y, val) =>
            {
                if (val.HasValue)
                    layer[x + size, y + size] = source[x, y];
            });
            return layer;
        }

        public ILayer ExtendLayer(Layer2DObject source, int size)
        {
            var layer = new Layer2DObject(source.Resolution + new IntVector2(2 * size, 2 * size));

            IterateValues(source, (x, y, val) =>
            {
                if (val.HasValue)
                    layer[x + size, y + size] = source[x, y];
            });

            return layer;
        }

        public IMask GetQuarterFadedCircle(int radius, float fade, IInterpolation interpolation)
        {
            if (radius < 0 || fade > 1 || fade < 0) throw new ArgumentException();
            int fadeBegin = (int)(radius * (1 - fade));
            var brushmask = new Mask(new Vector2(radius, radius));
            for (int y = 0; y < radius; y++)
            {
                var li = (int)JryMath.Sqrt(radius * radius - (radius - y) * (radius - y));

                for (int x = 0; x < li; x++)
                {
                    var yCor = radius - y;
                    float val = 1;
                    var dis = JryMath.Sqrt(x * x + yCor * yCor);
                    if (dis > fadeBegin)
                        val = interpolation.Interpolate(1, 0, (dis - fadeBegin) / (radius - fadeBegin));

                    brushmask[x, yCor - 1] = val;
                }

            }
            return brushmask;
        }

        public void MergeLayers(ILayer[] layers, ILayer destination)
        {
            if (layers == null || layers.Length == 0 || destination == null) throw new ArgumentException("invalid params");

            if (layers.Any(r => r.Resolution != destination.Resolution)) throw new ArgumentException("Layers have different resolutions");


            for (int i = 0; i < layers.Length; i++)
            {
                var currentLayer = layers[i];

                IterateValues(destination, (x, y, fal) =>
                {
                    float values = 0;
                    var temp = currentLayer[x, y];
                    destination[x, y] = temp.HasValue ? temp.Value + values : values;
                }
                               );
            }
        }

        public bool InBounds(IntVector2 resolution, IntVector2 position)
        {
            var lX = resolution.X;
            var lY = resolution.Y;

            return position.X < lX && position.X >= 0 && position.Y < lY && position.Y >= 0;
        }

        public bool InBounds(Vector2 resolution, int x, int y)
        {
            var lX = resolution.X;
            var lY = resolution.Y;

            return x < lX && x >= 0 && y < lY && y >= 0;
        }

        public bool InBounds(Vector2 resolution, float x, float y)
        {
            var lX = resolution.X;
            var lY = resolution.Y;

            return x < lX && x >= 0 && y < lY && y >= 0;
        }


        Blur blur = new Blur();

        public ILayerMasked Blur(ILayerMasked l, int fadeSize)
        {
            var m = new Layer2DObject(l.Resolution);
            blur.Process(l, m, fadeSize);
            return m;
        }

        public ILayer Blur(ILayer l, int fadeSize)
        {
            var m = new Layer2DObject(l.Resolution);
            blur.Process(l, m, fadeSize);
            return m;
        }
        public IMask Blur(IMask l, int fadeSize)
        {
            IMask m = new Mask(l.Resolution);
            blur.Process(l, m, fadeSize);
            return m;
        }
    }

}