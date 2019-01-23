using System.IO;

namespace TerraSketch.DataObjects.SaveLoad
{
    public interface ISaveLoadManager
    {
        ISaveItem Load(FileInfo fi);
        void Save(ISaveItem si, FileInfo fi);

    }
}