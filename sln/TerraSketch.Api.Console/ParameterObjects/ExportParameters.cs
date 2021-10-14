namespace TerraSketch.DataObjects.ParameterObjects
{

    public class ExportParameters : IExportParameters
    {

        public ExportParameters()
        {
            MaxHeight = 100;
            MinHeight = 0;
        }
        public float MaxHeight
        {
            get; set;
        }

        public float MinHeight
        {
            get; set;
        }
    }
}
