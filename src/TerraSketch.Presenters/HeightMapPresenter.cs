using Common.MathUtils;
using System.Collections.Generic;
using TerraSketch.Heightmap.Tools;
using TerraSketch.Layer;

namespace TerraSketch.Presenters
{
    
    public partial class HeightMapPresenter : ABasePresenter,IPresenterZoomable
    {
        private readonly MasterPresenter _masterPreseneter;
        private readonly IHeightmapView _heightmapView;
        public event ZoomChangedEventHandler ZoomChanged;
        public HeightMapPresenter( IHeightmapView heightView, MasterPresenter mp)
        {
            this._heightmapView = heightView;
            zoomManager = new ZoomManager(heightView);
            zoomManager.ZoomChanged += ZoomManager_ZoomChanged;
            PluginTools = new List<IConvolutionPlugin>()
            {
                Plugins.None,
                Plugins.Blur,
                Plugins.Sharpen,
                Plugins.Elevate,
                Plugins.Lower
#if DEBUG
                ,Plugins.TestBrush
#endif


            };
            
            _masterPreseneter = mp;
        }

        private void ZoomManager_ZoomChanged(object sender, System.EventArgs e)
        {
            NotifyPropertyChanged(()=>Zoom);
            if(ZoomChanged!=null)
            ZoomChanged(this, new System.EventArgs());
        }

        public void InitializeGeneratedObjects(ILayerMasked obj)
        {
            HeightmapLayer = obj;
            _heightmapView.RefreshView();
            HeightmapVersion = 0;
        }
      
        public IntVector2 GetRecalculatedSize()
        {
            var res = zoomManager.RecalcZoomMult(_masterPreseneter.World.Parameters.BitmapResolution);
            return res;
        }

       
    }
}
