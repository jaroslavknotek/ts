using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraSketch.Layer;

namespace TerraSketch.SideDoorModule
{
    class LayerConverter
    {
        public ILayer LoadLayer(Bitmap m)
        {
            var l = new Layer2DObject(m.Size.Width,m.Size.Height);

            for (int y = 0; y < l.Resolution.Y; y++)
            {
                for (int x = 0; x < l.Resolution.X; x++)
                {

                    l[x, y] = (float)m.GetPixel(x,y).R / 256;
                } 
            }
            
            return l;
        }
    }
}
