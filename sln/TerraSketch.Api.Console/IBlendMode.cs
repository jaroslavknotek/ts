using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraSketch.Layer.BlendModes
{
    public interface IBlendMode
    {
        float Blend(float value1, float value2);
    }
}
