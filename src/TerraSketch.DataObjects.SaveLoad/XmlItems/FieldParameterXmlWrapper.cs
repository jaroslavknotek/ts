using System;
using TerraSketch.DataObjects.ParameterObjects;
using System.Xml.Serialization;
using TerraSketch.DataObjects.Abstract;
using TerraSketch.DataObjects.FieldObjects.FieldParams;
using TerraSketch.DataObjects.SaveLoad.XmlItems;

namespace TerraSketch.DataObjects.SaveLoad
{
    public class FieldParameterConverter
    {
        
        
        FieldDetailConverter fd = null;
        FieldBlendModeConverter fb = null;
        FieldProfileConverter fp = null;
        private ILoadItemParameter parameters;

        public FieldParameterConverter(ILoadItemParameter parameters)
        {
            this.parameters = parameters;
        
            fd = new FieldDetailConverter(parameters);
            fp = new FieldProfileConverter(parameters);
            fb = new FieldBlendModeConverter(parameters);
        }
        public IFieldParameters ToObject(FieldParameterXmlWrapper wrapper)
        {
            FieldParameters f = new FieldParameters();

            f.FieldProfile = fp.ToObject(wrapper.Profile);
            f.Detail = fd.ToObject( wrapper.Detail);
            f.Offset = wrapper.Offset;
            f.BlendModeWrap = fb.ToObject(wrapper.Blend);
            f.Seed = wrapper.Seed;
            return f;
        }

        public FieldParameterXmlWrapper ToXmlWrapper(IFieldParameters param)
        {
            FieldParameterXmlWrapper fpxw = new FieldParameterXmlWrapper();
            fpxw.Detail = fd.ToXmlWrapper(param.Detail);
            fpxw.Offset = param.Offset;
            fpxw.Profile = fp.ToXmlWrapper(param.FieldProfile);
            fpxw.Blend = fb.ToXmlWrapper(param.BlendModeWrap);
            fpxw.Seed = param.Seed;
            return fpxw;
        }
    }
    [Serializable]
    public class FieldParameterXmlWrapper : IFieldParameters
    {
        [XmlElement("Detail")]
        public FieldDetailXmlWrapper Detail { get; set; }


   

        [XmlElement("Profile")]
        public FieldProfileXmlWrapper Profile { get; set; }

        [XmlElement("BlendMode")]
        public FieldBlendModeXmlWrapper Blend { get; set; }


        [XmlIgnore]
        IFieldProfile IFieldParameters.FieldProfile { get; set; }

        [XmlIgnore]
        IFieldDetail IFieldParameters.Detail { get; set; }
        

        [XmlIgnore]
        public IFieldBlendMode BlendModeWrap
        {
            get;
            set;
        }
        [XmlElement("Offset")]
        public float Offset
        {
            get;

            set;
            
        }
        [XmlElement("Seed")]
        public int? Seed
        {
            get;set;
        }
    }

}
