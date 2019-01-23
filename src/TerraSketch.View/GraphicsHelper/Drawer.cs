using System.Drawing;
using System.Numerics;

namespace TerraSketch.View.GraphicsHelper
{
    public class Drawer
    {
        private static Drawer _instance;

        public static Drawer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Drawer();
                return _instance;
            }
        }
        private Drawer()
        {

        }

        public void DrawLine(Graphics gfx, Pen pen, Vector2 l, Vector2 r)
        {
            Point p1 = new Point((int)l.X, (int)l.Y);
            Point p2 = new Point((int)r.X, (int)r.Y);
            gfx.DrawLine(pen, p1, p2);
        }
    }
}
