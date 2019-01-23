using System.Drawing;
using TerraSketch.DataObjects.FieldObjects;

namespace TerraSketch.View.GraphicsHelper
{
    interface IGfxPrinter
    {
        void Draw(Graphics gfx, IFieldPolygon planarObj, float zoom);
    }
}
