using System;
using System.Xml.Serialization;
using TerraSketch.DataObjects.ParameterObjects;

namespace TerraSketch.DataObjects.SaveLoad
{
    [Serializable]
    public class ExportParametersXmlWrapper : IExportParameters
    {

        [XmlElement("MaxHeight")]
        public float MaxHeight { get; set; }
        [XmlElement("MinHeight")]
        public float MinHeight { get; set; }

    }

    public class ExportParametersConverter
    {
        private ILoadItemParameter parameters;

        public ExportParametersConverter(ILoadItemParameter parameters)
        {
            this.parameters = parameters;

        }
        public IExportParameters ToObject(IExportParameters wrapper)
        {
            ExportParameters p = new ExportParameters();
            p.MaxHeight= wrapper.MaxHeight;
            p.MinHeight = wrapper.MinHeight;


            return p;
        }

        public ExportParametersXmlWrapper ToXmlWrapper(IExportParameters par)
        {
            ExportParametersXmlWrapper p = new ExportParametersXmlWrapper();
            p.MaxHeight= par.MaxHeight;
            p.MinHeight = par.MinHeight;

            


            return p;
        }
    }

}
