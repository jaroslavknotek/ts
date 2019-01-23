using System;
using System.Linq;
using System.Xml.Serialization;
using TerraSketch.DataObjects.Abstract;
using TerraSketch.DataObjects.FieldObjects.FieldParams;
using TerraSketch.Layer.BlendModes;

namespace TerraSketch.DataObjects.SaveLoad
{

    public class FieldBlendModeXmlWrapper : IFieldBlendMode
    {
        [XmlIgnore]
        public IBlendMode BlendMode { get; set; }

        public string Caption
        {
            get;set;
        }
    }

    public class FieldBlendModeConverter
    {
        private ILoadItemParameter parameters;

        public FieldBlendModeConverter(ILoadItemParameter parameters)
        {
            this.parameters = parameters;
        }
        public IFieldBlendMode ToObject(IFieldBlendMode wrapper)
        {
            AFieldBlendMode b = parameters.BlendModes.First(r => r.Caption == wrapper.Caption);

            return b;
        }

        public FieldBlendModeXmlWrapper ToXmlWrapper(IFieldBlendMode param)
        {
            var txw = new FieldBlendModeXmlWrapper();
            txw.Caption = param.Caption;
            return txw;
        }
    }
}