using Common.MathUtils;
using System.Collections.Generic;
using System.Numerics;
using TerraSketch.Heightmap.Tools;
using TerraSketch.Layer;

namespace TerraSketch.Presenters
{
    public partial class HeightMapPresenter
    {
        LayerUtility utils = new LayerUtility();
        ZoomManager zoomManager;

        public IntVector2 BaseResolution
        {
            get
            {
                return _masterPreseneter.World.Parameters.BitmapResolution;
            }
        }

        private int _brushSize = 10;

        public int BrushSize
        {
            get { return _brushSize; }
            set
            {
                if (value == _brushSize) return;
                _brushSize = value;
                NotifyPropertyChanged();

            }
        }


        private int _brushStrenght = 100;

        public int BrushStrenght
        {
            get { return _brushStrenght; }
            set
            {
                if (_brushStrenght == value) return;
                _brushStrenght = value;
                NotifyPropertyChanged();
            }
        }


        private int _brushFade;

        public int BrushFade
        {
            get { return _brushFade; }
            set
            {
                if (_brushFade == value) return;
                _brushFade = value;
                NotifyPropertyChanged();
            }
        }





        private LayerStats _layerStats = null;
        private ILayerMasked _beforeUpdateLayer;
        private ILayerMasked _heightmapLayer;
        public ILayerMasked HeightmapLayer
        {
            get { return _heightmapLayer; }
            private set
            {
                if (_heightmapLayer != value)
                {
                    _heightmapLayer = value;

                    if (HeightmapLayer != null)
                        _layerStats = utils.GatherStats(HeightmapLayer);
                    else
                        _layerStats = null;

                    NotifyPropertyChanged();
                }
            }
        }

        public long HeightmapVersion { get; set; }

        public float HeightmapLayerMin
        {
            get
            {
                if (_layerStats == null) return float.MinValue;
                return _layerStats.Min;

            }
        }

        public float HeightmapLayerMax
        {
            get
            {
                if (_layerStats == null) return float.MinValue;
                return _layerStats.Max;

            }
        }


        public bool HasValidPluginSelected
        {
            get
            {
                return SelectedPluginTool != Plugins.None && SelectedPluginTool != null;
            }
        }


        public bool HasInitializedLayer
        {
            get
            {
                return HeightmapLayer != null;
            }
        }

        private IConvolutionPlugin _selectedPluginTool;

        public IConvolutionPlugin SelectedPluginTool
        {
            get { return _selectedPluginTool; }
            set
            {
                if (value == _selectedPluginTool) return;
                _selectedPluginTool = value;

                NotifyPropertyChanged();
            }
        }

        public List<IConvolutionPlugin> PluginTools
        {
            get; private set;
        }

        private ILayerPainter _layerPainter;

        public ILayerPainter LayerPainter
        {
            get
            {
                if (_layerPainter == null)
                    _layerPainter = new HeighmapPainter();
                return _layerPainter ?? new HeighmapPainter();
            }
        }

        public Vector2 CurrentMousePosition
        {
            get; set;
        }


        public float Zoom
        {
            get { return zoomManager.Zoom; }
            set
            {
                if (zoomManager.Zoom == value) return;
                zoomManager.Zoom = value;

            }
        }

    }
}
