
using TerraSketch.DataObjects.ParameterObjects;

namespace TerraSketch.DataObjects.FieldObjects
{
    
    public interface IField
    {
        int ZOrder { get; set; }

        IFieldParameters Parameters { get; }


       IFieldPolygon Polygon { get; set; }
        event ZOrderChangedEvent ZOrderChanged;
    }
}