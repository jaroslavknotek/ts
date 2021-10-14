using Common.MathUtils;
using TerraSketch.Layer.BlendModes;

namespace TerraSketch.Layer
{

    public class AlphaBlend : IBlendModeAlpha
    {
        public float Blend(float baseval, float toBeTransparent, float alpha)
        {
            return (1.0f - alpha) * baseval + alpha * toBeTransparent;
        }
    }

    public class AddBlendMode : IBlendMode
    {

        public float Blend(float value1, float value2)
        {
            return value1 + value2;
        }
    }
    public class SubtractBlendMode : IBlendMode
    {
        public float Blend(float value1, float value2)
        {
            return value1 - value2;
        }
    }




    public class FirstOnlyBlendMode : IBlendMode
    {

        public float Blend(float value1, float value2)
        {
            return value1;
        }
    }

    public class SecondOnlyBlendMode : IBlendMode
    {

        public float Blend(float value1, float value2)
        {
            return value2;
        }
    }



    public class MaxBlendMode : IBlendMode
    {

        public float Blend(float value1, float value2)
        {
            return JryMath.Max(value1, value2);
        }
    }



    public class MinBlendMode : IBlendMode
    {

        public float Blend(float value1, float value2)
        {
            return JryMath.Min(value1, value2);
        }
    }

    public class ScreenBlendMode : IBlendMode
    {

        public float Blend(float value1, float value2)
        {
            return 1 - (1 - value1) * (1 - value2);
        }
    }
    public class OverlayBlendMode : IBlendMode
    {
        public float Blend(float value1, float value2)
        {
            if (value1 < 0.5f) return 2 * value1 * value2;
            return 1 - (1 - value1) * (1 - value2);
        }
    }
    public class LightenBlendMode : IBlendMode
    {

        public float Blend(float value1, float value2)
        {
            return JryMath.Max(value1, value2);
        }
    }
    public class DarkenBlendMode : IBlendMode
    {

        public float Blend(float value1, float value2)
        {
            return JryMath.Min(value1, value2);
        }
    }

}
