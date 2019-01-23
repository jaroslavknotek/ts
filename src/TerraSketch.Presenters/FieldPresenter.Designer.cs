using Common.MathUtils;
using System.Collections.Generic;
using System.Linq;
using TerraSketch.DataObjects;
using TerraSketch.DataObjects.FieldObjects;
using TerraSketch.DataObjects.FieldObjects.FieldParams;
using TerraSketch.Heightmap.Composer;
using TerraSketch.DataObjects.Abstract;

namespace TerraSketch.Presenters
{

    public partial class FieldPresenter
    {
        private ZoomManager zoomManager = null;
        private readonly IHeightmapComposer composer ;

        public IntVector2 BaseResolution
        {
            get
            {
                if (ParentPresenter == null) return default(IntVector2);
                return ParentPresenter.World.Parameters.BitmapResolution;
            }
        }


        public IEnumerable<IFieldPolygon> GfxObjs
        {
            get
            {
                return World.Fields.Select(r => r.Polygon);
            }
        }

        private IField _selectedField;

        public IField SelectedField
        {
            get
            {
                return _selectedField;
            }
            set
            {
                if (_selectedField == value) return;
                _selectedField = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(() => HasSelectedField);
                NotifyPropertyChanged(() => SelectedFieldBlendMode);
                NotifyPropertyChanged(() => SelectedFieldDetail);
                NotifyPropertyChanged(() => SelectedFieldSeed);
                NotifyPropertyChanged(() => SelectedFieldHeightOffset);
                NotifyPropertyChanged(() => SelectedFieldProfile);
                NotifyPropertyChanged(() => SelectedFieldZOrder);
                NotifyPropertyChanged(() => CanChangeZOrder);
            }
        }


        public string SelectedFieldZOrder
        {
            get
            {
                if (SelectedField == null) return "";

                return SelectedField.ZOrder.ToString();

            }
            set
            {
                int zorder;
                if ( int.TryParse(value, out zorder)&& zorder == SelectedField.ZOrder) return;
                SelectedField.ZOrder = zorder;
                FieldView.RefreshView();
                NotifyPropertyChanged();
            }
        }


        public int SelectedFieldHeightOffset
        {
            get
            {
                if (SelectedField == null) return 0;
                return (int)(SelectedField.Parameters.Offset * 100);
            }
            set
            {
                if (SelectedField == null) return;

                var val = (float)value * .01f;
                if (val == SelectedField.Parameters.Offset) return;
                SelectedField.Parameters.Offset = val;
                NotifyPropertyChanged();

            }
        }

        public int SelectedErosionStrength
        {
            get
            {
                if (World == null) return 0;
                return (int)(World.Parameters.ErosionStrength );
            }
            set
            {
                if (World == null) return;
                if(World.Parameters.ErosionStrength == value) return;
                
                World.Parameters.ErosionStrength = value;
                NotifyPropertyChanged();

            }
        }


        #region FieldParameter Props

        public IList<AFieldBlendMode> GatheredFieldBlendModes { get; private set; }

       
        private IList<AFieldDetail> _gatheredDetails;

        public IList<AFieldDetail> GatheredDetails
        {
            get
            {
                return _gatheredDetails;
            }
        }
      
        private IList<AFieldProfile> _gatheredProfiles;

        public IList<AFieldProfile> GatheredProfile
        {
            get
            {
                return _gatheredProfiles;
            }
        }




        public IFieldProfile SelectedFieldProfile
        {
            get
            {
                if (SelectedField == null)
                    return null;
                return SelectedField.Parameters.FieldProfile;
            }
            set
            {
                if (SelectedField == null || SelectedField.Parameters.FieldProfile == value) return;
                SelectedField.Parameters.FieldProfile = value;
                NotifyPropertyChanged();
            }
        }

        public IFieldBlendMode SelectedFieldBlendMode
        {
            get
            {
                if (SelectedField == null)
                    return null;
                return SelectedField.Parameters.BlendModeWrap;
            }
            set
            {
                if (SelectedField == null || SelectedField.Parameters.BlendModeWrap == value) return;
                SelectedField.Parameters.BlendModeWrap = value;
                NotifyPropertyChanged();
            }
        }

        public IFieldDetail SelectedFieldDetail
        {
            get
            {
                if (SelectedField == null)
                    return null;
                return SelectedField.Parameters.Detail;
            }
            set
            {
                if (SelectedField == null || SelectedField.Parameters.Detail == value) return;
                SelectedField.Parameters.Detail = value;
                NotifyPropertyChanged();
            }
        }

        public string SelectedFieldSeed
        {
            get
            {
                if(SelectedField?.Parameters?.Seed == null) return null;
                return SelectedField.Parameters.Seed.ToString();
            }
            set
            {
                if (SelectedField == null) return;
                int val = 0;

                if (!int.TryParse(value, out val) && SelectedField.Parameters.Seed == val) return;

                SelectedField.Parameters.Seed = val;
                NotifyPropertyChanged();
            }
        }


        #endregion


        public float Zoom
        {
            get { return zoomManager.Zoom; }
            set { }

        }
        float IPresenterZoomable.Zoom
        {
            get { return zoomManager.Zoom; }

        }


        public bool HasSelectedField
        {
            get { return SelectedField != null; }
        }

        public IWorld World
        {
            get
            {
                if (ParentPresenter == null) return null;
                return ParentPresenter.World;
            }
        }

        public IFieldPolygon CurrentPolygon
        {
            get {
                if(SelectedField == null)return null;
                return SelectedField.Polygon; }
        }



        private FieldMode Mode
        {
            get
            {
                // TODO editing should suggest!! that its rotating or something
                //if (SelectedField != null)
                //    return FieldMode.Editing;

                return FieldMode.Idle;
            }
        }

        public bool CanChangeZOrder
        {
            get { return HasSelectedField && !HasSelectedBase; }
        }

        public bool HasSelectedBase
        {
            get
            {
                return SelectedField == BaseField;
            }
        }

        public bool UseBase
        {
            get { return World.UseBase; }
            set
            {
                if (World.UseBase == value) return;

                World.UseBase = value;
                NotifyPropertyChanged();
            }
        }

        public BaseField BaseField
        {
            get { return World.BaseField; }
        }


    }

    public enum FieldMode
    {
        CreatingNew, Editing, Idle
    }
}
