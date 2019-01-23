using System;
using Troschuetz.Random;

namespace Common.MathUtils.Probability
{

    public class SeedProvider
    {
        private const int MAX = int.MaxValue;
        private static SeedProvider _inst;
        private int[] seedList = null;
        private int seedIndex = 0;

        public static SeedProvider Instance { get
            {
                if (_inst == null) _inst = new SeedProvider(0);
                return _inst;
            }
        }

        public int InputSeed { get; set; }


        
        private SeedProvider(int seed)
        {
            InputSeed = seed;
            var amount = 512;
            precomputeSeeds(seed,amount);
        }

        public int NextSeed()
        {
            return seedList[seedIndex++ % seedList.Length];
        }

        private void precomputeSeeds(int seed, int amount)
        {
            var g = new MT19937Generator(seed);
            ContinuousUniformDistribution rand  = new ContinuousUniformDistribution(g);
            seedList = new int[amount];
            for (int i = 0; i < seedList.Length; i++)
                seedList[i] = (int)(rand.NextDouble() * MAX);


        }

        public static void ChangeSeed(int value)
        {
            _inst = new SeedProvider(value);
        }
    }
}
