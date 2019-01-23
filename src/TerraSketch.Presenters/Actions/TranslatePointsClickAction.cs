using Common.DataObjects.Geometry;
using System;
using System.Linq;
using System.Numerics;
using TerraSketch.DataObjects.FieldObjects;

namespace TerraSketch.Presenters.Actions
{
    class TranslatePointsClickAction :AUpdatableMouseAction, IClickAction,IUpdatableMouseAction
    {
        private Vector2 first;
        public TranslatePointsClickAction( IFieldPolygon polygon):base(polygon)
        {
        }
        
     
        
        public void Update(Vector2 location)
        {
            if (performCalled) return;
            if(!firstSet)
            {
                first = new GeometryUtils().GetCenter(updatablePolygon.SelectedPoints.ToArray());
                firstSet = true;
                updated = first;
            }

            updatablePolygon.MoveSelected(location - updated);
            updated = location;
        }

     

      

        
    }
}
