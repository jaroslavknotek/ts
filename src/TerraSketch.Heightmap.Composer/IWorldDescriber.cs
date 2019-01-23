using System.Collections.Generic;
using TerraSketch.DataObjects.FieldObjects;
using TerraSketch.DataObjects.ParameterObjects;
using TerraSketch.Generators.Abstract;

namespace TerraSketch.Heightmap.Composer
{
    public interface IWorldDescriber
    {
        IList<ILayerGlobalParameters> DescribeFields( IEnumerable<IField> fields);
        ILayerGlobalParameters DescribeField(IField field);
        IBaseLayerDescriptor DescribeBaseLayer(IWorldInformativeParameters param, IField f);
    }
}