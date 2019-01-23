using System;
using TerraSketch.DataObjects.Abstract;
using TerraSketch.DataObjects.FieldObjects.FieldParams;

namespace TerraSketch.DataObjects.ParameterObjects
{
    //
    public class FieldParameters : IFieldParameters

    {
        public IFieldDetail Detail { get;
            set; }
        
        public IFieldProfile FieldProfile { get; set; }

        public IFieldBlendMode BlendModeWrap
        {
            get;set;
        }

        public float Offset { get; set; }
        public int? Seed { get; set; }
        public FieldParameters()
        {
            Offset = .5f;
            // TODO use ioc
            Seed = new Random(DateTime.Now.Millisecond).Next(0, int.MaxValue );
        }
        
    }
}
