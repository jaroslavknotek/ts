#define LINx
using System.Numerics;
using Common.MathUtils;
using TerraSketch.Generators.Abstract;
using TerraSketch.Layer;

namespace TerraSketch.Generators.Noise
{
    //public class FractalNoise : INoise
    //{
    //    private readonly LayerUtility _layerUtils = new LayerUtility();

    //    public INoise Noise { get; set; } = new PerlinNoise();

    //    public ILayerMasked Do(INoiseParameters f, Vector2 size)
    //    {
    //        var myOctave = 3;
    //        var maxOctave = JryMath.Floor(JryMath.Log(JryMath.Min(size.X, size.Y), 2));
    //        var octave = JryMath.Min(myOctave, maxOctave);
    //        ILayerMasked[] layers = new ILayerMasked[octave];
    //        for (int i = 0; i < octave ; i++)
    //        {
    //            var pow = JryMath.Pow(2, i+3);
    //            // todo use base params

    //            INoiseParameters param = new NoiseParameters()
    //            {

    //                Amplitude = 1 / pow,
    //                BaseAmplitude = 1,
    //                FromDepth = 0,
    //                ToDepth = 3,
    //                Lacunarity = 1+ 1/pow ,
    //                Seed = f.Seed
    //            };

    //            // nice sand parameter
    //            //param = new NoiseParameters()
    //            //{

    //            //    Amplitude = 1 / pow,
    //            //    BaseAmplitude = 1,
    //            //    Depth = 5,
    //            //    Lacunarity = pow,
    //            //    Seed = f.Seed
    //            //};
    //            layers[i]= Noise.Do(param  , size);
    //        }

    //        var l = new Layer2DObject(size);
    //        _layerUtils.MergeLayers(layers,l);
    //        return l;

    //    }







    //}

}
