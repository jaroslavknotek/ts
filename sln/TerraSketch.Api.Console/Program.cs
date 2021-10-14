using System;
using System.Numerics;

namespace TerraSketch.ConsoleApi
{
     public class SysRand : Common.MathUtils.Probability.IRandom0
    {
        private readonly Random _rand ;
        public SysRand(int seed)
        {
            _rand = new Random(123);
        }

        public float NextF()
        {
            return (float)_rand.NextDouble();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var noiseParams = new TerraSketch.Generators.Noise.NoiseParameters()
            {
                Amplitude = 2,
                BaseAmplitude = 1,
                FromDepth = 5,
                ToDepth = 7,
                Lacunarity = 2,
                Random = new SysRand(123)
            };
            var perlin = new TerraSketch.Generators.Noise.PerlinNoise(noiseParams);
            var size = 128;
            var area = perlin.Do(new Vector2(size,size));
            
            var ramp = " .:-=+*#%@".ToCharArray();
            
            var max = -1;
            var min = int.MaxValue;
            for (int y = 0; y < area.Resolution.Y; y++)
            {
                for (int x = 0; x < area.Resolution.X; x++)
                {
                    var value = area[x,y];
                    var v = (int)( value * 10);
                    Console.Write(ramp[v % ramp.Length]);
                    max = Math.Max(v,max);
                    min = Math.Min(v,min);
                }
                Console.WriteLine();
            }
            Console.WriteLine($"max: {max} - min: {min}");
        }
    }
}
