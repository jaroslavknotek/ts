using TerraSketch.DataObjects.FieldObjects;
using TerraSketch.DataObjects.ParameterObjects;

namespace TerraSketch.DataObjects
{

    
    public class World :IWorld
    {

        public IWorldParameters Parameters
        {
            get; private set;
        }

        public World()
        {
            ExportParameters = new ExportParameters();
            Parameters = new WorldParameters();
            BaseField = new FieldObjects.BaseField();
        }
        
        private IFieldList _fields = new FieldList();
        public IFieldList Fields
        {
            get { return _fields; }
        }

        public BaseField BaseField
        {
            get; set;
        }

        public bool UseBase
        { get; set; }

        public IExportParameters ExportParameters
        {
            get;
        }
    }
}
