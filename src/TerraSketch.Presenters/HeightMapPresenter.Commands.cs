using System.Numerics;
using TerraSketch.Layer;
using TerraSketch.Presenters.Interfaces;

namespace TerraSketch.Presenters
{
    public partial class HeightMapPresenter
    {
        #region Refresh Heightmap
        private float _lastZoom ;
        private long _version;
        private ILayerMasked _cachedZoomedLayer;
        public ILayerMasked GetCachedZoomedHeightmap()
        {
            if (HeightmapLayer == null) return null;
            if (Zoom == 1 ) return HeightmapLayer;
            if (Zoom == _lastZoom && _cachedZoomedLayer != null && _version== HeightmapVersion) return _cachedZoomedLayer;
            _lastZoom = Zoom;
            _version = HeightmapVersion;
            Layer2DObject l = new Layer2DObject (GetRecalculatedSize());

            float xScale = (float)HeightmapLayer.Resolution.X/l.Resolution.X;
            float yScale = (float)HeightmapLayer.Resolution.Y/l.Resolution.Y;

            for (int y = 0; y < l.Resolution.Y; y++)
            {
                for (int x = 0; x < l.Resolution.X; x++)
                {
                    var oldX = (int)(x * xScale);
                    var oldY = (int)(y * yScale);
                    l[x, y] = HeightmapLayer[oldX, oldY];
                }
            }
            _cachedZoomedLayer = l;
            return l;

        }
        private ICommandWrapper _commandClearSavedHeightmap = null;
        
        public ICommandWrapper CommandClearSavedHeightmap
        {
            get {
                if(_commandClearSavedHeightmap == null)
                    _commandClearSavedHeightmap = new CommandWrapper(
                        CanExecuteCommandClearSavedHeightmap,
                        (o) => ExecuteCommandClearSavedHeightmap(o));


                return _commandClearSavedHeightmap;
                    }
        }
        bool CanExecuteCommandClearSavedHeightmap(object o)
        {
            return HeightmapLayer!=null;
        }

        void ExecuteCommandClearSavedHeightmap(object o)
        {
            HeightmapLayer = null;
            _heightmapView.ClearView();
        }
        public void BeginPaint(Vector2 vector2)
        {
            if(!CanPaint()) return;

            _beforeUpdateLayer = new Layer2DObject(HeightmapLayer.Resolution);

            utils.CloneFromTo(HeightmapLayer, _beforeUpdateLayer);
            LayerPainter.InitializeSource(this.HeightmapLayer);
            var rec = zoomManager.RecalcZoomDiv(vector2);
            LayerPainter.BeginBrushpath(rec, SelectedPluginTool, BrushSize,(float)BrushStrenght/100,(float)BrushFade /100);
        }
        public void UpdatePaint(Vector2 vector2)
        {

            // TODO async paint. 
            var rec = zoomManager.RecalcZoomDiv(vector2);
            if (LayerPainter.ReadyToDraw)
                LayerPainter.UpdateBrushPath(rec);

            updatePaintAndMask();

            _heightmapView.RefreshView();
        }

        // this is temporary method that removes old mask in order to prevent mask gaining additional values
        // final implementation should use third layer that indicates whether value was drawn or not.
        private void updatePaintAndMask()
        {
            var l = LayerPainter.GetPaintedLayer();
            utils.PaintLayerOnLayerMasked(_beforeUpdateLayer, l, HeightmapLayer);
            //LayerPainter.ResetMask(l.Resolution);
            HeightmapVersion++;
        }

        public void FinishPaint(Vector2 vector2)
        {
            if (!LayerPainter.ReadyToDraw) return;
            var rec = zoomManager.RecalcZoomDiv(vector2);
            LayerPainter.FinishBrushPath(rec);

            updatePaintAndMask();
            _heightmapView.RefreshView();
        }



        #endregion

        #region Zoom in/out commands
        
        public ICommandWrapper CommandZoomIn => zoomManager.CommandZoomIn;


        public ICommandWrapper CommandZoomOut => zoomManager.CommandZoomOut;

        #endregion

        public bool CanPaint()
        {
            return HeightmapLayer != null
                   && SelectedPluginTool != null
                   && BrushSize > 0;
        }
    }



}
