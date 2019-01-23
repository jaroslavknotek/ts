using System;
using System.Numerics;
using Common.DataObjects.Geometry;

namespace TerraSketch.Layer
{
    public class LayerDrawer
    {
        private readonly ILayer _layer;
        private readonly LayerUtility _layerUtility = new LayerUtility();
        public LayerDrawer(ILayer l)
        {
            _layer = l;
        }

        public void DrawLineSafe(Vector2 a, Vector2 b)
        {

            var gscolor = 1;


            int x0 = (int)a.X;
            int y0 = (int)a.Y;
            int x1 = (int)b.X;
            int y1 = (int)b.Y;
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                int c1,c2;
                if (steep)
                {
                    c1 = y;
                    c2 = x;
                }
                else
                {
                    c1 = x;
                    c2 = y;
                }
                if (c1 >= 0 && c2 >= 0 &&
                    c1 < _layer.Resolution.X && c2 < _layer.Resolution.Y)

                    _layer[c1, c2] = gscolor;
                //   yield return new Point((steep ? y : x), (steep ? x : y));
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
        }


        // got from http://ericw.ca/notes/bresenhams-line-algorithm-in-csharp.html
        // but modidifed
        public void DrawLine(Vector2 a, Vector2 b)
        {

            var gscolor = 1;


            int x0 = (int)a.X;
            int y0 = (int)a.Y;
            int x1 = (int)b.X;
            int y1 = (int)b.Y;
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                _layer[(steep ? y : x), (steep ? x : y)] = gscolor;
                //   yield return new Point((steep ? y : x), (steep ? x : y));
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
        }

        // Draws each Y coordinates holds one point only.
        public void SketchLine(Vector2 v1, Vector2 v2)
        {
            var gscolor = 1;
            var b = v1.Y > v2.Y ? v1 : v2;
            var a = v1.Y <= v2.Y ? v1 : v2;


            int x0 = (int)a.X;
            int y0 = (int)a.Y;
            int x1 = (int)b.X;
            int y1 = (int)b.Y;

            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            float d = (float)dx / dy;
            //var error = d - (int)d;
            float x = x0;
            for (int i = y0; i < y1; i++)
            {
                _layer[(int)x, i] = gscolor;
                x += d;
            }

        }

        public void DrawArea(IArea area)
        {
            foreach (var segment in area.Segments)
            {
                DrawLineSafe(segment.Point1, segment.Point2);
            }
        }


        public void FillArea(IArea area)
        {
            DrawArea(area);
            var rand = new Random(area.Center.GetHashCode());
            _layerUtility.FloodFill(_layer, area.Center,0,(float)rand.NextDouble());
        }

        public void FillRectangle(Vector2 topLeftPosition, Vector2 size)
        {
            for (int i = (int)topLeftPosition.Y; i < topLeftPosition.Y+size.Y; i++)
            {
                DrawLineSafe( new Vector2(topLeftPosition.X,i), new Vector2(topLeftPosition.X+size.X, i));
            }

        }
    }
}
