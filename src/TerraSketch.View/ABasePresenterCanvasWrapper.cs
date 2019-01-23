using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using TerraSketch.Presenters;

namespace TerraSketch.View
{
    /// <summary>
    /// Object that represent data source for Canvas component. When bindn properly, object support zoom.
    ///  
    /// </summary>
    public abstract class ABasePresenterCanvasWrapper:INotifyPropertyChanged
    {

        private const int padding = 20;
        private const int scrollSize = 30;

        private IPresenterZoomable pres = null;

        public ABasePresenterCanvasWrapper(IPresenterZoomable p)
        {
            pres = p;
            pres.ZoomChanged += ZoomChanged;   
        }


        private void ZoomChanged(object sender, EventArgs arg)
        {
            // todo add nameof
            NotifyPropertyChanged("VScrollValue" );
            NotifyPropertyChanged("HScrollValue");

            //NotifyPropertyChanged(nameof(VScrollValue));
            //NotifyPropertyChanged(nameof(HScrollValue));
        }

        public Size CanvasSize
        {
            get
            {
                var v = pres.GetRecalculatedSize();
                var x = v.X;
                var y = v.Y;
                return new Size(x, y);
            }
        }

        public Size CavasContainerNoScrolls { get
            {
                return new Size(
                    CanvasContainerSize.Width - scrollSize
                    , CanvasContainerSize.Height - scrollSize
                    );
            }
        }

        private Size _canvasContainerSize;

        public Size CanvasContainerSize
        {
            get { return _canvasContainerSize; }
            set
            {
                if (_canvasContainerSize == value) return;
                _canvasContainerSize = value;
                    NotifyPropertyChanged();
                
            }
        }
        

        private int _hScrollValue;

        public int HScrollValue
        {
            get { return _hScrollValue; }
            set
            {
                if (value < MScrollMinValue || value >= HScrollMaxValue || _hScrollValue == value) return;
                    _hScrollValue = value;
                NotifyPropertyChanged();
            }
        }
        
        public int HScrollMaxValue
        {
            get

            {
                var val = CanvasSize.Width - CavasContainerNoScrolls.Width + 2 * padding;
                return Math.Max(0, val);
            }
        }

        private int _vScrolValue;
        public int VScrollValue
        {
            get {
                return _vScrolValue;
            }
            set
            {
                if (value < MScrollMinValue || value >= VScrollMaxValue ) return;
                _vScrolValue = value;

                NotifyPropertyChanged();
            }
        }


        
        public int VScrollMaxValue
        {
            get
            {
                var val = CanvasSize.Height - CavasContainerNoScrolls.Height +2*padding;
                return Math.Max(0, val);
            }
        }
        public int MScrollMinValue { get { return 0; } }

        public Point CanvasLocation
        {
            get
            {
                return new Point(
                    -HScrollValue +padding ,
               -VScrollValue +padding
                    );
            }
            set { }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if(PropertyChanged == null )return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private Point canvasCenter
        {
            get
            {
                // todo use cache
                return new Point(
                    CavasContainerNoScrolls.Width / 2,
                    CavasContainerNoScrolls.Height / 2
                    );
            }
        }
    }

 
}
