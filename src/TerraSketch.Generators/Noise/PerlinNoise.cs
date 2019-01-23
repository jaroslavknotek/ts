using Common.MathUtils;
using Common.MathUtils.Probability;
using System.Numerics;
using TerraSketch.Generators.Abstract;
using TerraSketch.Layer;

namespace TerraSketch.Generators.Noise
{
    public class PerlinNoise : INoise
    {

        /***************************************************************************************
       *    Title: Perlin noise
       *    Author: unknown
       *    Date: unknown
       *    Code version: unknown
       *    Availability: https://code.google.com/archive/p/fractalterraingeneration/wikis/Perlin_Noise.wiki
       *    This code was modified a lot. 
       ***************************************************************************************/



        private readonly IRandom0 _rand;
        private readonly INoiseParameters _noiseParameters;
        public PerlinNoise(INoiseParameters noiseParameters)
        {
            _rand = noiseParameters.Random;
            _noiseParameters = noiseParameters;
        }


        public ILayerMasked Do(Vector2 size)
        {
            //int maxDim = (int)JryMath.Max(size.X, size.Y);
//CONSTANT - There is a problem with values that are not 2^n wher n is element of N

            var maxDim = 256;
            int gradSize = 8;
            IInterpolation interpolation = new LinearClipped();

            Layer2DObject map = new Layer2DObject(size);
            int hgrid = (int)size.X;
            int vgrid = (int)size.Y;


            float gain = _noiseParameters.Amplitude;
            float lacunarity = _noiseParameters.Lacunarity;

            var gradients = setupGradient(gradSize);

            //set up the random numbers table
            int[] permutations = getPermutaions(maxDim);

            int maxDimMinOne = maxDim - 1;
            int gradSizeMinOne = gradSize - 1;

            for (int i = 0; i < vgrid; i++)
            {
                for (int j = 0; j < hgrid; j++)
                {
                    float pixel_value = 0.0f;

                    float amplitude = 1.0f;
                    float frequency = 1.0f / maxDim;

                    for (int k = _noiseParameters.FromDepth; k < _noiseParameters.ToDepth; k++)
                    {
                        int x = JryMath.Floor(j * frequency);
                        int y = JryMath.Floor(i * frequency);

                        float fracX = j * frequency - x;
                        float fracY = i * frequency - y;

                        // following two lines solved the bug.
                        x += k;
                        y += k;
                        IntVector4 v = getIndices(permutations, maxDimMinOne, gradSizeMinOne, x, y);
                        Vector2[] grads = getGrads(gradients, v);
                        float interpolatedxy = biInterpolate(interpolation, grads, fracX, fracY);

                        pixel_value += interpolatedxy * amplitude;
                        amplitude *= gain;
                        frequency *= lacunarity;
                    }

                    //put it in the map
                    map[j, i] = pixel_value;
                }
            }
            return map;
        }

        private static Vector2[] getGrads(Vector2[] gradients, IntVector4 v)
        {
            return new[] {
                            gradients[v.X],
                            gradients[v.Y],
                            gradients[v.Z],
                            gradients[v.W]
                        };
        }

        private float biInterpolate(IInterpolation interpolation, Vector2[] gradients, float fracX, float fracY)
        {
            float noise11 = dotproduct(gradients[0], fracX, fracY);
            float noise12 = dotproduct(gradients[1], fracX - 1.0f, fracY);
            float noise21 = dotproduct(gradients[2], fracX, fracY - 1.0f);
            float noise22 = dotproduct(gradients[3], fracX - 1.0f, fracY - 1.0f);

            fracX = fade(fracX);
            fracY = fade(fracY);

            float interpolatedx1 = interpolation.Interpolate(noise11, noise12, fracX);
            float interpolatedx2 = interpolation.Interpolate(noise21, noise22, fracX);

            float interpolatedxy = interpolation.Interpolate(interpolatedx1, interpolatedx2, fracY);
            return interpolatedxy;
        }

        private IntVector4 getIndices(int[] permutations, int maxDimMinOne, int gradSizeMinOne, int x, int y)
        {
            //int grad11 = (int)(rand2.NextD(x-k, y-k) * gradSize);
            //int grad12 = (int)(rand2.NextD(x + 1-k, y-k) * gradSize);
            //int grad21 = (int)(rand2.NextD(x-k, y + 1-k) * gradSize);
            //int grad22 = (int)(rand2.NextD(x + 1-k, y + 1-k) * gradSize);
            int grad11 = permutations[(x + permutations[y & maxDimMinOne]) & maxDimMinOne] & gradSizeMinOne;
            int grad12 = permutations[(x + 1 + permutations[y & maxDimMinOne]) & maxDimMinOne] & gradSizeMinOne;
            int grad21 = permutations[(x + permutations[(y + 1) & maxDimMinOne]) & maxDimMinOne] & gradSizeMinOne;
            int grad22 = permutations[(x + 1 + permutations[(y + 1) & maxDimMinOne]) & maxDimMinOne] & gradSizeMinOne;
            return new IntVector4(grad11, grad12, grad21, grad22);
        }

        private int[] getPermutaions(int maxDim)
        {
            var permutations = new int[maxDim]; //make it as long as the largest dimension
            for (int i = 0; i < maxDim; ++i)
                permutations[i] = i;//put each number in once

            //randomize the random numbers table
            for (int i = 0; i < maxDim; ++i)
            {
                var index = i;
                int j = (int)(_rand.NextF() * maxDim);
                int k = permutations[index];
                permutations[index] = permutations[j];
                permutations[j] = k;
            }

            return permutations;
        }

        private static Vector2[] setupGradient(int gradSize)
        {
            Vector2[] gradients = new Vector2[gradSize];
            for (int i = 0; i < gradients.Length; ++i)
            {
                Vector2 v = new Vector2(JryMath.Sin(0.785398163f * i)
                    , JryMath.Cos(0.785398163f * i));

                gradients[i] = v;
            }

            return gradients;
        }

        float dotproduct(Vector2 v, float x, float y)
        {
            return (v.X * x + v.Y * y);
        }

        float fade(float x)
        {
            //this equates to 6x^5 - 15x^4 + 10x^3 
            return (x * x * x * (x * (6 * x - 15) + 10));
        }

        private struct IntVector4
        {
            public int X { get; }

            public int Y { get; }
            public int Z { get; }
            public int W { get; }

            public IntVector4(int grad11, int grad12, int grad21, int grad22) : this()
            {
                this.X = grad11;
                this.Y = grad12;
                this.Z = grad21;
                this.W = grad22;
            }
        }
    }


}