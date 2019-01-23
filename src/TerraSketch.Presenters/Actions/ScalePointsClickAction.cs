using Common.DataObjects.Geometry;
using System;
using System.Linq;
using System.Numerics;
using TerraSketch.DataObjects.FieldObjects;

namespace TerraSketch.Presenters.Actions
{
    class ScalePointsClickAction : AUpdatableMouseAction, IClickAction,IUpdatableMouseAction
    {
        public ScalePointsClickAction( IFieldPolygon polygon):base(polygon)
        {
            
            updatablePolygon = polygon;
        }
       
        public void Update(Vector2 location)
        {
            if (performCalled) return;
            if(!firstSet)
            {
                center  = new GeometryUtils().GetCenter(updatablePolygon.SelectedPoints.ToArray());
                firstSet = true;
                updated = center;
            }
            var temp = location - updated;
            var f = (temp - center).Length() / center.Length();
            var factor = f * f * f *f;
            updatablePolygon.ScaleSelected( factor, center);
            updated = location;
        }

      

        
    }
}
