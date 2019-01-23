using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TerraSketch.DataObjects.FieldObjects;
using TerraSketch.DataObjects.ParameterObjects;

namespace TerraSketch.DataObjects.SaveLoad
{
    [Serializable]
    public class WorldXmlWrapper : IWorld
    {

        private List<FieldXmlWrapper> _fields;
        [XmlElement("Fields")]
        public List<FieldXmlWrapper> FieldsWrap
        {
            get { return _fields; }
            set { _fields = value; }
        }

        [XmlIgnore]

        public IFieldList Fields
        {
            get
            {
                var fl = new FieldList();
                _fields.ForEach(r => fl.Add(r));
                return fl;
                ;
            }
            set { _fields = (List<FieldXmlWrapper>)value; }
        }


        private WorldParametersXmlWrapper _paramWrap;
        [XmlElement("WPar")]
        public WorldParametersXmlWrapper ParametersWrap
        {
            get { return _paramWrap; }
            set { _paramWrap = value; }
        }

        [XmlIgnore]

        public IWorldParameters Parameters
        {
            get;
            set;
        }

        [XmlElement("UseBase")]
        public bool UseBase
        {
            get; set;
        }

        [XmlIgnore]
        public BaseField BaseField
        {
            get; set;
        }
        [XmlElement("BaseLayer")]
        public FieldXmlWrapper BaseFieldXml { get; set; }




        private ExportParametersXmlWrapper _expParamWrap;
        [XmlElement("ExpPar")]
        public ExportParametersXmlWrapper ExportParametersWrap
        {
            get { return _expParamWrap; }
            set { _expParamWrap = value; }
        }

        [XmlIgnore]

        public IExportParameters ExportParameters
        {
            get;
            set;
        }
    }

    public class WorldConverter
    {
        FieldConverter fpc = null;
        private ILoadItemParameter parameters;
        WorldParametersConverter wpc = null;

        public WorldConverter(ILoadItemParameter parameters)
        {
            this.parameters = parameters;
            wpc = new WorldParametersConverter(parameters);
            fpc = new FieldConverter(parameters);
        }
        public IWorld ToObject(WorldXmlWrapper wrapper)
        {
            World p = new World();
            var par = wpc.ToObject(wrapper.ParametersWrap);
            p.UseBase = wrapper.UseBase;
            var bf = fpc.ToObject(wrapper.BaseFieldXml);

            p.BaseField = new BaseField(bf.Polygon, bf.Parameters);

            p.Parameters.BitmapResolutionString = par.BitmapResolutionString;
            p.Parameters.ErosionStrength = par.ErosionStrength;


            p.ExportParameters.MaxHeight = wrapper.ExportParametersWrap.MaxHeight;
            p.ExportParameters.MinHeight = wrapper.ExportParametersWrap.MinHeight;

            foreach (var fld in wrapper.Fields)
            {
                p.Fields.Add(fpc.ToObject(fld));
            }

            return p;
        }


        public WorldXmlWrapper ToXmlWrapper(IWorld wor)
        {
            WorldXmlWrapper p = new WorldXmlWrapper();
            p.UseBase = wor.UseBase;
            var par = wor.Parameters;
            p.BaseFieldXml = fpc.ToXmlWrapper(wor.BaseField);

            p.FieldsWrap = new List<FieldXmlWrapper>();

            p.ExportParametersWrap = new ExportParametersXmlWrapper();
            p.ExportParametersWrap.MinHeight = wor.ExportParameters.MinHeight;
            p.ExportParametersWrap.MaxHeight = wor.ExportParameters.MaxHeight;

            p.ParametersWrap = new WorldParametersXmlWrapper();
            p.ParametersWrap.BitmapResolutionString = par.BitmapResolutionString;
            p.ParametersWrap.ErosionStrength = par.ErosionStrength;

            foreach (var fld in wor.Fields)
            {
                p.FieldsWrap.Add(fpc.ToXmlWrapper(fld));
            }

            return p;
        }




    }
}
