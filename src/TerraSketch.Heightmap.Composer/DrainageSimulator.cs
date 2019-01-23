using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Common.MathUtils;
using Common.MathUtils.Probability;
using TerraSketch.Generators.Noise;
using TerraSketch.Layer;
using TerraSketch.Logging;

namespace TerraSketch.Heightmap.Composer
{

    public class BallDrainageSimulator : IDrainageSimulator
    {
        private readonly LayerUtility _layerUtility = new LayerUtility();
        private const int riverMark = -1;

        public ILayer GetDrainageMap(ILayer input, int riverCount)
        {
            var layerNoised = slightlyNoiseLayer(input);
            var localMaximae = getLocalMaximaRecord(layerNoised, riverCount);
            uint i = 0;
            ILayer finalLayer = new Layer2DObject(input.Resolution);
            foreach (var max in localMaximae)
            {
                ILayer layer = new Layer2DObject(input.Resolution);
                drawOnLayer(layerNoised, layer, max, ++i);
                _layerUtility.IterateValues(layer, (vv, val) =>
                {
                    if (val.HasValue && val.Value==riverMark)
                        finalLayer[vv] = 1;
                    ;
                });
            }
            return finalLayer;
        }

        private ILayer slightlyNoiseLayer(ILayer input)
        {
            var noise = getNoise(input);
            new VisualLogger().Log(noise, "noise");
            ILayer baseLayer = new Layer2DObject(input.Resolution);
            _layerUtility.IterateValues(baseLayer,
                (vv, val) => baseLayer[vv] = 0);

            ILayer layerNoised = new Layer2DObject(input.Resolution);
            _layerUtility.IterateValues(input,
                (vv, val) => layerNoised[vv] = val + .05f * noise[vv]);
            new VisualLogger().Log(layerNoised, "noised");
            return layerNoised;
        }

        private ILayer getNoise(ILayer input)
        {
            var rand = new Rand(123456);

            var v = new PerlinNoise(new NoiseParameters()
            {
                FromDepth = 1,
                ToDepth = 7,
                Amplitude = .8f,
                BaseAmplitude = 1,
                Lacunarity = 2.5f,
                Random = rand
            }
            );


            return v.Do(input.Resolution);
        }
        
        private void drawOnLayer(ILayer source, ILayer targer, IntVector2 begin, UInt32 riverId)
        {
            var heap = new HeapWrapper();
            heap.Add(1, begin);
            var dbRiver = new Dictionary<IntVector2, int>();
            int x = 0;
            while (heap.Count != 0)
            {
                var current = heap.Get();
                targer[current] = riverMark;
                x++;
                try
                {
                    dbRiver.Add(current, x);
                }
                catch (Exception)
                {

                }
                if (isEnd(source, current))
                    break;
                var moore = getAllNonRiverNeighbors(source, targer, current, riverId);
                foreach (var vec in moore)
                {
                    targer[vec] = riverId;
                    // ReSharper disable once PossibleInvalidOperationException
                    heap.Add(source[vec].Value, vec);
                }
            }





            //bool canContinue = true;
            //var current = begin;
            //int iteration = 1;
            //targer[begin] = -1;
            //while (canContinue)
            //{
            //    if (iteration == 223)
            //    {

            //    }
            //    var lowest = getLowestNonVisited(source, targer, current, iteration++);
            //    if (lowest == new IntVector2(0, 0))
            //    {

            //    }

            //    targer[lowest] = -1;
            //    canContinue = !isEnd(source, lowest);
            //    current = lowest;
            //}


        }
        
        private IEnumerable<IntVector2> getAllNonRiverNeighbors(ILayer source, ILayer visited, IntVector2 coordinate, UInt32 riverId)
        {
            int x = coordinate.X;
            int y = coordinate.Y;
            var moore = getMoore(x, y);
            return
                moore.Where(r => _layerUtility.InBounds(source.Resolution, r)
                                 && visited[r] != riverMark
                                 && visited[r] != riverId
                );

        }
        
        private static IntVector2[] getMoore(int x, int y)
        {
            IntVector2[] moore =
            {
                new IntVector2(x + 1, y),
                new IntVector2(x - 1, y),
                new IntVector2(x, y + 1),
                new IntVector2(x, y - 1),
                new IntVector2(x + 1, y + 1),
                new IntVector2(x - 1, y - 1),
                new IntVector2(x - 1, y + 1),
                new IntVector2(x + 1, y - 1)
            };
            return moore;
        }
        
        private bool isEnd(ILayer source, Vector2 lowest)
        {
            // add another rivver
            return lowest.X == 0 || lowest.X == source.Resolution.X - 1
                || lowest.Y == 0 || lowest.Y == source.Resolution.Y - 1;
        }
        
        private IEnumerable<IntVector2> getLocalMaximaRecord(ILayer layer, int amount)
        {
            var someSprings = new List<IntVector2>();
            var x =new  HeapWrapper();
            _layerUtility.IterateValues(layer, (cor,val) => x.Add(-val.Value,cor));


            var upper10Percent = layer.Resolution.X * layer.Resolution.Y / 5;
            var takeEveryNth = (int)JryMath.Ceil(upper10Percent / (float)amount);
            for (int i = 0; i < upper10Percent; i++)
            {
                var item = x.Get();
                if (i % takeEveryNth == 0)
                    someSprings.Add(item);
            }
            return someSprings;
        }



        private class HeapWrapper
        {
            private readonly SortedDictionary<float, IList<IntVector2>> heap = new SortedDictionary<float, IList<IntVector2>>();


            public int Count => heap.Count;


            public IntVector2 Get()
            {
                var rec = heap.First();
                IList<IntVector2> coorList = rec.Value;
                var key = rec.Key;

                var result = coorList[coorList.Count - 1];
                coorList.RemoveAt(coorList.Count - 1);

                if (coorList.Count == 0)
                {
                    heap.Remove(key);
                }
                return result;
            }
            public bool TryGet(out IntVector2 result)
            {

                if (heap.Count == 0)
                {
                    result = new IntVector2();
                    return false;
                }
                result = Get();
                return true;
            }

            public void Add(float height, IntVector2 coordinate)
            {
                IList<IntVector2> coorList;

                if (heap.TryGetValue(height, out coorList))
                {
                    coorList.Add(coordinate);
                }
                else
                {
                    heap.Add(height, new List<IntVector2>() { coordinate });
                }
            }


        }

    }


    public interface IDrainageSimulator
    {
        ILayer GetDrainageMap(ILayer layer, int riverCount);
    }
    public interface IEdgeDetector
    {
        ILayer GetEdges(ILayer layer);
    }


}
