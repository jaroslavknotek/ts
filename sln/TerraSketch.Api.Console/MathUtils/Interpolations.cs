namespace Common.MathUtils
{
    public interface IInterpolation
    {
        float Interpolate(float a, float b, float progress);

    }

    public class ToNearerInterpolationClipped : IInterpolation
    {
        public float Interpolate(float a, float b, float progress)
        {
            if (progress < .5) return a;
            return b;

        }
    }
    public class CosineClipped : IInterpolation
    {
        public float Interpolate(float a, float b, float progress)
        {

            if (progress < 0) return a;
            if (progress > 1) return b;

            float tm = (1 - JryMath.Cos(progress * JryMath.Pi)) / 2;

            return a * (1 - tm) + b * tm;

        }
    }

    public class LinearClipped : IInterpolation
    {
        public float Interpolate(float a, float b, float progress)
        {

            if (progress < 0) return a;
            if (progress > 1) return b;


            return a * (1 - progress) + b * progress;


        }
    }

}
