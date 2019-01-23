using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using TerraSketch.Presenters;

namespace TerraSketch.View
{
    public partial class MasterView
    {

        public HeightMapPresenter HeightMapPresenter
        {
            get { return MasterPreseneter.HeightmapPresenter; }
        }
        
        

        private Bitmap rendered;

        private void renderHeightMapWithBrush(Graphics gfx, Vector2 pos, int brushRadius)
        {
            if (rendered == null) //rerender
            {
                var final = HeightMapPresenter.GetCachedZoomedHeightmap();
                if (final == null) return;
                var min = HeightMapPresenter.HeightmapLayerMin;
                var max = HeightMapPresenter.HeightmapLayerMax;
                rendered = new BitmapRendering.BitmapRenderer().RenderHeightMapToBitmap(final,min,max);
            }

            if (rendered == null)//rerender was not sucessful
                return;
            gfx.DrawImage(rendered, new PointF(0, 0));
            gfx.DrawEllipse(Pens.Red, new RectangleF(pos.X - brushRadius, pos.Y - brushRadius, brushRadius * 2, brushRadius * 2));
        }
        private void initHeightmapView()
        {
            initHeightmapData();
            InitHeightmapControl();
        }

        private void InitHeightmapControl()
        {
            binder.Bind(HeightMapPresenter.CommandZoomIn, tsbtnHZoomIn);
            binder.Bind(HeightMapPresenter.CommandZoomOut, tsbtnHZoomOut);
            binder.Bind(MasterPreseneter.FieldPresenter.CommandGenerate, btnRecalc);
            binder.Bind(HeightMapPresenter.CommandClearSavedHeightmap, btnClearHeighmap);
        }

        private void initHeightmapData()
        {
            HeightmapPresenterViewWrapper hpvw = new HeightmapPresenterViewWrapper(HeightMapPresenter);
            heightmapPresenterViewWrapperBindingSource.DataSource = new List<HeightmapPresenterViewWrapper>() { hpvw };

            var hmpDS = new List<HeightMapPresenter>() { HeightMapPresenter };

            heightMapPresenterBindingSource.DataSource = hmpDS;

            pluginToolsBindingSource.DataSource = MasterPreseneter.HeightmapPresenter.PluginTools;

            cbBlendMode.DataSource = FieldPresenter.GatheredFieldBlendModes;
            
            cbDetail.DataSource = FieldPresenter.GatheredDetails;
            cbProfile.DataSource = FieldPresenter.GatheredProfile;
            hpvw.CanvasContainerSize = pnlHeightmapSize.Size;
        }

        public void SelectHeightmapView()
        {
            /*  tcTabControl.SelectedTab = tcTabControl.TabPages[1];*/
            tcTabControl.SelectedTab = tpHeightmapTab;
            ((IHeightmapView)this).RefreshView();
        }


        
        void IHeightmapView.RefreshView()
        {
            rendered = null;
            renderHeightMapWithBrush(pbHeightmapCanvas.CreateGraphics(), Vector2.Zero, 0);

        }

        public void ClearView()
        {
            pbHeightmapCanvas.CreateGraphics().Clear(Color.White);
        }

       
        private void pbHeightmapCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pb = sender as PictureBox;
            if (pb == null) return;


            var loc = new Vector2(e.Location.X, e.Location.Y);
            HeightMapPresenter.CurrentMousePosition = loc;
            rendered = null;
            renderHeightMapWithBrush(pb.CreateGraphics(),
            loc, (int)(HeightMapPresenter.BrushSize * HeightMapPresenter.Zoom));
            
            if (e.Button == MouseButtons.Left
                            && HeightMapPresenter.HasValidPluginSelected
                            && HeightMapPresenter.HeightmapLayer != null)
            {

                HeightMapPresenter.UpdatePaint(loc);
            }
        }

        private void pbHeightmapCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (HeightMapPresenter.CanPaint())
                HeightMapPresenter.BeginPaint(new Vector2(e.X, e.Y));
        }

        private void pbHeightmapCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (HeightMapPresenter.CanPaint())
            {
                HeightMapPresenter.FinishPaint(new Vector2(e.X, e.Y));
                rendered = null;
            }
        }
        private void pbHeightmapCanvas_Paint(object sender, PaintEventArgs e)
        {
            var pb = sender as PictureBox;
            if (pb == null) return;

            rendered = null;

            renderHeightMapWithBrush(e.Graphics, HeightMapPresenter.CurrentMousePosition, HeightMapPresenter.BrushSize);
        }
    }
}
