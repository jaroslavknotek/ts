using Common.MathUtils;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using TerraSketch.DataObjects;
using TerraSketch.DataObjects.FieldObjects;
using TerraSketch.DataObjects.ParameterObjects;
using TerraSketch.Generators;
using TerraSketch.Generators.Abstract;
using TerraSketch.Layer;
using TerraSketch.Layer.BlendModes;
using TerraSketch.Logging;

namespace TerraSketch.Heightmap.Composer
{
    public class HeightmapComposer : IHeightmapComposer
    {
        private const int strengthMultpilier = 30;
        private readonly LayerUtility _layerUtils;
        private readonly IWorldDescriber _describer;
        private readonly IVisualLogger _visualLogger;


        public ILayerMasked ComposedLayer { get; private set; }


        public HeightmapComposer(IVisualLogger logger)
        {
            _visualLogger = logger;

            _layerUtils = new LayerUtility();
            _describer = new WorldDescriber();
        }

        public async Task Compose(IWorld w)
        {
            var param = w.Parameters;
            var fields = w.Fields;

            var pars = await generateFields(param, fields);

            IErosionDescriptor bs;

            ILayerMasked baseLayer;
            IBlendModeAlpha ba;
            if (w.UseBase)
            {
                var bsd = _describer.DescribeBaseLayer(param, w.BaseField);
                //baseLayer = await bsd.LayerGlobalParameters.Profile
                baseLayer = await bsd.LayerGlobalParameters.Generator.GenerateLayer();
                //_layerUtils.Normalize(baseLayer);
                //HACK baselayers mask extend it size
                var x = new Layer2DObject(w.Parameters.BitmapResolution);
                _layerUtils.IterateValues(x, (v, val) => x[v] = baseLayer[v]);
                baseLayer = x;

                ba = bsd.AlphaBlend;
                bs = bsd;
            }
            else
            {
                baseLayer = new Layer2DObject(w.Parameters.BitmapResolution);
                baseLayer.Mask = new Mask(baseLayer.Resolution);
                _layerUtils.IterateValues(baseLayer.Mask, (v, val) => baseLayer[v] = 1);

                setUpDefaulValue(baseLayer);
                bs = new ErosionDescriptor();
                ba = new AlphaBlend();
            }

            bs.HydraulicErosionParams.Strenght = param.ErosionStrength * strengthMultpilier;

            ILayerMasked merged = merge(ba, baseLayer, pars);
            //_layerUtils.FitInto(merged, 0, 1);
            postProcessWholeLayer(merged, bs, w.Parameters.RiverAmount);
            ComposedLayer = merged;

        }

        private void setUpDefaulValue(ILayer baseLayer)
        {
            float defaultValue = 0;
            _layerUtils.IterateValues(baseLayer, (x, y, v) => baseLayer[x, y] = defaultValue);
        }

        private void postProcessWholeLayer(ILayerMasked final, IErosionDescriptor bs, int riverAmount)
        {
            _visualLogger.Log(final, "baseNoEro");
            doErosion(final, bs);
            generateRIvers(final, riverAmount);
        }

        private void doErosion(ILayerMasked final, IErosionDescriptor bs)
        {
            if (bs.HydraulicErosionParams.Strenght <= 0) return;
            onErosionStarted();
            bs.HydraulicErosion.Erode(final, bs.HydraulicErosionParams);
            _visualLogger.Log(final, "baseWithEro");
        }

        private void generateRIvers(ILayerMasked final, int riverAmount)
        {
            if (riverAmount <= 0) return;
            onRiver();
            doRivers(final, riverAmount);
            _visualLogger.Log(final, "baseWithEroAndRivers");
        }

        private async Task<IList<DescribedLayer>> generateFields(IWorldInformativeParameters param, IEnumerable<IField> fields)
        {
            onLayerDescribed();
            IList<ILayerGlobalParameters> desc = _describer.DescribeFields(fields);

            var generatedLayer = new List<ILayerMasked>();
            // TODO paralell
            foreach (var layerDescriptor in desc)
            {
                // TODO refactor
                var l = await layerDescriptor.Generator.GenerateLayer();
                generatedLayer.Add(l);
                onLayerGenerated();
            }

            IList<DescribedLayer> pars = aggregateParams(desc, generatedLayer.ToArray());
            return pars;
        }

        private ILayerMasked merge(IBlendModeAlpha ablend, ILayerMasked baseLayer, IList<DescribedLayer> pars)
        {
            onMergeStarted();
            if (pars == null || pars.Count == 0) throw new ArgumentException();

            var final = baseLayer;

            foreach (var describ in pars)
            {
                var current = describ.Layer;
                var blendMode = describ.GlobalParameters.BlendMode;
                var offset = describ.GlobalParameters.Offset - .5f;

                var minY = JryMath.Max(0, -current.Offset.Y);
                var minX = JryMath.Max(0, -current.Offset.X);
                var maxY = JryMath.Min(current.Resolution.Y, final.Resolution.Y - current.Offset.Y );
                var maxX = JryMath.Min(current.Resolution.X, final.Resolution.X - current.Offset.X );
                for (int y = minY; y < maxY; y++)
                {

                    for (int x = minX; x < maxX; x++)
                    {
                        int ix = x + current.Offset.X;
                        int iy = y + current.Offset.Y;
                      

                        var curVal = current[x, y];
                        if (!curVal.HasValue || !final[ix, iy].HasValue || !current.HasMask) continue;
                        var maskValue = current.Mask[x, y];
                        if (!maskValue.HasValue) continue;


                        var currentLayerValue = curVal.Value + offset;
                        var currentLayerMaskValue = maskValue.Value;
                        //currentLayerMaskValue = 1;
                        var upToThisPointGeneratedLayerValue = final[ix, iy].Value;


                        var blended = blendMode.Blend(upToThisPointGeneratedLayerValue, currentLayerValue * currentLayerMaskValue);
                        final[ix, iy] = ablend.Blend(upToThisPointGeneratedLayerValue, blended, currentLayerMaskValue);
                        if (final[ix, iy] > 0.05f)
                        {
                            
                        }
                    }
                }
            }

            return baseLayer;

        }





        private IList<DescribedLayer> aggregateParams(IList<ILayerGlobalParameters> desc, ILayerMasked[] generatedLayers)
        {
            if (desc.Count != generatedLayers.Length)
                throw new ArgumentException();

            var pars = new List<DescribedLayer>();
            for (int i = 0; i < generatedLayers.Length; i++)
            {
                pars.Add(new DescribedLayer(desc[i], generatedLayers[i]));
                
            }

            return pars;
        }


        private void doRivers(ILayerMasked layer, int riverAmount)
        {
            onRiver();
            IDrainageSimulator drainageSimulator = new BallDrainageSimulator();
            var drainageMap = drainageSimulator.GetDrainageMap(layer, riverAmount);


            _visualLogger.Log(drainageMap, "drainage");

            _layerUtils.IterateValues(drainageMap,
                (cor, val) =>
                {
                    if (drainageMap[cor].HasValue)

                        layer[cor] -= drainageMap[cor].Value * .01f;
                }
            );
        }

        private void onLayerDescribed()
        {
            if (LayerDescribed == null) return;
            LayerDescribed.Invoke(this, new StatusChangedArgument());
        }
        private void onLayerGenerated()
        {
            if (LayerGenerated == null) return;
            LayerGenerated.Invoke(this, new StatusChangedArgument());
        }

        private void onMergeStarted()
        {
            if (MergeStarted == null) return;
            MergeStarted.Invoke(this, new StatusChangedArgument());
        }
        private void onErosionStarted()
        {
            if (ErosionStarted == null) return;
            ErosionStarted.Invoke(this, new StatusChangedArgument());
        }
        private void onRiver()
        {
            if (RiverGenerationStarted == null) return;
            RiverGenerationStarted.Invoke(this, new StatusChangedArgument());
        }

        public event StatusChanged MergeStarted;
        public event StatusChanged LayerDescribed;
        public event StatusChanged LayerGenerated;
        public event StatusChanged RiverGenerationStarted;
        public event StatusChanged ErosionStarted;
    }

    internal struct DescribedLayer
    {
        public ILayerMasked Layer { get; private set; }
        public ILayerGlobalParameters GlobalParameters { get; private set; }

        public DescribedLayer(ILayerGlobalParameters layerGlobalParameters, ILayerMasked layer2DObject) : this()
        {
            this.GlobalParameters = layerGlobalParameters;
            this.Layer = layer2DObject;
        }
    }

    public class Sobel : IEdgeDetector
    {
        private float[,] xSobel = {
            { 1, 0, -1},
            { 2, 0, -2},
            { 1, 0, -1}
        };

        private float[,] ySobel = {
            { 1, 2, 1},
            { 0, 0, 0},
            { -1, -2,- 1}
        };


        public ILayer GetEdges(ILayer layer)
        {
            //readonly copy of layer
            var copy = getReadOnlyCopy(layer);

            for (int y = 1; y < layer.Resolution.Y - 1; y++)
            {
                for (int x = 1; x < layer.Resolution.X - 1; x++)
                {
                    var xs = getValue(copy, xSobel, x, y);
                    var ys = getValue(copy, ySobel, x, y);

                    layer[x, y] = xs + ys;
                }
            }
            return layer;
        }


        private float getValue(ILayer source, float[,] sobel, int xCor, int yCor)
        {
            float value = 0;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    var xPr = xCor + x - 1;
                    var yPr = yCor + y - 1;
                    //if (xPr < source.Resolution.X
                    //    && xPr >= 0
                    //    && yPr < source.Resolution.Y
                    //    && yPr >= 0)
                    //{

                    float v = source[xPr, yPr].HasValue ? source[xPr, yPr].Value : 0;
                    value += sobel[x, y] * v;
                    //}
                }
            }
            return value;
        }
        private static LL getReadOnlyCopy(ILayer layer)
        {
            var l = new LayerUtility();
            var copy = new LL(layer.Resolution.X, layer.Resolution.Y);
            l.CloneFromTo(layer, copy);
            return copy;
        }




    }

    public class LL : ILayer
    {
        private readonly float?[,] _f;

        public LL(int sizeX, int sizeY)
        {
            _f = new float?[sizeX, sizeY];
            this.Resolution = new IntVector2(sizeX, sizeY);
        }
        public IntVector2 Resolution { get; }

        float? ILayer.this[int x, int y]
        {
            get { return _f[x, y]; }
            set { _f[x, y] = value; }
        }

        float? ILayer.this[Vector2 v]
        {
            get { return _f[(int)v.X, (int)v.Y]; }
            set { _f[(int)v.X, (int)v.Y] = value; }
        }
    }
}
