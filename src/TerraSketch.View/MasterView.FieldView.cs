using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using TerraSketch.DataObjects.FieldObjects;
using TerraSketch.Presenters;
using TerraSketch.View.GraphicsHelper;

namespace TerraSketch.View
{
    public partial class MasterView
    {
        private IEnumerable<object> planars = new List<IFieldPolygon>();
        FieldPresenterViewWrapper fieldPresenterViewWrapper;
        public FieldPresenter FieldPresenter
        {
            get { return MasterPreseneter.FieldPresenter; }
        }

       
        public bool IsFieldCanvasFocused()
        {
            return pbFieldCanvas.Focused;
        }

        private FrmWaitView waitView;
        public void ShowProgressBar()
        {
            waitView = new FrmWaitView();
            waitView.Show(this);
        }

        public void CloseProgressBar()
        {
            waitView?.Close();
            waitView = null;
        }

        public void UpdateProgressBar(string text)
        {
            if (waitView == null || !waitView.Visible) return;
            waitView.UpdateText(text);
        }


        #region Init part
        private void bindFieldControls()
        {
            // takes care of all action that are to be perfomed after click
            binder.Bind(FieldPresenter.CommandPerformAction, pbFieldCanvas, Mouse.LeftDown);
            binder.Bind(FieldPresenter.CommandAddNewImmediately, pbFieldCanvas, Mouse.LeftDoubleclick);

            //action
            binder.Bind(FieldPresenter.CommandGenerate, btnGenerate);
            binder.Bind(FieldPresenter.CommandSetupBase, btnSetUpBase);

            #region Zoom commands
            binder.Bind(FieldPresenter.CommandZoomIn, tsbZoomIn);
            binder.Bind(FieldPresenter.CommandZoomOut, tsbZoomOut); 
            #endregion
            #region Order commands
            binder.Bind(FieldPresenter.CommandBringToFront, tsbtnBringToFront);
            binder.Bind(FieldPresenter.CommandBringToTop, tsbtnBringToTop);
            binder.Bind(FieldPresenter.CommandSendToBack, tsbtnSendToBack);
            binder.Bind(FieldPresenter.CommandSendToBottom, tsbtnSendToBottom);
            #endregion
            #region Segment commands
            binder.Bind(FieldPresenter.CommandSplit, tsbSplit);
            binder.Bind(FieldPresenter.CommandMerge,tsbMerge);
            #endregion
            #region Points commands
            binder.Bind(FieldPresenter.CommandRotate, tsbRotate);
            binder.Bind(FieldPresenter.CommandTranslate, tsbTranslate);
            binder.Bind(FieldPresenter.CommandScale, tsbScale);
            #endregion
            #region Add/Delete Select/Deselect
            binder.Bind(FieldPresenter.CommandAddNew, tsbAddNew);
            binder.Bind(FieldPresenter.CommandRejectAction, tsbRejectChanges);
            binder.Bind(FieldPresenter.CommandDeleteSelectedField, tsbDeleteField);
            binder.Bind(FieldPresenter.CommandDeleteSelected, tsbDeleteSelected);
            binder.Bind(FieldPresenter.CommandDeselectAll, tsbDeselectAll);
            binder.Bind(FieldPresenter.CommandSelectAll, tsbSelectAllPoints);
            #endregion



            ////keysonly
            //binder.Bind(FieldPresenter.CommandDelete,  Keys.Delete);
            //binder.Bind(FieldPresenter.CommandDeleteAll,  Keys.ShiftKey, Keys.Delete);
            //binder.Bind(FieldPresenter.CommandAcceptChanges,  Keys.Enter);


            //// issue with on_down_key 
            ////binder.Bind(FieldPresenter.CommandZoomIn, pbFieldCanvas, Keys.ControlKey, Keys.Add);
            ////binder.Bind(FieldPresenter.CommandZoomOut, pbFieldCanvas, Keys.ControlKey, Keys.Subtract);
            binder.Bind(FieldPresenter.CommandRotate, Keys.R);
            binder.Bind(FieldPresenter.CommandScale,  Keys.S);
            binder.Bind(FieldPresenter.CommandTranslate, Keys.T);

            binder.Bind(FieldPresenter.CommandRejectAction, Keys.Escape);

            // selecting
            binder.Bind(FieldPresenter.CommandSelectPolygonPoint, pbFieldCanvas, Mouse.RightDown);
            binder.Bind(FieldPresenter.CommandSelectPolygonPointMany, pbFieldCanvas, Mouse.RightDown, CommandBinder.ModifierKeys.Control);
        }


        private void initFieldView()
        {
            bindFieldControls();
            bindFieldData();



        }

        private void bindFieldData()
        {
            fieldPresenterViewWrapper = new FieldPresenterViewWrapper(FieldPresenter);
            fieldPresenterViewWrapperBindingSource.DataSource = new List<FieldPresenterViewWrapper>() { fieldPresenterViewWrapper };

            fieldPresenterBindingSource.DataSource = new List<FieldPresenter>() { FieldPresenter };

            fieldPresenterViewWrapper.CanvasContainerSize = pnlFieldCanvasSize.Size;
            fieldPresenterViewWrapper.HScrollValue = 0;
            fieldPresenterViewWrapper.VScrollValue = 0;
        }

        private void pbFieldCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (DesignMode) return;

            pbFieldCanvas.Size = fieldPresenterViewWrapper.CanvasSize;

            var gfx = e.Graphics;
            gfx.Clear(Color.White);

            // all valid polys
            foreach (var item in FieldPresenter.GfxObjs)
                PlanarObjectPrinter.DrawObject(gfx, item, FieldPresenter.Zoom);

            // this is currently edited poly not yet in presenter.gfxobj collection
            if (FieldPresenter.CurrentPolygon != null && !FieldPresenter.HasSelectedBase)
                PlanarObjectPrinter.DrawSelectedObject(gfx, FieldPresenter.CurrentPolygon, FieldPresenter.Zoom);
        }
        private void pbFieldCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if(FieldPresenter.CurrentUpdatableMouseAction != null)
            FieldPresenter.CurrentUpdatableMouseAction.Update(new Vector2(e.Location.X, e.Location.Y));
            pbFieldCanvas.Refresh();
        }


        #endregion

    }
}
