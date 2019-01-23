using Common.DataObjects.Geometry;
using Common.MathUtils;
using System.Numerics;

namespace TerraSketch.Layer
{
    public interface ILayer
    {
        IntVector2 Resolution { get;  }
        float? this[int x, int y] { get; set; }
        float? this[Vector2 v] { get; set; }
    }
    public interface ILayerMasked :ILayer
    {
        bool HasMask { get; }
        IMask Mask
        {
            get;
            set;
        }
        IntVector2 Offset { get;set; }
        Rect ValueArea { get; }

    }
}
