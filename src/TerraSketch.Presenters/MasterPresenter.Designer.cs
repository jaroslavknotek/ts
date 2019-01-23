using System;
using TerraSketch.DataObjects;

namespace TerraSketch.Presenters
{

    public partial class MasterPresenter : ABasePresenter
    {

        private IWorld _world;

        public IWorld World
        {
            get { return _world; }
            private set
            {
                if (_world == value) return;
                _world = value;
                NotifyPropertyChanged();
            }
        }
        private int _sensitivity = 30;
        public int MouseSensitivity
        {
            get
            {
                return _sensitivity;
            }

            set
            {
                if (_sensitivity == value) return;
                {
                    _sensitivity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string BitmapResolutionString
        {
            get
            {
                return World.Parameters.BitmapResolutionString;
            }

            set
            {
                try
                {
                    if (World.Parameters.BitmapResolutionString == value) return;

                    World.Parameters.BitmapResolutionString = value;
                    FieldPresenter.FieldView.RefreshView();
                    NotifyPropertyChanged();
                }
                catch (Exception)
                {

                    throw;
                }



            }
        }

        public int RiverAmount
        {
            get
            {
                return World.Parameters.RiverAmount;
            }

            set
            {
                try
                {
                    if (World.Parameters.RiverAmount == value || RiverAmount<0) return;
                    
                    World.Parameters.RiverAmount = value;
                    NotifyPropertyChanged();
                }
                catch (Exception)
                {

                    throw;
                }



            }
        }





        public float MaxHeight
        {
            get { return World.ExportParameters.MaxHeight; }
            set
            {
                if (World.ExportParameters.MaxHeight == value) return;
                World.ExportParameters.MaxHeight = value;
                NotifyPropertyChanged();
            }
        }


        public float MinHeight
        {
            get { return World.ExportParameters.MinHeight; }
            set
            {
                if (World.ExportParameters.MinHeight == value) return;
                World.ExportParameters.MinHeight = value;
                NotifyPropertyChanged();
            }
        }

        
    }
}
