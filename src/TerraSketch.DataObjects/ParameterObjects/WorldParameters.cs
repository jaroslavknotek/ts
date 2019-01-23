using Common.MathUtils;

namespace TerraSketch.DataObjects.ParameterObjects
{
    
    public class WorldParameters :IWorldParameters

    {
        private const float houndreth = (float)1 / 100;

        
        public string BitmapResolutionString
        {
            get { return string.Format("{0}:{1}", BitmapResolution.X, BitmapResolution.Y); }
            set
            {

                var sp = value.Split(':');
                if (sp.Length != 2) return;
                float x;
                float y;

                // TODO error if too large
                if (!float.TryParse(sp[0], out x) || !float.TryParse(sp[1], out y) || x < 0 || y < 0 || x > 4096 || y > 4096)
                    return;


                BitmapResolution = new IntVector2((int)x, (int)y);

            }
        }

        private IntVector2 _bitmapResolution;
        
        public IntVector2 BitmapResolution
        {
            get
            {
                return _bitmapResolution;
            }
            set
            {
                if (_bitmapResolution != value )
                    _bitmapResolution = value;
            }
        }
        //private float _maxHeight;

        //public float MaxHeight
        //{
        //    get
        //    {
        //        return _maxHeight;
        //    }

        //    set
        //    {
        //        if (value != _maxHeight && value > MinHeight)
        //            _maxHeight = value;
                
        //    }
        //}

        //public float  MaxHeightPercent
        //{
        //    get { return MaxHeight * 100; }
        //    set { MaxHeight = value * houndreth; }
        //}

        //private float _minHeight;
        //public float MinHeight
        //{
        //    get
        //    {
        //        return _minHeight;
        //    }

        //    set
        //    {
        //        if (value != _minHeight && value < MaxHeight)
        //            _minHeight = value;
        //    }
        //}
        //public float MinHeightPercent
        //{
        //    get { return MinHeight * 100; }
        //    set { MinHeight = value* houndreth; }
        //}

        private float _seaLevel;

        // sea level i percentage
        public float SeaLevel
        {
            get
            {
                return _seaLevel;
            }

            set
            {
                if (value != _seaLevel)
                    _seaLevel = value;
            }
        }
        public float SeaLevelPercent
        {
            get { return SeaLevel * 100; }
            set { SeaLevel = value* houndreth; }
        }

        private float _baseLevel;
        // in percentage
        public float BaseLevel
        {
            get
            {
                return  _baseLevel;
            }

            set
            {
                if (value != _baseLevel)
                    _baseLevel = value;
            }
        }


        public float BaseLevelPercent
        {
            get { return _baseLevel * 100; }
            set { _baseLevel = value * houndreth; }
        }

        public WorldParameters()
        {
            //MaxHeight = .3f;
            //MinHeight = .1f;
            SeaLevel = .15f;
            BaseLevel = .2f;
            BitmapResolution = new IntVector2(512, 512);
        }

        public int ErosionStrength { get; set; }
        public int RiverAmount { get; set; }
    }
}
