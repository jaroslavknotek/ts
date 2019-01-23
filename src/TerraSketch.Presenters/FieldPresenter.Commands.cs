using Common.MathUtils;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using TerraSketch.DataObjects.FieldObjects;
using TerraSketch.Presenters.Actions;
using TerraSketch.Presenters.Interfaces;

namespace TerraSketch.Presenters
{
    public partial class FieldPresenter
    {
        /// <summary>
        /// This is an action that has to wait for user input. Command sets this property and click performs it
        /// </summary>
        IClickAction _currentClickableAction;

        private IUpdatableMouseAction _currentUpdatableMouseAction;
        /// <summary>
        /// When rotate, translate... action button is pressed. This object represent the action which reacts on mouse impout appropriately.
        /// </summary>
        public IUpdatableMouseAction CurrentUpdatableMouseAction => _currentUpdatableMouseAction;


        private ICommandWrapper _commandPerformAction ;

        public ICommandWrapper CommandPerformAction
        {
            get
            {
                if (_commandPerformAction == null)
                    _commandPerformAction = new CommandWrapper(
                        CanExecutePerformAction,
                (o)=>ExecutePerformAction(o));

                return _commandPerformAction;
            }
        }

        bool CanExecutePerformAction(object o)
        {
            return _currentClickableAction != null;
        }
        void ExecutePerformAction(object o)
        {
            if (!CanExecutePerformAction(o)) return;

            var point = FieldView.GetCurrentMousePos();
            var location = zoomManager.RecalcZoomDiv(point);

            _currentClickableAction.Perform(location);
            _currentClickableAction.Dispose();
            _currentClickableAction = null;
        }


        public IntVector2 GetRecalculatedSize()
        {
            return zoomManager.RecalcZoomMult(World.Parameters.BitmapResolution);
        }

        public void UnselectField()
        {
            this.SelectedField = null;
        }

        private bool WorldIsValid
        {
            get
            {
                // todo all polygons visible
                // todo params valid
                return World.Fields.Any();
            }
        }


        #region Generator commands

        private ICommandWrapper _commandGenerate = null;


        public ICommandWrapper CommandGenerate
        {
            get
            {
                if (_commandGenerate == null)
                    _commandGenerate = new CommandWrapper((object o) => CanExecuteCommandGenerate(o),
                        async (object o) => await ExecuteCommandGenerate(o));
                return _commandGenerate;
            }
        }

        bool CanExecuteCommandGenerate(object o)
        {
            return WorldIsValid;
        }

        async Task ExecuteCommandGenerate(object o)
        {
            FieldView.ShowProgressBar();
            
            updateBasePoly();

            var hasIntersectingPolygon = World.Fields.Any(r => r.Polygon.CheckForIntersections());
            if (hasIntersectingPolygon 
                && !ParentPresenter.MasterView.ShowInfoYesNoMessage("Warning",
                "At least one polygon has intersecting lines. Do you still want to continue?"))
                return;
            FieldView.UpdateProgressBar("Starting generation");

            await composer.Compose(World);
            var layer = composer.ComposedLayer;

            FieldView.UpdateProgressBar("Object initialization");
            await Task.Factory.StartNew(() =>
            {
                ParentPresenter.HeightmapPresenter.InitializeGeneratedObjects(layer);
                ParentPresenter.MasterView.SelectHeightmapView();
            }
                , CancellationToken.None
            , TaskCreationOptions.None
            , _uiScheduler);



            FieldView.CloseProgressBar();
        }

        private ICommandWrapper _commandSetupBase = null;

        public ICommandWrapper CommandSetupBase
        {
            get
            {
                if (_commandSetupBase == null)
                    _commandSetupBase = new CommandWrapper((object o) => CanExecuteSelectBaseLayer(o)
                , (object o) => ExecuteSelectBaseLayer(o));

                return _commandSetupBase;
            }
        }

        bool CanExecuteSelectBaseLayer(object o)
        {
            return UseBase;
        }
        void ExecuteSelectBaseLayer(object o)
        {
            if(SelectedField !=null)
                SelectedField.Polygon.DeselectAll();
            SelectedField = BaseField;
        }


        #endregion

        #region Zoom in/out commands

        public ICommandWrapper CommandZoomIn
        {
            get
            {
                return zoomManager.CommandZoomIn;
            }
        }


        public ICommandWrapper CommandZoomOut
        {
            get
            {
                return zoomManager.CommandZoomOut;
            }
        }

        #endregion

        #region Segment commands
        private ICommandWrapper _commandSplit = null;

        public ICommandWrapper CommandSplit
        {
            get
            {
                if (_commandSplit == null)
                    _commandSplit = new CommandWrapper(
                        CanExecuteSplit,
                (o)=>ExecuteSplit(o));

                return _commandSplit;
            }
        }

        bool CanExecuteSplit(object o)
        {
            return CurrentPolygon != null && CurrentPolygon.HasSelectedEdges;
        }
        void ExecuteSplit(object o)
        {
            CurrentPolygon.SplitSelected();
            FieldView.RefreshView();
        }

        private ICommandWrapper _commandMerge = null;

        public ICommandWrapper CommandMerge
        {
            get
            {
                if (_commandMerge == null)
                    _commandMerge = new CommandWrapper(
                        CanExecuteMerge,
                (o)=>ExecuteMerge(o));

                return _commandMerge;
            }
        }

        bool CanExecuteMerge(object o)
        {
            return CurrentPolygon != null && CurrentPolygon.HasSelectedEdgesConnected
                && CurrentPolygon.SelectedPointsCount >= 2 && CurrentPolygon.PointsCount - CurrentPolygon.SelectedPointsCount >= 2;
        }
        void ExecuteMerge(object o)
        {
            CurrentPolygon.MergeSelected();
            FieldView.RefreshView();
        }
        #endregion

        #region Points commands
        private ICommandWrapper _commandRotate = null;

        public ICommandWrapper CommandRotate
        {
            get
            {
                if (_commandRotate == null)
                    _commandRotate = new CommandWrapper(
                        CanExecuteRotate,
                (o)=>ExecuteRotate(o));

                return _commandRotate;
            }
        }

        bool CanExecuteRotate(object o)
        {
            return CurrentPolygon != null && CurrentPolygon.HasSelectedPoints && FieldView.IsFieldViewSelected();
        }
        void ExecuteRotate(object o)
        {
            var ac = new RotatePointsClickAction(CurrentPolygon);
            _currentClickableAction = ac;
            _currentUpdatableMouseAction = ac;
            ac.ActionDone += afterAction;
        }
        private ICommandWrapper _commandScale = null;

        public ICommandWrapper CommandScale
        {
            get
            {
                if (_commandScale == null)
                    _commandScale = new CommandWrapper(
                        CanExecuteScale,
                (o) => ExecuteScale(o));

                return _commandScale;
            }
        }

        bool CanExecuteScale(object o)
        {
            return CurrentPolygon != null && CurrentPolygon.HasSelectedPoints && FieldView.IsFieldViewSelected();
        }
        void ExecuteScale(object o)
        {
            var ac = new ScalePointsClickAction(CurrentPolygon);
            _currentClickableAction = ac;
            _currentUpdatableMouseAction = ac;
            ac.ActionDone += afterAction;
        }
        private ICommandWrapper _commandTranslate = null;

        public ICommandWrapper CommandTranslate
        {
            get
            {
                if (_commandTranslate == null)
                    _commandTranslate = new CommandWrapper(
                        CanExecuteTranslate,
                (o) => ExecuteTranslate(o)
                );

                return _commandTranslate;
            }
        }

        bool CanExecuteTranslate(object o)
        {
            return CurrentPolygon != null && CurrentPolygon.HasSelectedPoints && FieldView.IsFieldViewSelected();
        }
        void ExecuteTranslate(object o)
        {
            var ac = new TranslatePointsClickAction(CurrentPolygon);
            _currentClickableAction = ac;
            _currentUpdatableMouseAction = ac;
            ac.ActionDone += afterAction;
        }
        #endregion

        #region Order Commands
        private ICommandWrapper _commandBringToFront = null;

        public ICommandWrapper CommandBringToFront
        {
            get
            {
                if (_commandBringToFront == null)
                    _commandBringToFront = new CommandWrapper(
                        CanExecuteBringToFront,
                (o) => ExecuteBringToFront(o)
                );

                return _commandBringToFront;
            }
        }

        bool CanExecuteBringToFront(object o)
        {
            return CanChangeZOrder && World.Fields.CanBeBroughtToFront(SelectedField);
        }
        void ExecuteBringToFront(object o)
        {
            World.Fields.BringToFront(SelectedField);
            NotifyPropertyChanged(() => SelectedFieldZOrder);
            FieldView.RefreshView();
        }


        private ICommandWrapper _commandBringToTop = null;

        public ICommandWrapper CommandBringToTop
        {
            get
            {
                if (_commandBringToTop == null)
                    _commandBringToTop = new CommandWrapper(
                        CanExecuteBringToTop,
                (o) => ExecuteBringToTop(o)
                );

                return _commandBringToTop;
            }
        }

        bool CanExecuteBringToTop(object o)
        {
            return CanChangeZOrder && World.Fields.CanBeBroughtToFront(SelectedField);
        }
        void ExecuteBringToTop(object o)
        {
            World.Fields.BringToTop(SelectedField);
            NotifyPropertyChanged(() => SelectedFieldZOrder);
            FieldView.RefreshView();
        }

        private ICommandWrapper _commandSendToBack = null;

        public ICommandWrapper CommandSendToBack
        {
            get
            {
                if (_commandSendToBack == null)
                    _commandSendToBack = new CommandWrapper(
                        CanExecuteSendToBack,
                (o) => ExecuteSendToBack(o)
                );

                return _commandSendToBack;
            }
        }

        bool CanExecuteSendToBack(object o)
        {
            return CanChangeZOrder && World.Fields.CanBeSentToBack(SelectedField);
        }
        void ExecuteSendToBack(object o)
        {
            World.Fields.SendToBack(SelectedField);
            NotifyPropertyChanged(() => SelectedFieldZOrder);
            FieldView.RefreshView();
        }



        private ICommandWrapper _commandSendToBottom = null;

        public ICommandWrapper CommandSendToBottom
        {
            get
            {
                if (_commandSendToBottom == null)
                    _commandSendToBottom = new CommandWrapper(
                        CanExecuteSendToBottom,
                (o) => ExecuteSendToBottom(o)
                );

                return _commandSendToBottom;
            }
        }

        bool CanExecuteSendToBottom(object o)
        {
            return CanChangeZOrder && World.Fields.CanBeSentToBack(SelectedField);
        }
        void ExecuteSendToBottom(object o)
        {
            World.Fields.SendToBottom(SelectedField);
            NotifyPropertyChanged(() => SelectedFieldZOrder);
            FieldView.RefreshView();
        }

        #endregion

        #region Add/Delete Select/Unselect
        private ICommandWrapper _commandRejectAction = null;

        public ICommandWrapper CommandRejectAction
        {
            get
            {
                if (_commandRejectAction == null)
                    _commandRejectAction = new CommandWrapper(
                        CanExecuteRejectAction,
                (o) => ExecuteRejectAction(o)
                );

                return _commandRejectAction;
            }
        }

        bool CanExecuteRejectAction(object o)
        {
            return _currentClickableAction != null || CurrentUpdatableMouseAction != null;
        }
        void ExecuteRejectAction(object o)
        {
            if (_currentClickableAction != null)
                _currentClickableAction.Reject();
            _currentUpdatableMouseAction = null;
            if (_currentClickableAction != null)
                _currentClickableAction.Dispose();
            _currentClickableAction = null;
            if (CurrentUpdatableMouseAction != null)
                CurrentUpdatableMouseAction.Dispose();

        }
        private ICommandWrapper _commandAddNew = null;


        public ICommandWrapper CommandAddNew
        {
            get
            {
                if (_commandAddNew == null)

                    _commandAddNew = new CommandWrapper((object o) => CanExecuteAddNew(o),
                        (object o) => ExecuteAddNew(o));
                return _commandAddNew;
            }
        }



        void ExecuteAddNew(object o)
        {
            if (CurrentPolygon != null)
                CurrentPolygon.DeselectAll();


            _currentClickableAction = new NewFieldClickAction();
            _currentClickableAction.ActionDone += (sender, args) =>
            {
                var poly = args.Result as IFieldPolygon;
                if (poly == null) return;
                commitPoly(poly);

                FieldView.RefreshView();
            };
        }



        bool CanExecuteAddNew(object o)
        {
            return Mode == FieldMode.Idle || Mode == FieldMode.Editing;
        }



        private ICommandWrapper _commandAddNewImmediately = null;


        public ICommandWrapper CommandAddNewImmediately
        {
            get
            {
                if (_commandAddNewImmediately == null)

                    _commandAddNewImmediately = new CommandWrapper((object o) => CanExecuteAddNewImmediately(o),
                        (object o) => ExecuteAddNewImmediately(o));
                return _commandAddNewImmediately;
            }
        }



        void ExecuteAddNewImmediately(object o)
        {
            if (CurrentPolygon != null)
                CurrentPolygon.DeselectAll();

            var hSize = 20;
            var location = FieldView.GetCurrentMousePos();
            var polygon = new FieldPolygon();
            polygon.AddPoint(new Vector2(location.X - hSize, location.Y - hSize));
            polygon.AddPoint(new Vector2(location.X + hSize, location.Y - hSize));
            polygon.AddPoint(new Vector2(location.X + hSize, location.Y + hSize));
            polygon.AddPoint(new Vector2(location.X - hSize, location.Y + hSize));
            commitPoly(polygon);
            FieldView.RefreshView();
        }



        bool CanExecuteAddNewImmediately(object o)
        {
            return Mode == FieldMode.Idle || Mode == FieldMode.Editing;
        }


        private ICommandWrapper _commandSelectPolygonPointMany = null;


        public ICommandWrapper CommandSelectPolygonPointMany
        {
            get
            {
                if (_commandSelectPolygonPointMany == null)
                    _commandSelectPolygonPointMany = new CommandWrapper((object o) => CanExecuteCommandSelectPolygonPointMany(o),
                        (object o) => ExecuteCommandSelectPolygonPointMany(o));
                return _commandSelectPolygonPointMany;
            }

        }

        bool CanExecuteCommandSelectPolygonPointMany(object o)
        {
            return Mode == FieldMode.Idle;
        }

        void ExecuteCommandSelectPolygonPointMany(object o)
        {
            // todo rewrite
            var point = zoomManager.RecalcZoomDiv(FieldView.GetCurrentMousePos());

            if (SelectedField != null)//this means that it has selected point of current field
            {
                var poly = SelectedField.Polygon;
                if (poly.IsNearToSelected(point))
                {
                    deselect(point);
                    FieldView.RefreshView();
                    return;
                }
                else if (poly.Select(point))
                {
                    FieldView.RefreshView();
                    return;
                }
            }

            if(SelectedField!=null)
            SelectedField.Polygon.DeselectAll();

            foreach (var field in World.Fields)
            {
                if (field == SelectedField) continue;
                if (field.Polygon.Select(point))
                {
                    SelectedField = field;
                    break;
                }
            }

            FieldView.RefreshView();
        }

        private ICommandWrapper _commandSelectPolygonPoint = null;


        public ICommandWrapper CommandSelectPolygonPoint
        {
            get
            {
                if (_commandSelectPolygonPoint == null)
                    _commandSelectPolygonPoint = new CommandWrapper((object o) => CanExecuteCommandSelectPolygonPoint(o),
                        (object o) => ExecuteCommandSelectPolygonPoint(o));
                return _commandSelectPolygonPoint;
            }

        }

        bool CanExecuteCommandSelectPolygonPoint(object o)
        {
            return Mode == FieldMode.Idle;
        }

        void ExecuteCommandSelectPolygonPoint(object o)
        {

            var point = zoomManager.RecalcZoomDiv(FieldView.GetCurrentMousePos());
            if (SelectedField != null)
                SelectedField.Polygon.DeselectAll();
            if (SelectedField != null && SelectedField.Polygon.Select(point))//this means that it has selected point of current field
            {
                FieldView.RefreshView();
                return;
            }
            SelectedField = null;
            foreach (var field in World.Fields.OrderByDescending(r => r.ZOrder))
            {
                if (field.Polygon.Select(point))
                {
                    SelectedField = field;
                    break;
                }
                else if (field.Polygon.IsInPoly(point))
                {
                    SelectedField = field;
                    SelectedField.Polygon.SelectAll();
                    break;
                }

            }

            FieldView.RefreshView();
        }


        private ICommandWrapper _commandDelete = null;

        public ICommandWrapper CommandDeleteSelected
        {
            get
            {
                if (_commandDelete == null)
                    _commandDelete = new CommandWrapper(
                        CanExecuteDelete,
                (o) => ExecuteDelete(o)
                        );

                return _commandDelete;
            }
        }

        bool CanExecuteDelete(object o)
        {
            return CurrentPolygon != null && CurrentPolygon.HasSelectedPoints;
        }
        void ExecuteDelete(object o)
        {
            if (CurrentPolygon.PointsCount - CurrentPolygon.SelectedPointsCount >= 3)
            {
                CurrentPolygon.RemoveSelected();
                FieldView.RefreshView();
            }
            else
            {
                if (ParentPresenter.MasterView.ShowInfoYesNoMessage("Are you sure", "This operation will delete field"))
                    deleteSelectedField();
            }
            FieldView.RefreshView();

        }
        private ICommandWrapper _commandDeleteSelectedField = null;

        public ICommandWrapper CommandDeleteSelectedField
        {
            get
            {
                if (_commandDeleteSelectedField == null)
                    _commandDeleteSelectedField = new CommandWrapper(
                        CanExecuteDeleteSelectedField,
                (o) => ExecuteDeleteSelectedField(o)
                );

                return _commandDeleteSelectedField;
            }
        }

        bool CanExecuteDeleteSelectedField(object o)
        {
            return SelectedField != null && !HasSelectedBase;
        }
        void ExecuteDeleteSelectedField(object o)
        {
            if (!ParentPresenter.MasterView.ShowInfoYesNoMessage("Confirm", "Are you sure to delete field")) return;

            deleteSelectedField();

            FieldView.RefreshView();
        }

        private void deleteSelectedField()
        {
            World.Fields.Remove(SelectedField);

            SelectedField = null;
        }

        private ICommandWrapper _commandSelectAll = null;

        public ICommandWrapper CommandSelectAll
        {
            get
            {
                if (_commandSelectAll == null)
                    _commandSelectAll = new CommandWrapper(
                        CanExecuteSelectAll,
                (o) => ExecuteSelectAll(o)
                        );

                return _commandSelectAll;
            }
        }

        bool CanExecuteSelectAll(object o)
        {
            return CurrentPolygon != null && CurrentPolygon.HasAnyNotSelectedPoint && !HasSelectedBase;
        }
        void ExecuteSelectAll(object o)
        {
            if(CurrentPolygon!=null)
            CurrentPolygon.SelectAll();
            FieldView.RefreshView();
        }


        private ICommandWrapper _commandUnselectAll = null;

        public ICommandWrapper CommandDeselectAll
        {
            get
            {
                if (_commandUnselectAll == null)
                    _commandUnselectAll = new CommandWrapper(
                        CanExecuteDeselectAll,
                (o) => ExecuteDeselectAll(o)
                        );

                return _commandUnselectAll;
            }
        }

        bool CanExecuteDeselectAll(object o)
        {
            return CurrentPolygon != null && CurrentPolygon.HasSelectedPoints;
        }
        void ExecuteDeselectAll(object o)
        {
            if (CurrentPolygon != null)
                CurrentPolygon.DeselectAll();
            SelectedField = null;
            FieldView.RefreshView();
        }


        #endregion


        private void deselect(Vector2 v)
        {
            SelectedField.Polygon.Deselect(v);

            if (SelectedField.Polygon.SelectedPointsCount == 0)
                SelectedField = null;
        }

        private void afterAction(object sender, ActionDoneEventArgs arg)
        {
            var poly = arg.Result as IFieldPolygon;
            if(poly!=null && SelectedField!=null)
            {
                SelectedField.Polygon = poly;
            }


            FieldView.RefreshView();

        }
    }
}
