using System;
using System.Threading.Tasks;
using TerraSketch.DataObjects;
using TerraSketch.Layer;

namespace TerraSketch.Heightmap.Composer
{

    public class StatusChangedArgument : EventArgs
    {
        //public string Message { get; }

        //public StatusChangedArgument(
        //    string messange
        //    )
        //{
        //    Message = messange;
        //}
    }

    public delegate void StatusChanged(object sender, StatusChangedArgument arg);
    public interface IHeightmapComposer
    {
        event StatusChanged LayerDescribed;
        event StatusChanged MergeStarted;
        event StatusChanged LayerGenerated;
        event StatusChanged ErosionStarted;
        event StatusChanged RiverGenerationStarted;
        ILayerMasked ComposedLayer { get; }
        Task Compose(IWorld w);
    }
}