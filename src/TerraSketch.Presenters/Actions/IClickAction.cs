using System;
using System.Numerics;

namespace TerraSketch.Presenters.Actions
{
    

    public class ActionDoneEventArgs : EventArgs
    {
        public Object Result { get; private set; }
        public ActionDoneEventArgs(Object result)
        {
            Result = result;
        }
    }
    
    public delegate void EndPerformAction(object sender, ActionDoneEventArgs e);
    public interface IClickAction:IDisposable
    {
        /// <summary>
        /// Action completed either sucessfuly or not
        /// </summary>
        event EndPerformAction ActionDone;
        void Perform(Vector2 location);
        void Reject();
    }
}
