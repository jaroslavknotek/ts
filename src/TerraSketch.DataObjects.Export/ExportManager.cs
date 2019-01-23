using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraSketch.Layer;

namespace TerraSketch.DataObjects.Export
{
    public interface IExportManager
    {
        void ExportToFile(FileInfo fileInfo, ILayer layer);
        void ExportToFile(FileInfo fileInfo, ILayer layer, float minCoef, float maxCoef);

    }
    public class ExportManager : IExportManager
    {
        private const String floatFormat = "0.0000";
        public void ExportToFile(FileInfo fileInfo, ILayer layer, float minCoef,float maxCoef)
        {
            
            
            using (var sw = new StreamWriter(fileInfo.FullName))
            {
                sw.WriteLine(minCoef.ToString(floatFormat));
                sw.WriteLine(maxCoef.ToString(floatFormat));
                for (int y = 0; y < layer.Resolution.Y; y++)
                {

                    for (int x = 0; x < layer.Resolution.X; x++)
                    {
                        var value = layer[x, y];
                        float outputValue = 0;
                        if (!value.HasValue)
                            outputValue = float.MinValue;
                        else
                            outputValue = value.Value;

                        sw.Write(outputValue.ToString(floatFormat));

                        if (x != layer.Resolution.X)
                            sw.Write(" ");
                    }

                    if (y != layer.Resolution.Y)
                        sw.Write(System.Environment.NewLine);
                }
            }
        }
        public void ExportToFile(FileInfo fileInfo, ILayer layer)
        {
            this.ExportToFile(fileInfo, layer, 0,100);
        }

    }
}
