using Common.MathUtils;
using System;
using System.Collections.Generic;
using System.Numerics;
using TerraSketch.Heightmap.Tools;
using TerraSketch.Layer;

namespace TerraSketch.Presenters
{
    public interface ILayerPainter
    {
        bool ReadyToDraw { get; }
        void InitializeSource(ILayer l);
        
        ILayerMasked GetPaintedLayer();

        void BeginBrushpath(Vector2 pos, IConvolutionPlugin brush, int size, float strenght, float fade);
        void UpdateBrushPath(Vector2 pos);
        void FinishBrushPath(Vector2 pos);
        void ResetMask( IntVector2 vec);
    }

    public class HeighmapPainter : ILayerPainter
    {

        private LayerUtility layUtils = new LayerUtility();
        private ILayer sourceLayer = null;

        // All values are stored here
        private ILayerMasked updatedLayer = null;

        // Mask is used for fade value
        private IMask updatedLayerMask = null;
        private IMask brushmask = null;
        
        private IConvolutionPlugin brush = null;
        private int brushSize = 0;
        private float brushStrenght = 0;
        private float brushFade = 0;
        private List<Vector2> brushPath = null;

        private bool initialized = false;
        private IInterpolation interpolation = new LinearClipped();
        public bool ReadyToDraw
        {
            get
            {
                return initialized;
            }
        }
        public void InitializeSource(ILayer l)
        {
            // TODO get rid of this stupid exception
            if (initialized) throw new InvalidOperationException("Is already initialized");
            initialized = true;
            sourceLayer = l;
            updatedLayer = new Layer2DObject(l.Resolution);

            ResetMask(l.Resolution);
        }

        public void ResetMask(IntVector2 resolution)
        {
            updatedLayerMask = new Mask(resolution);
            updatedLayer.Mask = updatedLayerMask;
        }
        public ILayerMasked GetPaintedLayer()
        {
            return updatedLayer;
        }
        public void BeginBrushpath(Vector2 pos, IConvolutionPlugin pluginBrush, int size, float strenght, float fade)
        {
            brushStrenght = strenght;
            brushFade = fade;
            brush = pluginBrush;
            if (brushSize != size)
            {
                brushSize = size;
                brushmask = layUtils.GetQuarterFadedCircle(size, fade, interpolation);
            }
            brushPath = new List<Vector2>() { pos };
            brush.InitializeKernelMatrix(brushSize);

            //immediate update
            UpdateBrushPath(pos);
        }
        public void FinishBrushPath(Vector2 pos)
        {
            applyTool(pos);
            reset();
        }

        public void UpdateBrushPath(Vector2 pos)
        {

            if (brushPath == null || brushPath.Count < 1) return;

            brushPath.Add(pos);

            int brushStepPx = brushSize / 4;
            int c = brushPath.Count;
            var oldPt = brushPath[c - 2];
            var newPt = brushPath[c - 1];

            var dist = Vector2.Distance(newPt, oldPt);
            var samples = (int)(dist / brushStepPx);
            
            int start = 0;
            if (c != 2)
            {
                //i=1 because it was previously used on endpoint
                start = 1;
            }

            for (int i = start; i < samples; i++)
            {
                var addition = (newPt - oldPt) / samples * i;
                Vector2 v = oldPt + addition;
                applyTool(v);
            }

            applyTool(newPt);
        }

        private void applyTool(Vector2 v)
        {
            int vx = (int)v.X;
            int vy = (int)v.Y;
            float threshold = .001f;
            for (int y = 0; y < brushSize; y++)
            {
                for (int x = 0; x < brushSize; x++)
                {
                    var val = brushmask[x, y];
                    if (!val.HasValue || val.Value < threshold) break;//circle ends here
                    
                    perf(vx+x,vy+y     ,val.Value);
                    perf(vx + x, vy - y,val.Value);
                    perf(vx - x, vy + y,val.Value);
                    perf(vx - x, vy - y,val.Value);
                }
            }
        }

        private void perf(int x, int y, float maskValue )
        {
            if (inBounds(y, x))
            {
                var fadedStrenght = brushStrenght * maskValue;

                if (!updatedLayerMask[x, y].HasValue || updatedLayerMask[x, y] < fadedStrenght)
                {
                    updatedLayerMask[x, y] = fadedStrenght;
                    updatedLayer[x, y] = brush.Apply(sourceLayer, x, y);
                }
            }
        }


        private bool inBounds(int x, int y)
        {
            return x > 0 && y > 0 && x < updatedLayer.Resolution.X && y < updatedLayer.Resolution.Y;
        }


        private void reset()
        {
            brush = null;
            brushSize = 0;
            brushPath = null;
            initialized = false;
        }

    }
}
