using Common.DataObjects.Geometry;
using Common.MathUtils;
using System;
using System.Linq;
using System.Numerics;
using TerraSketch.DataObjects.FieldObjects;

namespace TerraSketch.Presenters.Actions
{
    class RotatePointsClickAction : AUpdatableMouseAction, IClickAction, IDisposable, IUpdatableMouseAction
    {
       
        
     
        private Vector2 firstLocation;
        public RotatePointsClickAction(IFieldPolygon polygon):base(polygon)
        {
            center = new GeometryUtils().GetCenter(updatablePolygon.SelectedPoints);
            updated = center;
        }
      
       

        public void Update(Vector2 location)
        {
            if (performCalled) return;
            if(!firstSet )
            {
                firstLocation = location;
                firstSet = true;
            }

            var dot = Vector2.Dot(firstLocation.Normalize(), location.Normalize());
            var angle = JryMath.Acos(dot);
            var a = (updated - center).Normalize();
            var b = (location - center).Normalize();
            angle = (float)(Math.Atan2(a.Y, a.X) -
                 Math.Atan2(b.Y, b.X));
            if (!float.IsNaN(angle))
            {
                updatablePolygon.RotateSelected(angle, center);
            }
            updated = location;
        }

    }
}
