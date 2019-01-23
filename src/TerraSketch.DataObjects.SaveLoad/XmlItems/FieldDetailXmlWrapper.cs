using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraSketch.DataObjects.FieldObjects.FieldParams;

namespace TerraSketch.DataObjects.SaveLoad.XmlItems
{
    public class FieldDetailXmlWrapper:IFieldDetail
    {
        public string Caption
        {
            get;

            set;
        }
    }

    public class FieldDetailConverter
    {
        private ILoadItemParameter parameters;

        public FieldDetailConverter( ILoadItemParameter parameters)
        {
            this.parameters = parameters;
        }
        public IFieldDetail ToObject( IFieldDetail wrapper)
        {
            AFieldDetail b = parameters.Details.First(r => r.Caption == wrapper.Caption);

            return b;
        }

        public FieldDetailXmlWrapper ToXmlWrapper(IFieldDetail param)
        {
            var txw = new FieldDetailXmlWrapper();
            txw.Caption = param.Caption;
            return txw;
        }
    }
}
