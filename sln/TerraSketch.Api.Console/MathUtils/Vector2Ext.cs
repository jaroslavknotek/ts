using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Common.MathUtils
{
    public static class Vector2Ext
    {
        //public static bool IsValid( this Vector2 v)
        //{
        //    return NumericsUtils.IsVector2Valid(v);
        //}

        public static Vector2 Normalize(this Vector2 v)
        {
            return Vector2.Normalize(v);
        }

        public static Vector2 Abs(this Vector2 vector)
        {
            return new Vector2(JryMath.Abs(vector.X), JryMath.Abs(vector.Y));
        }
    }
}
