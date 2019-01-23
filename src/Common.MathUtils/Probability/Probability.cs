using System;
using Troschuetz.Random;
namespace Common.MathUtils.Probability
{
    public class PropabilityHelperRandom0
    {
        public float NextInRange(IRandom0 rand, float from, float to)
        {
            if (from > to) throw new ArgumentException("from > to");
            var d = rand.NextF();

            var res = from + (to - from) * d;
            return (float)res;
        }
    }
    
    //public class PrecomputedRandom :IRandom3
    //{
    //    private ContinuousUniformDistribution rand = null;
    //    private float[] precomputed = null;
    //    public PrecomputedRandom(int amount, int seed)
    //    {
    //        var g = new Troschuetz.Random.MT19937Generator(seed);
    //        rand = new ContinuousUniformDistribution(g);
    //        precomputed= intitArray(amount);
    //    }

    //    public float NextD(int seed1, int seed2, int seed3)
    //    {
    //        int i = (seed1 + 1) * (seed2 + 7) * (seed2 + 7) * (seed3 + 13) * (seed3 + 2) >> 5;
    //        var rindex = JryMath.Abs(i);
    //        return precomputed[rindex % precomputed.Length];

    //    }

    //    private float[] intitArray(int s)
    //    {
    //        float[] f = new float[s];
    //        for (int i = 0; i < s; i++)
    //        {
    //            f[i] = (float)rand.NextDouble() - 0.5f;
    //        }
    //        return f;
    //    }


    //}

    public class RandomSeeded : IRandom3, IRandom2
    {
        private readonly int _seed;
        public RandomSeeded(int seed)
        {
            _seed = seed;
        }
        public virtual float NextD(int seedX, int seedY)
        {
            seedX += 3* _seed ;
            seedY += 5 * _seed ;
            var n = seedX + seedY * 57;
            n = (n << 13) ^ n;
            // it must fit into 0-1
            var ra = (float)JryMath.Abs((1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0));

            var intra = (long)ra;
            return ra - intra ;
        }

        public float NextD(int seedX, int seedY, int seedZ)
        {
            seedX += _seed;
            seedY += 5 * _seed;
            seedZ += 13 * _seed;
            var n = seedX + seedY * 57 + seedZ * 271;
            n = (n << 13) ^ n;
            var ra = (float)JryMath.Abs((1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0));
            var intra = (long)ra;
            
            return  ra - intra;
        }

    }
    //public class FakeRandom2 : IRandom2
    //{
    //    ContinuousUniformDistribution rand = null;
    //    public FakeRandom2(int seed)
    //    {
    //        var g = new Troschuetz.Random.MT19937Generator(seed);
    //        rand = new ContinuousUniformDistribution(g);
    //    }
    //    public float NextD(int seed1, int seed2)
    //    {
    //        return (float)rand.NextDouble();
    //    }
    //}

    public class Rand :IRandom0
    {
        ContinuousUniformDistribution cud = null;
        public Rand(int seed)
        {
            var g = new MT19937Generator(seed);
            cud = new ContinuousUniformDistribution(g);
        }

        public float NextF()
        {
            return (float)cud.NextDouble();
        }
    }

    
    public interface IRandom3
    {
        float NextD(int seed1, int seed2, int seed3);
    }
    public interface IRandom2
    {
        float NextD(int seed1, int seed2);
    }

    public interface IRandom0
    {
        float NextF();
    }
}
