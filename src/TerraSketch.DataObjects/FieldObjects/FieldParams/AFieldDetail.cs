namespace TerraSketch.DataObjects.FieldObjects.FieldParams
{

    public interface IFieldDetail
    {
        string Caption { get; }
    }
   public abstract class AFieldDetail:IFieldDetail
    {
        public abstract string Caption { get; }

        public static bool operator ==(AFieldDetail a, AFieldDetail b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.Caption == b.Caption;
        }
        public static bool operator !=(AFieldDetail a, AFieldDetail b)
        {
            return !(a==b);
        }

        public override bool Equals(object obj)
        {
            var x = obj as AFieldDetail;
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
