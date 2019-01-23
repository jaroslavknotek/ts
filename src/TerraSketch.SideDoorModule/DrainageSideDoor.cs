using System.IO;
using System.Threading.Tasks;
using TerraSketch.DataObjects.SaveLoad;
using TerraSketch.Heightmap.Composer;
using TerraSketch.Resources;

namespace TerraSketch.SideDoorModule
{
    public class TestResource

    {
        // TODO make single resource manager.
        public LoadItemsParameter GetDataSources()
        {
            var ds = new DataSourcesMananger().GetDataSources();

            return new LoadItemsParameter()
            {
                Details = ds.Details,
                Profiles = ds.Profiles,
                BlendModes = ds.FieldBlendModes
            };
        }
    }


    class DSTest
    {
        public void Test(ISideDoor ds)
        {
            ds.SaveLayers().Wait();
        }
        
    }

    // preprared for unit testing
    class DrainageSideDoor:ISideDoor
    {
        private readonly ISaveLoadManager _saveLoadManager;
        private readonly IHeightmapComposer _composer;
        public DrainageSideDoor(ISaveLoadManager saveLoadManager, IHeightmapComposer composer)
        {
            _saveLoadManager = saveLoadManager;
            _composer = composer;
        }
        public async Task SaveLayers()
        {
            var worldSaveItem = _saveLoadManager.Load(new FileInfo("SavedFiles/canyonMountain"));
            await _composer.Compose(worldSaveItem.World);

        }
    }
    class CommonSideDoor :ISideDoor
    {
        private readonly ISaveLoadManager _saveLoadManager;
        private readonly IHeightmapComposer _composer;
        public CommonSideDoor(ISaveLoadManager saveLoadManager, IHeightmapComposer composer)
        {
            _saveLoadManager = saveLoadManager;
            _composer = composer;
        }
        public async Task SaveLayers()
        {
            var worldSaveItem = _saveLoadManager.Load(new FileInfo("SavedFiles/canyonMountain"));
            await _composer.Compose(worldSaveItem.World);

        }
    }


    internal interface ISideDoor
    {
        Task SaveLayers();
    }
}
