using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.MathUtils;
using System.Numerics;

namespace Common.DataObjects.Geometry
{
    public class Ellipse
    {
        float a, b, c;
        Vector3 center;

        // Sphere constructor
        public Ellipse(float r, Vector3 center) : this(r, r, r, center)
        {
        }

        // Ellipse constructor
        public Ellipse(float a, float b, float c, Vector3 center)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.center = center;
        }

        // "polar coordinates" method
        public Vector3 IntersectionI(float x, float y)
        {
            Vector3 o = new Vector3(0, 0, -c);
            Vector3 m = new Vector3(x - center.X, y - center.Y, c);
            Vector3 v = o - m;
            
            v.Normalize();
            double A = v.X * v.X * b * b * c * c + v.Y * v.Y * a * a * c * c + v.Z * v.Z * a * a * b * b;
            double B = 2 * (v.X * b * b * c * c + v.Y * a * a * c * c + v.Z * a * a * b * b);
            double C = v.X * v.X * b * b * c * c + v.Y * v.Y * a * a * c * c + v.Z * a * a * b * b - a * a * b * b * c * c;
            double D = Math.Sqrt(B * B - 4 * A * C);
            double t = (-B - D) / (2 * A);
            double X = m.X + t * v.X;
            double Y = m.Y + t * v.Y;
            double Z = m.Z + t * v.Z;
            return new Vector3((float)X, -(float)Y, (float)Z);
        }

        // "parallel rays" method
        public Vector3? Intersection(float x, float y, bool restricted)
        {
            x -= center.X;
            y -= center.Y;

            if ((x < -a) || (x > a) || (y < -b) || (y > b))
            {
                float x1 = (float)Math.Sqrt((a * a * b * b * y * y) / (b * b * y * y + x * x));
                float x2 = -x1;
                float y1 = (y * x1) / -x;
                float y2 = (y * x2) / -x;
                if (Math.Abs(x - x1) < Math.Abs(x - x2))
                    return new Vector3(x1, y1, 0);
                else
                    return new Vector3(x2, y2, 0);
            }

            float z = (1 - (x * x) / (a * a) - (y * y) / (b * b)) * c * c;
            if (z < 0)
                return null;
            z = (float)Math.Sqrt(z);
            return new Vector3(x, -y, z);
        }
    }
}
