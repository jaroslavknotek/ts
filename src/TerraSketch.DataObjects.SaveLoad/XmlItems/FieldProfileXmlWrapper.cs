using System.Linq;
using TerraSketch.DataObjects.FieldObjects.FieldParams;

namespace TerraSketch.DataObjects.SaveLoad.XmlItems
{
    
    public class FieldProfileXmlWrapper : IFieldProfile
    {
        public string Caption
        {
            get;

            set;
        }
    }

    public class FieldProfileConverter
    {
        private ILoadItemParameter parameters;

        public FieldProfileConverter(ILoadItemParameter parameters)
        {
            this.parameters = parameters;
        }
        public IFieldProfile ToObject( IFieldProfile wrapper)
        {
            AFieldProfile b = parameters.Profiles.First(r => r.Caption == wrapper.Caption);

            return b;
        }

        public FieldProfileXmlWrapper ToXmlWrapper(IFieldProfile param)
        {
            FieldProfileXmlWrapper txw = new FieldProfileXmlWrapper();
            txw.Caption = param.Caption;
            return txw;
        }
    }
}
