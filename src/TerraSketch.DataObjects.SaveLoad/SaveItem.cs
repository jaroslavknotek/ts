using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TerraSketch.Layer;

namespace TerraSketch.DataObjects.SaveLoad
{
    public interface ISaveItem
    {
        IWorld World { get; set; }
        ILayerMasked Layer { get; set; }
    }
    
    public class SaveItem:ISaveItem
    {
    
        public IWorld World { get; set; }
    
        public ILayerMasked Layer { get; set; }
    }
}
