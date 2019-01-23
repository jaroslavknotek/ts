
using System.Collections.Generic;
using TerraSketch.DataObjects.FieldObjects;
using TerraSketch.DataObjects.ParameterObjects;

namespace TerraSketch.DataObjects
{
    
    public interface IWorld
    {
        IWorldParameters Parameters
        {
            get;
        }
        IExportParameters ExportParameters
        {
            get;
        }

        BaseField BaseField { get; }

        IFieldList Fields { get; }
        bool UseBase { get; set; }
        
    }
}