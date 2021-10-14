using TerraSketch.DataObjects.ParameterObjects;

namespace TerraSketch.DataObjects.FieldObjects
{
    public delegate void ZOrderChangedEvent(object sender, ZOrderChangedEventArgs e);
    public class Field : IField
    {

        private int _zOrder;

        public int ZOrder
        {
            get { return _zOrder; }
            set
            {
                if (_zOrder == value) return;
                var previousZOrder = _zOrder;
                _zOrder = value;
                OnZOrderChanged(this, new ZOrderChangedEventArgs(previousZOrder, value));
            }
        }

        public event ZOrderChangedEvent ZOrderChanged;

        private void OnZOrderChanged(object sender, ZOrderChangedEventArgs e)
        {
            //ZOrderChanged?.Invoke(sender, e);
            if (ZOrderChanged != null) ZOrderChanged(sender, e);
        }

        public IFieldParameters Parameters
        {
            get;
        }

        public Field(IFieldPolygon polygon)
        {
            Parameters = new FieldParameters();


            Polygon = polygon;
        }

        public IFieldPolygon Polygon { get; set; }







    }

}