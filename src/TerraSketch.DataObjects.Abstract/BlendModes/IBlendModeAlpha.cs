using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraSketch.Layer.BlendModes
{
    public interface IBlendModeAlpha
    {
        float Blend(float baseval, float toBeTransparent, float alpha);
    }
}
