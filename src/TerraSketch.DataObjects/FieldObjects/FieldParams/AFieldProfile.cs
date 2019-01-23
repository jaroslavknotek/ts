namespace TerraSketch.DataObjects.FieldObjects.FieldParams
{
    public interface IFieldProfile
    {
        string Caption { get;  }
    }
   public abstract class AFieldProfile:IFieldProfile
    {
        public abstract string Caption { get;                }

        public static bool operator ==(AFieldProfile a, AFieldProfile b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.Caption == b.Caption;
        }
        public static bool operator !=(AFieldProfile a, AFieldProfile b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            var x = obj as AFieldProfile;
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
