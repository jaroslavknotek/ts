using TerraSketch.DataObjects.Abstract;
using TerraSketch.Layer.BlendModes;

namespace TerraSketch.DataObjects.FieldObjects.FieldParams
{

    public abstract class AFieldBlendMode : IFieldBlendMode
    {
        public IBlendMode BlendMode { get; protected set; }
        public abstract string Caption { get; }

        public static bool operator ==(AFieldBlendMode a, AFieldBlendMode b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.Caption == b.Caption;
        }
        public static bool operator !=(AFieldBlendMode a, AFieldBlendMode b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            var x = obj as AFieldBlendMode;
            if (x == null) return false;

            return this == x;
        }

        public override int GetHashCode()
        {
            return Caption.GetHashCode();
        }


        public override string ToString()
        {
            return Caption;
        }

    }
}
