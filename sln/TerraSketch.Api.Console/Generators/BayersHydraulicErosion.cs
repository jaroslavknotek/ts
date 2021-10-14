using System.Numerics;
using Common.MathUtils;
using TerraSketch.Generators.Abstract;
using TerraSketch.Layer;

namespace TerraSketch.Generators
{
    //public class BayersHydraulicErosion : IErosionType
    //{

    //    private float rain_amount = .5f; //: how much rain is deposited on each cell each iteration
    //    private float evaporation = .1f; // what percentage of water evaporates each iteration
    //    private ILayer _water;
    //    private float solubility = .05f; //: how much soil is eroded into sediment each iteration

    //    // solubility = capacity => no need for following
    //    //private float[,] _sediment = null;
    //    //private float capacity = 1;     //: how much sediment a given unit of water can hold


    //    public void Erode(ILayer layer, IErosionParameters par)
    //    {

    //    }


    //    private class Drop
    //    {
    //        public IntVector2 Position { get; set; }
    //        public Vector2 FlowDirection { get; set; }

    //        public float Velocity { get; set; }
    //        public float Water { get; set; }

    //        public float Sediment { get; set; }

    //        public Vector2 Gradient { get; set; }
    //    }
    //}
}