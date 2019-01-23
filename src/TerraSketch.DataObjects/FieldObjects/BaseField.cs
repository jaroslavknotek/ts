using Common.MathUtils;
using TerraSketch.DataObjects.ParameterObjects;

namespace TerraSketch.DataObjects.FieldObjects
{
    public class BaseField : IField
    {

        public BaseField()
        {

            Parameters = new FieldParameters();
        }

        public BaseField( IFieldPolygon poly, IFieldParameters fp)
        {
            Polygon = poly;
            Parameters = fp;
            
        }

        public event ZOrderChangedEvent ZOrderChanged;

        public void UpdatePolygon(IntVector2 v)
        {
            var p = new FieldPolygon();
            p.AddPoint(new System.Numerics.Vector2(0, 0));
            p.AddPoint(new System.Numerics.Vector2(v.X, 0));
            p.AddPoint(new System.Numerics.Vector2(v.X,v.Y));
            p.AddPoint(new System.Numerics.Vector2(0, v.Y));
            Polygon = p;
        }

        public IFieldParameters Parameters
        {
            get;private set;
        }

        public IFieldPolygon Polygon
        {
            get;  set;
        }

        public int ZOrder
        {
            get
            {
                return -1;
            }
            set
            {

                //throw new InvalidOperationException("Cannot set ZOrder");
            }
        }

    }
}
