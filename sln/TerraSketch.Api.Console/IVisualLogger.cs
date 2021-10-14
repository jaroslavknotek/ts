using TerraSketch.Layer;

namespace TerraSketch.Logging
{
    public interface IVisualLogger
    {
        void Log(ILayer layer);
        void Log(ILayer layer, string name);
        void Log(ILayer layer, string name, LogImageProcessingOptions options);
    }
}