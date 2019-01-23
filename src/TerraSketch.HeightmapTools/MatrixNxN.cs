namespace TerraSketch.Heightmap.Tools
{
   public class MatrixNxN
    {
        public int Zeros { get; private set; }
        public uint N { get; private set; }
        private readonly float[][] array = null;
        
        public MatrixNxN( uint n)
        { 
            array = new float[n][];
            N = n;
            for (int i = 0; i < n; i++)
            {
                array[i] = new float[n];
            }
            Zeros = (int)(n * n);
        }
        public float this[uint x, uint y]
        {
            get
            {

                return getByCoordinates(x, y);
            }
            set {

                setByCoordinates(x, y, value);
            }
        }

        protected virtual void setByCoordinates(uint x, uint y, float value)
        {
            var val = array[y][x];
            if (val == 0 && value != 0) Zeros--;
            if (val != 0 && value== 0) Zeros++;
            array[y][x] = value;
        }

        protected virtual float getByCoordinates(uint x, uint y)
        {
            return array[y][x];
        }


        public static Dummy Empty = new Dummy();
        public class Dummy : MatrixNxN
        {
            public  Dummy() : base(1)
            {

            }

            protected override float getByCoordinates(uint x, uint y)
            {
                return 0;
            }

        }
    }
    
}
