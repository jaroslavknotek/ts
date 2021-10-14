using System;
using System.Drawing;
using System.IO;
// using TerraSketch.BitmapRendering;
using TerraSketch.Layer;

namespace TerraSketch.Logging
{

    public enum LogImageProcessingOptions
    {
        Normalize, Clip
    }

    /// <summary>
    /// Logs images to a file system. Logs only when Debug symbol is active.
    /// </summary>
    public class VisualLogger : IVisualLogger
    {
        private const string debugImageDirectoryName = "debugImage";
        //private readonly IBitmapRenderer _bitmapRenderer = new BitmapRenderer();
        public void Log(ILayer layer)
        {
            var name = "image";
            Log(layer, name);
        }
        public void Log(ILayer layer, string filename)
        {
            Log(layer, filename, LogImageProcessingOptions.Normalize);
        }

        public void Log(ILayer layer, string filename, LogImageProcessingOptions options)
        {
            // #if DEBUG
            //             // TODO NICE TO HAVE check that filename is valid path
            //             var currentDirectoryPath = Environment.CurrentDirectory;
            //             DirectoryInfo di = new DirectoryInfo(currentDirectoryPath);
            //             try
            //             {
            //                 di.CreateSubdirectory(debugImageDirectoryName);
            //             }
            //             catch (Exception)
            //             {
            //                 // directory exists
            //             }
            //             Bitmap bitmap;
            //             switch (options)
            //             {
            //                 case LogImageProcessingOptions.Normalize:
            //                     bitmap = _bitmapRenderer.RenderHeightMapToBitmap(layer);
            //                     break;
            //                 case LogImageProcessingOptions.Clip:
            //                     bitmap = _bitmapRenderer.RenderHeightMapToBitmapClip(layer);
            //                     break;
            //                 default:
            //                     throw new ArgumentOutOfRangeException(nameof(options), options, null);
            //             }

            //             bitmap.Save($"{currentDirectoryPath}\\{debugImageDirectoryName}\\{filename}-{DateTime.Now:yyMMdd}-{DateTime.Now:hhmmss}-{DateTime.Now.Millisecond}.bmp");
            // #endif
        }
    }
}
