#define xImmediateApply
using System.Numerics;
using Common.MathUtils;
using TerraSketch.Generators.Abstract;
using TerraSketch.Layer;
namespace TerraSketch.Generators
{

    public class BasicHydraulicErosion : IErosionType
    {
        private readonly LayerUtility _layerUtility = new LayerUtility();

        
        private float rain_amount = .07f;  //: how much rain is deposited on each cell each iteration
        private float evaporation = .7f;  // what percentage of water evaporates each iteration
        private float solubility = .05f;  //: how much soil is eroded into sediment each iteration
        private ILayer _water;
        private ILayer _waterDiff;
        private ILayer _sediment;
        
        /// <summary>
        /// Does nothing when par.Strenght == 0
        /// Otherwise does what interface requires... erodes
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="par"></param>
        public void Erode(ILayer layer, IErosionParameters par)
        {
            if (par.Strenght == 0) return;

            int iterations = par.Strenght;
            init(layer.Resolution);
            for (int i = 0; i < iterations; i++)
            {
                iteration(layer);
            }
        }

        private void init(IntVector2 layerResolution)
        {

            _water = new Layer2DObject(layerResolution);
            _sediment = new Layer2DObject(layerResolution);
            _waterDiff = new Layer2DObject(layerResolution);
            new LayerUtility().IterateValues(_water, (v, val) =>
                {
                    _water[v] = 0;
                    _sediment[v] = 0;
                    _waterDiff[v] = 0;
                }
            );
        }

        private void iteration(ILayer layer)
        {

            for (int y = 0; y < layer.Resolution.Y; y++)
            {
                for (int x = 1; x < layer.Resolution.X; x++)
                {
                    //rainfall
                    _water[x, y] += rain_amount;
                }
            }

            for (int y = 0; y < layer.Resolution.Y; y++)
            {
                for (int x = 0; x < layer.Resolution.X; x++)
                {
                    performStep(layer, x, y);
                }
            }

#if !ImmediateApply

            for (int y = 0; y < layer.Resolution.Y; y++)
            {
                for (int x = 0; x < layer.Resolution.X; x++)
                {
                    //transfer water
                    _water[x, y] += _waterDiff[x, y];
                    //transfer sediment
                    layer[x, y] += _sediment[x, y];

                    _waterDiff[x, y] = 0;
                    _sediment[x, y] = 0;
                    //evaporation
                    _water[x, y] *= 1 - evaporation;
                }
            }
#endif
        }

        
        private void performStep(ILayer layer, int x, int y)
        {


            Vector2 lowest = _layerUtility.GetLowestNeighbour(layer, x, y);
            var lowestX = (int)lowest.X;
            var lowestY = (int)lowest.Y;

            if (lowestX == x && lowestY == y) return;
            //erosion

            var lowestWater = _water[lowestX, lowestY];
            var lowHeight = layer[lowestX, lowestY];
            var currentWater = _water[x, y];
            var currentHeight = layer[x, y];


            // sure it exists
            // ReSharper disable once PossibleInvalidOperationException
            float lowestLevel = lowestWater.Value
            // sure it exists
            // ReSharper disable once PossibleInvalidOperationException
                + lowHeight.Value;
            // sure it exists
            // ReSharper disable once PossibleInvalidOperationException
            float currentLevel = currentWater.Value
            // sure it exists
            // ReSharper disable once PossibleInvalidOperationException
                + currentHeight.Value;
            if (lowestLevel + currentWater < currentHeight.Value)
            {
                // case 3
                //transfer water completely 

                _waterDiff[lowestX, lowestY] += _water[x, y];
                _waterDiff[x, y] -= _water[x, y];
                //transfer sediment
                var sediment = _water[x, y] * solubility;
                //transfer from
                _sediment[x, y] -= sediment;
                //transfer to
                _sediment[lowest] += sediment;
            }
            else if (lowestLevel < currentLevel)
            {
                // case 2
                //transfer water completely 
                var transferedWater = JryMath.Min(currentLevel - lowestLevel, currentWater.Value) / 2;
                _waterDiff[lowestX, lowestY] += transferedWater;
                _waterDiff[x, y] -= transferedWater;

                //transfer sediment
                var factor = transferedWater / currentWater;

                var scaledSolubility = _water[x, y] * solubility * factor;
                var solub = scaledSolubility;
                //transfer from
                _sediment[x, y] -= solub;
                //transfer to
                _sediment[lowest] += solub;

            }
            //else  lowest one is not lower than current one

#if ImmediateApply
            // test codee
            _water[x, y] += _waterDiff[x, y];
            //transfer sediment
            layer[x, y] += _sediment[x, y];

            _waterDiff[x, y] = 0;
            _sediment[x, y] = 0;
            //evaporation
            _water[x, y] *= 1 - evaporation;
#endif
        }

        

        
    }
}