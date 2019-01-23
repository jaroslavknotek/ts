using System.Numerics;
using TerraSketch.DataObjects.FieldObjects;

namespace TerraSketch.Presenters.Actions
{
    public abstract class AUpdatableMouseAction
    {
        // first position of cursor. its used in reject 
        protected Vector2 center;
        // using deltas only require additional fields
        protected Vector2 updated;

        protected bool firstSet = false;

        protected bool performCalled = false;
        public event EndPerformAction ActionDone;

        protected IFieldPolygon originalStatePoly;
        protected IFieldPolygon updatablePolygon;
        public AUpdatableMouseAction(IFieldPolygon original)
        {
            originalStatePoly = original.ShallowCopy();
            updatablePolygon = original;
        }

        public void Perform(Vector2 location)
        {
            performCalled = true;
            if (ActionDone != null)
                ActionDone.Invoke(this, new ActionDoneEventArgs(updatablePolygon));
        }
        public void Reject()
        {
            if (ActionDone != null)
                ActionDone(this, new ActionDoneEventArgs(originalStatePoly));
        }

        // TOOD implement advanced Disose pattern
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
