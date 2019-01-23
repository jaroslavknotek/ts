using System.Threading.Tasks;
using TerraSketch.DataObjects.FieldObjects;
using TerraSketch.Heightmap.Composer;
using TerraSketch.Logging;
using TerraSketch.Resources;

namespace TerraSketch.Presenters
{
    public partial class FieldPresenter :ABasePresenter, IPresenterZoomable
    {
        readonly TaskScheduler _uiScheduler;

        public event ZoomChangedEventHandler ZoomChanged;

        public FieldPresenter(IFieldView view, MasterPresenter mp)
        {
            this.composer = new HeightmapComposer(new VisualLogger());
            composer.ErosionStarted += Composer_ErosionStarted;
            composer.LayerDescribed += Composer_LayerDescribed;
            composer.MergeStarted += Composer_MergeStarted;
            composer.LayerGenerated += Composer_LayerGenerated;
            composer.RiverGenerationStarted += Composer_RiverGenerationStarted;
            ParentPresenter = mp;
            _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            FieldView = view;

            zoomManager = new ZoomManager(view);
            zoomManager.ZoomChanged += ZoomManager_ZoomChanged;
            setupDataSources();

            setupFieldDefaults(BaseField);
            updateBasePoly();
        }

        private void Composer_RiverGenerationStarted(object sender, StatusChangedArgument arg)
        {
            FieldView.UpdateProgressBar("River generation started");
        }

        private void Composer_LayerGenerated(object sender, StatusChangedArgument arg)
        {
            FieldView.UpdateProgressBar("Layer generated");
        }

        private void Composer_MergeStarted(object sender, StatusChangedArgument arg)
        {
            FieldView.UpdateProgressBar("Merge process started");
        }

        private void Composer_LayerDescribed(object sender, StatusChangedArgument arg)
        {
            FieldView.UpdateProgressBar("Layers described");
        }

        private void Composer_ErosionStarted(object sender, StatusChangedArgument arg)
        {
            FieldView.UpdateProgressBar("Erosion started");
        }

        private void ZoomManager_ZoomChanged(object sender, System.EventArgs e)
        {
            NotifyPropertyChanged(()=>Zoom);
            if (ZoomChanged != null)
                ZoomChanged(this, new System.EventArgs());
            FieldView.RefreshView();
        }
        private void setupDataSources()
        {
            
            var dataSources = new DataSourcesMananger().GetDataSources();
            _gatheredProfiles = dataSources.Profiles;
            _gatheredDetails = dataSources.Details;
            GatheredFieldBlendModes = dataSources.FieldBlendModes;
        }
        

        public void updateBasePoly()
        {
            BaseField.UpdatePolygon(BaseResolution);
        }
        public MasterPresenter ParentPresenter { get; private set; }
        public IFieldView FieldView { get; private set; }


        private void commitPoly(IFieldPolygon poly)
        {
            Field f = new Field(poly);
            World.Fields.Add(f);
            if (SelectedField != null)
                SelectedField.Polygon.DeselectAll();
            SelectedField = f;
            // HACK i had to setup defualts here. 
            setupFieldDefaults(f);
            f.Polygon.SelectAll();

            NotifyPropertyChanged(() => SelectedField);
        }
        

        private void setupFieldDefaults(IField f)
        {
            f.Parameters.Detail = GatheredDetails[0];
            f.Parameters.FieldProfile = GatheredProfile[0];
            f.Parameters.BlendModeWrap = GatheredFieldBlendModes[0];
        }

        protected override void Dispose(bool disposing)
        {
            ZoomChanged -= ZoomManager_ZoomChanged;
            base.Dispose(true);
        }

    }

}