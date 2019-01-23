using Common.MathUtils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraSketch.Presenters
{
    
    public interface IPresenterZoomable
    {

        float Zoom { get; }
        IntVector2 GetRecalculatedSize();
        IntVector2 BaseResolution { get;}
        event ZoomChangedEventHandler ZoomChanged;
    }
}
