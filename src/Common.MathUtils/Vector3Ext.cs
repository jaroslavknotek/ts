using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Common.MathUtils
{
 public static   class Vector3Ext
    {
        public static Vector3 Normalize(this Vector3 v)
        {
            return Vector3.Normalize(v);
        }
    }
}
