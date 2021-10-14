namespace TerraSketch.Layer
{
    public class LayerStats
    {

        public int ValidCount { get; }
        public float Max { get; }

        public float Min { get; }

        public float Sum { get; }

        public float Average => ValidCount != 0 ? Sum / ValidCount : float.MinValue;

        public LayerStats(float min, float max, float sumVal, int countOfNonNull)
        {
            Min = min;
            Max = max;
            Sum = sumVal;
            ValidCount = countOfNonNull;
        }
    }
}
