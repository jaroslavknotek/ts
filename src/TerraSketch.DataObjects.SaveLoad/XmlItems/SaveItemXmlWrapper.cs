using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TerraSketch.Layer;

namespace TerraSketch.DataObjects.SaveLoad
{
    [Serializable]
    public class SaveItemXmlWrapper
    {
        [XmlElement("World")]
        public WorldXmlWrapper World { get; set; }

        [XmlElement("X")]
        public int X { get; set; }

        [XmlElement("Y")]
        public int Y { get; set; }

        //[XmlElement("Layer")]
        //public float[] LayerData { get; set; }

    }



    public class SaveItemConverter
    {
        private const float nullRepr = float.MinValue;
        private ILoadItemParameter parameters;
        WorldConverter wc = null;
        public SaveItemConverter(ILoadItemParameter parameters)
        {
            this.parameters = parameters;
            wc = new WorldConverter(parameters);
        }
        public ISaveItem ToObject( SaveItemXmlWrapper wrapper)
        {
            SaveItem p = new SaveItem();

            //if (wrapper.LayerData != null && wrapper.LayerData.Length != 0)
            //{

            //    Layer2DObject l = new Layer2DObject(wrapper.X, wrapper.Y);
            //    for (int i = 0; i < wrapper.LayerData.Length; i++)
            //    {
            //        var x = i % wrapper.X;
            //        var y = i / wrapper.X;

            //        var cur = wrapper.LayerData[i];
            //        if (cur == nullRepr)
            //            l[x, y] = null;
            //        else
            //            l[x, y] = cur;
            //    }
            //    p.Layer = l;
            //}

            p.World = wc.ToObject(wrapper.World);

            return p;
        }

        public SaveItemXmlWrapper ToXmlWrapper(ISaveItem param)
        {
            SaveItemXmlWrapper pxw = new SaveItemXmlWrapper();

            if (param.Layer != null)
            {
                pxw.X = param.Layer.Resolution.X;
                pxw.Y = param.Layer.Resolution.Y;
                var arr = new float[pxw.X * pxw.Y];
                for (int y = 0; y < pxw.Y; y++)
                {
                    for (int x = 0; x < pxw.X; x++)
                    {
                        var val = param.Layer[x, y];
                        if (!val.HasValue)
                            arr[pxw.X * y + x] = nullRepr;
                        else
                            arr[pxw.X * y + x] = val.Value;
                    }
                }

              //  pxw.LayerData = arr; ;
            }

            pxw.World = wc.ToXmlWrapper(param.World);

            return pxw;
        }
    }
}
