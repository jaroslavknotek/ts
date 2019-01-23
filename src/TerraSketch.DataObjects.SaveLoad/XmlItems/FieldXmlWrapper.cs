using System;
using System.Xml.Serialization;
using TerraSketch.DataObjects.FieldObjects;
using TerraSketch.DataObjects.ParameterObjects;

namespace TerraSketch.DataObjects.SaveLoad
{


    [Serializable]
    public class FieldXmlWrapper :IField
    {
        [XmlIgnore]
        public IFieldParameters Parameters { get { return _fld; }  set { _fld = (FieldParameterXmlWrapper)value; } }
        [XmlIgnore]        
        public IFieldPolygon Polygon { get { return _poly; }  set { _poly = (PolygonXmlWrapper)value; } }

        [XmlElement("ZOrd")]
        public int ZOrder { get;  set; }

        private FieldParameterXmlWrapper _fld;
        [XmlElement("FPars")]
        public FieldParameterXmlWrapper ParametersWrap
        {
            get { return _fld; }
            set { _fld = value; }
        }

        private PolygonXmlWrapper _poly;

                public event ZOrderChangedEvent ZOrderChanged;

        [XmlElement("Poly")]
        public PolygonXmlWrapper PolygonWrap
        {
            get { return _poly; }
            set { _poly = value; }
        }

    }
    public class FieldConverter
    {
        private ILoadItemParameter parameters;
        FieldParameterConverter _paramConverter =null;
        PolygonConverter _polyConverter = new PolygonConverter();
        public FieldConverter(ILoadItemParameter parameters)
        {
            this.parameters = parameters;
            _paramConverter = new FieldParameterConverter(parameters);
        }
        public IField ToObject(IField wrapper)
        {
            var p = _polyConverter.ToObject( wrapper.Polygon);
            var par = _paramConverter.ToObject( (FieldParameterXmlWrapper)wrapper.Parameters);
            Field f = new Field(p);
            f.Parameters.BlendModeWrap = par.BlendModeWrap;
            f.Parameters.Detail = par.Detail;
            f.Parameters.Seed = par.Seed.Value;
            f.Parameters.Offset = par.Offset;
            f.Parameters.FieldProfile = par.FieldProfile;
            f.ZOrder = wrapper.ZOrder;

            return f;
        }

        public FieldXmlWrapper ToXmlWrapper(IField param)
        {
            FieldXmlWrapper fxw = new FieldXmlWrapper
            {
                ParametersWrap = _paramConverter.ToXmlWrapper(param.Parameters),
                PolygonWrap = _polyConverter.ToXmlWrapper(param.Polygon),
                ZOrder = param.ZOrder
            };



            return fxw;
        }
    }

}
