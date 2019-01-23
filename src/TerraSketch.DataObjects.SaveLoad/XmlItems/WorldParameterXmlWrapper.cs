using System;
using System.Xml.Serialization;
using TerraSketch.DataObjects.ParameterObjects;

namespace TerraSketch.DataObjects.SaveLoad
{
    [Serializable]
    public class WorldParametersXmlWrapper : IWorldBindableParameters
    {
        

        [XmlElement("ResStr")]
        public string BitmapResolutionString { get; set; }

        [XmlElement("ErosionStrength")]
        public int ErosionStrength { get; set; }
        [XmlElement("RiverAmount")]
        public int RiverAmount { get; set; }
    }

    public class WorldParametersConverter
    {
        private ILoadItemParameter parameters;

        public WorldParametersConverter(ILoadItemParameter parameters)
        {
            this.parameters = parameters;

        }
        public IWorldParameters ToObject(IWorldBindableParameters wrapper)
        {
            WorldParameters p = new WorldParameters();
            p.BitmapResolutionString = wrapper.BitmapResolutionString;
            p.ErosionStrength = wrapper.ErosionStrength;
            p.RiverAmount = wrapper.RiverAmount;
            return p;
        }

        public WorldParametersXmlWrapper ToXmlWrapper(IWorldBindableParameters par)
        {
            WorldParametersXmlWrapper p = new WorldParametersXmlWrapper();
            
            p.BitmapResolutionString = par.BitmapResolutionString;
            p.ErosionStrength = par.ErosionStrength;
            // TODO use fucking automapper
            p.RiverAmount = par.RiverAmount;
            return p;
        }
    }

}
