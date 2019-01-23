using OpenTK;
using System;
using System.Drawing;
using System.Windows.Forms;
using TerraSketch.VisualPresenters;

namespace TerraSketch.View
{
    public partial class MasterView
    {
        // used for relative cooridnates
        private int x, y;
        public ICamera camera => Presenter?.Camera;
//test 
        public Visualization3DPresenter Presenter => MasterPreseneter.Visualization3DPresenter;


        public void SelectVisualView()
        {
            tcTabControl.SelectedTab = tcTabControl.TabPages[2];
        }

        private void init3DView()
        {
            visualization3DPresenterBindingSource.DataSource = Presenter;

            
        }

        /// <summary>
        /// Rendering of one frame.
        /// </summary>
        private void Render()
        {
            glControl1.MakeCurrent();
            
            
                Presenter.Render(HeightMapPresenter.HeightmapLayer
                , 1
                    , MasterPreseneter.MinHeight
                    , MasterPreseneter.MaxHeight
                    , HeightMapPresenter.HeightmapVersion);

            
            var gfx = pbMinimap.CreateGraphics();
            drawMiniMap(gfx, pbMinimap.Size.Width);

            glControl1.SwapBuffers();
        }

        /// <summary>
        /// Function called whenever the main application is idle..
        /// </summary>
        private void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                Render();
            }
        }

        private void drawMiniMap(Graphics gfx, int canvasSize)
        {
            var arrowSize = 30;
            int mapSizeX = 1000;
            int mapSizeY = 1000;
            if (rendered == null)
                gfx.FillRectangle(Brushes.Black, 0, 0, canvasSize, canvasSize);
            else
            {
                gfx.DrawImage(rendered, 0, 0, canvasSize, canvasSize);
                mapSizeX = rendered.Width;
                mapSizeY = rendered.Height;
            }
            var d = Presenter.Camera.Dir;
            var xzDir = d.Xz;
            var pos = camera.Center;
            var coef = ((float)canvasSize / mapSizeX);
            int camX = (int)(pos.X  *coef);
            int camY = (int)(pos.Z * coef);
            if (xzDir != Vector2.Zero)
            {
                var v = xzDir.Normalized();
                gfx.FillEllipse(Brushes.Red, camX -5, camY-5, 10,10);
                gfx.DrawLine(Pens.Red, camX, camY, camX + arrowSize * v.X, camY + arrowSize * v.Y);
            }
        }
        
        #region GL Events
        private void glControl1_Load(object sender, EventArgs e)
        {
            glControl1.VSync = true;
            int width = glControl1.Width;
            int height = glControl1.Height;
            Presenter.LoadOpenGL(width, height);
            Application.Idle += new EventHandler(Application_Idle);
        }


        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            if (camera == null) return;
            camera.UpdateRotation(x - e.X, y - e.Y, (float)MasterPreseneter.MouseSensitivity / 10);
        }


        void setPivot(int x, int y)
        {
            this.x = x; this.y = y;
        }
        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (camera == null) return;
            setPivot(e.X, e.Y);
            if (e.Button == MouseButtons.Left)
                camera.BeginRotation();
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (camera == null) return;
            if (e.Button == MouseButtons.Left)
                camera.EndRotation();
        }
        private void glControl1_Resize(object sender, EventArgs e)
        {
            int width = glControl1.Width;
            int height = glControl1.Height;
            Presenter.UpdateHeightWidth(width, height);
            Presenter.UpdateViewport();
            glControl1.Invalidate();
        }
        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (camera == null) return;
            switch (e.KeyCode)
            {
                case Keys.A:
                    camera.GoLeft();
                    break;
                case Keys.D:
                    camera.GoRight();
                    break;
                case Keys.S:
                    camera.GoBack();
                    break;
                case Keys.W:
                    camera.GoFront();

                    break;
                case Keys.Z:
                    camera.GoUp();
                    break;
                case Keys.X:
                    camera.GoDown();
                    break;
                case Keys.R:
                    camera.Reset();
                    break;
            }

        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }
        private void btnResetCamera_Click(object sender, EventArgs e)
        {
            Presenter.ResetCamera();
        }

        #endregion
    }
}
