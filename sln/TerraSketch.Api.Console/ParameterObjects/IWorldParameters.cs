using Common.MathUtils;


namespace TerraSketch.DataObjects.ParameterObjects
{

    public interface IWorldParameters : IWorldBindableParameters, IWorldInformativeParameters
    {

    }

    public interface IWorldInformativeParameters
    {

        IntVector2 BitmapResolution
        {
            get;
            set;
        }


    }

    public interface IWorldBindableParameters
    {

        string BitmapResolutionString
        {
            get;
            set;
        }

        int ErosionStrength { get; set; }
        int RiverAmount { get; set; }
    }
}