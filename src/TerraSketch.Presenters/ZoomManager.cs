using System.ComponentModel;
using System.Numerics;
using System.Windows.Input;
using TerraSketch.Presenters.Interfaces;

namespace TerraSketch.Presenters
{

   public class ZoomManager
    {

        private float _oneOverZoom = 1;
        private float _zoom = 1;

        public float Zoom
        {
            get { return _zoom; }
            set {
                if (_zoom != value)
                {
                    _zoom = value;
                    _oneOverZoom = 1 / _zoom;
                    if(ZoomChanged!=null)
                    ZoomChanged(this, new System.EventArgs());
                }
            }
        }
        public event ZoomChangedEventHandler ZoomChanged;

        private const float ZOOM_STEP = .1f;
        private const float ZOOM_MAX = 10;
        private const float ZOOM_MIN = .1f;

        private IRefreshableView view = null;
        public ZoomManager( IRefreshableView view)
        {
            this.view = view;
        }


        private ICommandWrapper _commandZoomIn = null;
        
        public ICommandWrapper CommandZoomIn
        {
            get
            {
                if(_commandZoomIn == null)
                _commandZoomIn = new CommandWrapper((object o) => CanExecuteCommandZoomIn(o)
                , (object o) => ExecuteCommandZoomIn(o));
                return _commandZoomIn;
            }
        }

        bool CanExecuteCommandZoomIn(object o)
        {

            return Zoom + ZOOM_STEP < ZOOM_MAX;

        }
        void ExecuteCommandZoomIn(object o)
        {

            Zoom += ZOOM_STEP;
            view.RefreshView();
        }

        private ICommandWrapper _commandZoomOut = null;

        

        public ICommandWrapper CommandZoomOut
        {
            get
            {
                if (_commandZoomOut == null)
                    _commandZoomOut = new CommandWrapper((object o) => CanExecuteCommandZoomOut(o)
                , (object o) => ExecuteCommandZoomOut(o));
                return _commandZoomOut;
            }
        }

        bool CanExecuteCommandZoomOut(object o)
        {
            return /*FieldView.IsFieldCanvasFocused() &&*/ Zoom - ZOOM_STEP > ZOOM_MIN;

        }
        void ExecuteCommandZoomOut(object o)
        {
            Zoom -= ZOOM_STEP;
            view.RefreshView();
        }


        public Vector2 RecalcZoomMult(Vector2 v)
        {
            return v * Zoom;
        }
        public Vector2 RecalcZoomDiv(Vector2 v)
        {
            return v * _oneOverZoom;
        }

        public float RecalcZoomMult(float f)
        {
            return f * Zoom;
        }
        public float RecalcZoomDiv(float f)
        {
            return f * _oneOverZoom;
        }
    }
}
