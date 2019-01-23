using System;
using System.Numerics;

namespace TerraSketch.Presenters.Actions
{
    public interface IUpdatableMouseAction:IDisposable
    {
        void Update(Vector2 location);
    }
}
