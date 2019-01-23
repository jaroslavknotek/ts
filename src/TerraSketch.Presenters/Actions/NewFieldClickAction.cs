using System;
using System.Numerics;
using TerraSketch.DataObjects.FieldObjects;

namespace TerraSketch.Presenters.Actions
{
    class NewFieldClickAction :  IClickAction
    {
        private const int hSize = 20;

        //public event BeginPerformAction BeginPerformAction;
        public event EndPerformAction ActionDone;

        public void Perform(Vector2 location)
        {
            //BeginPerformAction?.Invoke(this, new BeforePerformActionEventArgs(location));
            var polygon = new FieldPolygon();
            polygon.AddPoint(new Vector2(location.X - hSize, location.Y - hSize));
            polygon.AddPoint(new Vector2(location.X + hSize, location.Y - hSize));
            polygon.AddPoint(new Vector2(location.X + hSize, location.Y + hSize));
            polygon.AddPoint(new Vector2(location.X - hSize, location.Y + hSize));

            if(ActionDone!=null)
                ActionDone(this, new ActionDoneEventArgs(polygon));
        }
        public void Reject()
        {

        }
        

        public void Dispose()
        {
           
            if (ActionDone != null)
                foreach (var item in ActionDone.GetInvocationList())
                {
                    ActionDone -= (EndPerformAction)item;
                }
        }


    }
}
