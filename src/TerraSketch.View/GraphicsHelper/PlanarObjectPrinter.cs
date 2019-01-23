using System;
using System.Drawing;
using System.Linq;
using TerraSketch.DataObjects.FieldObjects;

namespace TerraSketch.View.GraphicsHelper
{
    public class PlanarObjectPrinter : IGfxPrinter
    {
        private static PlanarObjectPrinter p = new PlanarObjectPrinter();
        const int POINT_MARKER_SIZE = 10;
      static  Brush SELECTED = Brushes.Orange;
        public static void DrawObject(Graphics gfx, IFieldPolygon planarObj, float zoom )
        {
            p.Draw(gfx, planarObj, zoom);
        }

        public static void DrawSelectedObject(Graphics gfx, IFieldPolygon planarObj, float zoom )
        {
            p.drawPri(gfx, planarObj, zoom, SELECTED);
        }
        private PlanarObjectPrinter()
        {
        }



        public void Draw(Graphics gfx, IFieldPolygon planarObj, float zoom)
        {
            if (planarObj == null || zoom <= 0) return;

            Random rand = new Random(planarObj.GetHashCode());
            var fillColor = Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255));

            var b = new SolidBrush(fillColor);

            drawPri(gfx, planarObj, zoom, b);

        }

        private void drawPri(Graphics gfx, IFieldPolygon planarObj, float zoom, Brush b)
        {
            var color = Pens.Black;


            //if (planarObj.State == IPlanarObjectState.SelectedFinished)
            //    color = Pens.Green;

            gfx.FillPolygon(b, planarObj.Points.Select(r => new PointF(r.X * zoom, r.Y * zoom)).ToArray());

            foreach (var e in planarObj.Edges)
            {
                Drawer.Instance.DrawLine(gfx, color, e.Point1 * zoom, e.Point2 * zoom);
            }


            foreach (var item in planarObj.Points)
            {
                drawPoint(gfx, zoom, item, Brushes.Black);
            }
            foreach (var item in planarObj.SelectedPoints)
            {
                drawPoint(gfx, zoom, item, Brushes.OrangeRed);
            }
        }

        private void drawPoint(Graphics gfx, float zoom, System.Numerics.Vector2 item, Brush brush)
        {
            var s = (int)(POINT_MARKER_SIZE * zoom);
            var hs = (int)s / 2;
            var pointColor = brush;
            Point p = new Point((int)(item.X * zoom - hs), (int)(item.Y * zoom - hs));
            Size size = new Size(s, s);
            gfx.FillRectangle(pointColor, new Rectangle(p, size));
        }
    }
}
