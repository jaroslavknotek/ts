using TerraSketch.DataObjects;
using TerraSketch.DataObjects.SaveLoad;
using TerraSketch.VisualPresenters;

namespace TerraSketch.Presenters
{
    public partial class MasterPresenter
    {
        public MasterPresenter(IMasterView view)
        {
            MasterView = view;
            World = new World();

            
        }

        public void SetupPresenters(FieldPresenter fp, HeightMapPresenter hp, Visualization3DPresenter vp)
        {
            FieldPresenter = fp;
            HeightmapPresenter = hp;
            Visualization3DPresenter = vp;

            var param = new LoadItemsParameter()
            {
                Details = FieldPresenter.GatheredDetails,
                Profiles = FieldPresenter.GatheredProfile,
                BlendModes = FieldPresenter.GatheredFieldBlendModes
            };
            SaveLoadManager = new SaveLoadManager(param);
        }

        public FieldPresenter FieldPresenter
        {
            get;
           private set;

        }
        public HeightMapPresenter HeightmapPresenter
        {
            get;
            private set;
        }
        public Visualization3DPresenter Visualization3DPresenter
        { get; private set; }
        public IMasterView MasterView { get; private set; }


        public ISaveLoadManager SaveLoadManager { get; private set; }

    }

}